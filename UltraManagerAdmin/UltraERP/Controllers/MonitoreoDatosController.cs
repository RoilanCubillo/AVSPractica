using Analitic.Entities;
using Security.EntitiesAVS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;
using UltraERP.Models;

namespace UltraERP.Controllers
{
    [Authorize]
    public class MonitoreoDatosController : Controller
    {
        private const int DefaultResultCount = 500;

        public ActionResult Inicio()
        {
            MonitoreoDatosViewModel model = BuildModel();
            return View(model);
        }

        private MonitoreoDatosViewModel BuildModel()
        {
            var model = new MonitoreoDatosViewModel();

            try
            {
                model.Tiendas = GetStoresFromSql();
                model.DocumentosMH = GetDocumentosHaciendaFromSql(DefaultResultCount);
            }
            catch (Exception ex)
            {
                model.Alertas.Add("No se pudieron cargar documentos de Hacienda desde SQL: " + ex.Message);
            }

            try
            {
                model.DocumentosERP = GetDocumentosErpFromAnalitica(DefaultResultCount);
                model.AsientosERP = GetAsientosFromAnalitica(DefaultResultCount);
            }
            catch (Exception ex)
            {
                model.Alertas.Add(GetAnaliticaUnavailableMessage(ex));
            }

            ApplyDefaultDates(model);
            return model;
        }

        private IList<MonitoreoTiendaViewModel> GetStoresFromSql()
        {
            var stores = new List<MonitoreoTiendaViewModel>();
            ConnectionStringSettings settings = GetMasterConnection();

            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
                return stores;

            using (var connection = new SqlConnection(settings.ConnectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
                    SELECT ID, StoreCode, Name
                    FROM dbo.Store
                    WHERE (@Stores = '%' OR ID IN (SELECT TRY_CONVERT(INT, value) FROM STRING_SPLIT(@Stores, ',') WHERE value <> ''))
                    ORDER BY Name";
                command.Parameters.AddWithValue("@Stores", GetStoresAvailable());

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string id = Convert.ToString(ReadInt(reader, "ID"));
                        string code = ReadString(reader, "StoreCode");

                        stores.Add(new MonitoreoTiendaViewModel
                        {
                            ID = id,
                            Codigo = String.IsNullOrWhiteSpace(code) ? id : code,
                            Nombre = ReadString(reader, "Name")
                        });
                    }
                }
            }

            return stores;
        }

        private IList<MonitoreoDocumentoViewModel> GetDocumentosHaciendaFromSql(int take)
        {
            var documents = new List<MonitoreoDocumentoViewModel>();
            ConnectionStringSettings settings = GetMasterConnection();

            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
                return documents;

            using (var connection = new SqlConnection(settings.ConnectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
                    SELECT TOP (@Take)
                        ROW_NUMBER() OVER (ORDER BY I.FECHA_TRANSAC DESC, I.TRANSACTIONNUMBER DESC) AS RowID,
                        I.COD_SUCURSAL,
                        ISNULL(S.Name, 'Tienda ' + I.COD_SUCURSAL) AS StoreName,
                        I.TRANSACTIONNUMBER,
                        I.CLAVE50,
                        I.CLAVE20,
                        I.COMPROBANTE_TIPO,
                        I.COD_CLIENTE,
                        I.NOMBRE_CLIENTE,
                        I.FECHA_TRANSAC,
                        I.FECHA_HACIENDA,
                        I.ESTADO_HACIENDA,
                        I.OBSERVACIONES,
                        I.XML_RESPUESTA
                    FROM dbo.AVS_INTEGRAFAST_01 I
                    LEFT JOIN dbo.Store S ON S.ID = TRY_CONVERT(INT, I.COD_SUCURSAL)
                    WHERE (@Stores = '%' OR TRY_CONVERT(INT, I.COD_SUCURSAL) IN (SELECT TRY_CONVERT(INT, value) FROM STRING_SPLIT(@Stores, ',') WHERE value <> ''))
                    ORDER BY I.FECHA_TRANSAC DESC, I.TRANSACTIONNUMBER DESC";
                command.Parameters.AddWithValue("@Take", take <= 0 ? DefaultResultCount : take);
                command.Parameters.AddWithValue("@Stores", GetStoresAvailable());

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string estado = ReadString(reader, "ESTADO_HACIENDA");
                        DateTime fecha = ReadNullableDate(reader, "FECHA_TRANSAC") ?? DateTime.MinValue;

                        documents.Add(new MonitoreoDocumentoViewModel
                        {
                            ID = ReadInt(reader, "RowID"),
                            Origen = "MH",
                            TiendaID = ReadString(reader, "COD_SUCURSAL"),
                            Tienda = ReadString(reader, "StoreName"),
                            Consecutivo = ReadString(reader, "TRANSACTIONNUMBER"),
                            Clave = ReadString(reader, "CLAVE50"),
                            ComprobanteTipo = GetComprobanteDescription(ReadString(reader, "COMPROBANTE_TIPO")),
                            Cliente = ReadString(reader, "NOMBRE_CLIENTE"),
                            Fecha = fecha == DateTime.MinValue ? DateTime.Now : fecha,
                            FechaSincronizacion = ReadNullableDate(reader, "FECHA_HACIENDA"),
                            Total = 0,
                            EstadoID = MapHaciendaStatus(estado),
                            Mensaje = FirstNotEmpty(ReadString(reader, "OBSERVACIONES"), ReadString(reader, "XML_RESPUESTA"), "Sin mensaje registrado.")
                        });
                    }
                }
            }

            return documents;
        }

        private IList<MonitoreoDocumentoViewModel> GetDocumentosErpFromAnalitica(int take)
        {
            EN_DocumentosERP[] records = ExecuteAnalitica(client => client.GetAllDocsERP(
                GetStoresAvailable(),
                "%",
                "",
                "",
                0,
                "desc",
                0,
                take <= 0 ? DefaultResultCount : take,
                "",
                ""));

            return (records ?? new EN_DocumentosERP[0])
                .Select((x, index) => new MonitoreoDocumentoViewModel
                {
                    ID = index + 1,
                    Origen = "ERP",
                    TiendaID = Convert.ToString(x.StoreId),
                    Tienda = x.StoreName,
                    Consecutivo = x.Documento,
                    Clave = x.Documento,
                    ComprobanteTipo = x.Tipo,
                    Cliente = "Documento ERP",
                    Fecha = x.Fecha_Envio ?? DateTime.Now,
                    FechaSincronizacion = x.Fecha_Envio,
                    Total = 0,
                    EstadoID = MapErpDocumentStatus(x.Status),
                    Mensaje = FirstNotEmpty(x.Detalles_Envio, x.Respuesta_Envio, "Sin detalle registrado.")
                })
                .ToList();
        }

        private IList<MonitoreoAsientoViewModel> GetAsientosFromAnalitica(int take)
        {
            EN_Asientos[] records = ExecuteAnalitica(client => client.GetAllAsientos(
                GetStoresAvailable(),
                "%",
                "",
                "",
                0,
                "desc",
                0,
                take <= 0 ? DefaultResultCount : take,
                "",
                ""));

            return (records ?? new EN_Asientos[0])
                .Select(x => new MonitoreoAsientoViewModel
                {
                    ID = x.ID,
                    TiendaID = Convert.ToString(x.StoreId),
                    Tienda = x.StoreName,
                    NumeroAsiento = Convert.ToString(x.ID),
                    Referencia = x.Referencia,
                    Tipo = x.Descripcion,
                    FechaAsiento = x.Fecha_Asiento ?? DateTime.Now,
                    FechaSincronizacion = x.Fecha_Sync,
                    Debito = 0,
                    Credito = 0,
                    EstadoID = MapAccountingStatus(x.Estado_Sync),
                    Mensaje = FirstNotEmpty(x.Detalle_Sync, "Sin detalle registrado.")
                })
                .ToList();
        }

        private static void ApplyDefaultDates(MonitoreoDatosViewModel model)
        {
            IEnumerable<DateTime> dates = (model.DocumentosMH ?? new List<MonitoreoDocumentoViewModel>()).Select(x => x.Fecha)
                .Concat((model.DocumentosERP ?? new List<MonitoreoDocumentoViewModel>()).Select(x => x.Fecha))
                .Concat((model.AsientosERP ?? new List<MonitoreoAsientoViewModel>()).Select(x => x.FechaAsiento))
                .Where(x => x > DateTime.MinValue);

            DateTime maxDate = dates.Any() ? dates.Max() : DateTime.Today;
            model.FechaHastaDefault = maxDate.ToString("yyyy-MM-dd");
            model.FechaDesdeDefault = maxDate.AddDays(-30).ToString("yyyy-MM-dd");
        }

        private static string GetComprobanteDescription(string code)
        {
            switch ((code ?? "").TrimStart('0'))
            {
                case "1": return "Factura electronica";
                case "3": return "Nota credito";
                case "4": return "Tiquete electronico";
                case "9": return "Factura exportacion";
                default: return String.IsNullOrWhiteSpace(code) ? "Documento electronico" : "Tipo " + code;
            }
        }

        private static int MapHaciendaStatus(string status)
        {
            switch ((status ?? "").TrimStart('0'))
            {
                case "1":
                case "4":
                    return 1;
                case "2":
                case "55":
                    return 2;
                default:
                    return 3;
            }
        }

        private static int MapErpDocumentStatus(int status)
        {
            return status == 2 || status == 4 ? 1 : status == 3 ? 2 : 3;
        }

        private static int MapAccountingStatus(int status)
        {
            return status == 1 ? 1 : status == 2 ? 2 : 0;
        }

        private static string FirstNotEmpty(params string[] values)
        {
            return values.FirstOrDefault(x => !String.IsNullOrWhiteSpace(x)) ?? "";
        }

        private static T ExecuteAnalitica<T>(Func<IAnaliticaService, T> action)
        {
            var binding = new BasicHttpBinding();
            binding.OpenTimeout = TimeSpan.FromSeconds(8);
            binding.SendTimeout = TimeSpan.FromSeconds(20);
            binding.ReceiveTimeout = TimeSpan.FromSeconds(20);
            binding.CloseTimeout = TimeSpan.FromSeconds(5);
            binding.MaxReceivedMessageSize = 20000000;
            binding.MaxBufferSize = 20000000;
            binding.MaxBufferPoolSize = 20000000;

            string serviceUrl = ConfigurationManager.AppSettings["AnaliticaServiceUrl"];
            if (String.IsNullOrWhiteSpace(serviceUrl))
                serviceUrl = "http://192.168.137.219:7580/UltraERP_Service/AnaliticaService.svc";

            var factory = new ChannelFactory<IAnaliticaService>(binding, new EndpointAddress(serviceUrl));
            IAnaliticaService channel = factory.CreateChannel();
            IClientChannel clientChannel = (IClientChannel)channel;

            try
            {
                T result = action(channel);
                clientChannel.Close();
                factory.Close();
                return result;
            }
            catch
            {
                clientChannel.Abort();
                factory.Abort();
                throw;
            }
        }

        [ServiceContract(Namespace = "http://tempuri.org/", ConfigurationName = "wcfAnalitica.IAnaliticaService")]
        private interface IAnaliticaService
        {
            [OperationContract(Action = "http://tempuri.org/IAnaliticaService/GetAllDocsERP", ReplyAction = "http://tempuri.org/IAnaliticaService/GetAllDocsERPResponse")]
            EN_DocumentosERP[] GetAllDocsERP(string storesID, string hqUsersID, string searchValue, string tipoDocumento, int orderColumn, string orderDirection, int skip, int take, string fromDate, string toDate);

            [OperationContract(Action = "http://tempuri.org/IAnaliticaService/GetAllAsientos", ReplyAction = "http://tempuri.org/IAnaliticaService/GetAllAsientosResponse")]
            EN_Asientos[] GetAllAsientos(string storesID, string hqUsersID, string searchValue, string estadoAsiento, int orderColumn, string orderDirection, int skip, int take, string fromDate, string toDate);
        }

        private static string GetAnaliticaUnavailableMessage(Exception ex)
        {
            string message = ex == null ? "" : ex.Message;

            if (message.IndexOf("network-related", StringComparison.OrdinalIgnoreCase) >= 0 ||
                message.IndexOf("timed out", StringComparison.OrdinalIgnoreCase) >= 0 ||
                message.IndexOf("server was not found", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "El servicio de Analitica no esta disponible en este momento. Se muestran los documentos de Hacienda y se omiten temporalmente Documentos ERP y Asientos ERP.";
            }

            return "No se pudo cargar la informacion de Analitica. Se muestran los documentos de Hacienda disponibles.";
        }

        private static ConnectionStringSettings GetMasterConnection()
        {
            return ConfigurationManager.ConnectionStrings["UltraERP.BusinessDataAccess.Properties.Settings.MasterDB"];
        }

        private string GetStoresAvailable()
        {
            string dataStoreCode = ConfigurationManager.AppSettings["DataStoreCode"] ?? "uerp-store";
            EN_SC_DataAccess[] dataAccess = Session["USER_DATAACCESS"] as EN_SC_DataAccess[];

            if (dataAccess == null || dataAccess.Length == 0)
                return "%";

            EN_SC_DataAccess[] storesAccess = dataAccess.Where(x => x.Code == dataStoreCode).ToArray();
            if (storesAccess.Length == 0 || storesAccess.Any(x => x.EnableAll))
                return "%";

            return String.Join(",", storesAccess.Where(x => !String.IsNullOrWhiteSpace(x.DataIDs)).Select(x => x.DataIDs));
        }

        private static string ReadString(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? "" : Convert.ToString(reader[column]);
        }

        private static int ReadInt(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? 0 : Convert.ToInt32(reader[column]);
        }

        private static DateTime? ReadNullableDate(SqlDataReader reader, string column)
        {
            int ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? (DateTime?)null : Convert.ToDateTime(reader[column]);
        }
    }
}
