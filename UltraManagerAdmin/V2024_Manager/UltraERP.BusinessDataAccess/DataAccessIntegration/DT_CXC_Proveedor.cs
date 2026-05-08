using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_CXC_Proveedor : DT
    {
        #region Variables
        #endregion

        #region Constructor
        public DT_CXC_Proveedor() : base() { }
        #endregion

        public List<EN_CXC_Proveedor> GetAll()
        {
            List<EN_CXC_Proveedor> list =
                (from p in db.UEP_CXC_PROVEEDOR_GETALL()
                 select new EN_CXC_Proveedor()
                 {
                     ID = p.ID,
                     NumeroContrato = p.NumeroContrato,
                     FormaRebajoID = p.FormaRebajoID,
                     VigenciaDesde = p.VigenciaDesde,
                     VigenciaHasta = p.VigenciaHasta,
                     ProveedorID = p.ProveedorID,
                     CasaComercialID = p.CasaComercialID ?? 0,
                     Estado = p.Estado
                 }).ToList();
            return list;
        }

        public List<EN_CXC_FormaRebajo> GetAll_FormaRebajos()
        {
            List<EN_CXC_FormaRebajo> formaRebajos =
                (from f in db.UEP_CXC_FORMAREBAJO_GETALL()
                 select new EN_CXC_FormaRebajo() { ID = f.ID, Nombre = f.Nombre }
                 ).ToList();
            return formaRebajos;
        }

        public List<EN_Tipo> GetAll_IDCTipo()
        {
            List<EN_Tipo> data =
                (from i in db.UEP_CXC_IDCTIPO_GETALL()
                 select new EN_Tipo() { ID = i.ID, Nombre = i.Nombre }
                 ).ToList();

            return data;
        }

        public List<EN_Tipo> GetAll_IICTipo()
        {
            List<EN_Tipo> data =
                (from i in db.UEP_CXC_IICTIPO_GETALL()
                 select new EN_Tipo() { ID = i.ID, Nombre = i.Nombre }
                 ).ToList();

            return data;
        }

        public List<EN_Tipo> GetAll_FrecuenciaPagos()
        {
            List<EN_Tipo> data =
                (from i in db.UEP_CXC_FRECUENCIAPAGO_GETALL()
                 select new EN_Tipo() { ID = i.ID, Nombre = i.Nombre }
                 ).ToList();

            return data;
        }

        public List<EN_Tipo> GetAll_Negociaciones()
        {
            List<EN_Tipo> data =
                (from i in db.UEP_CXC_NEGOCIACION_GETALL()
                 select new EN_Tipo() { ID = i.ID, Nombre = i.Nombre }
                 ).ToList();

            return data;
        }

        public List<EN_CXC_IDC> GetAll_IDC(int fichaID)
        {
            List<EN_CXC_IDC> list =
                (from i in db.UEP_CXC_IDC_GETALL(fichaID)
                 select new EN_CXC_IDC()
                 {
                     CXC_ProveedorID = i.CXC_ProveedorID,
                     Estado = 1,
                     FrecuenciaPagoID = i.FrecuenciaPagoID,
                     ID = i.ID,
                     IDCTipoID = i.IDCTipoID,
                     ListaCategorias = i.ListaCategorias,
                     NegociacionID = i.NegociacionID,
                     ValorNegMonetizado = ((i.ValorNegMonetizado ?? 0)),
                     ValorNegPorcentaje = (float)(i.ValorNegPorcentaje ?? 0)
                 }).ToList();
            return list;
        }

        public List<EN_CXC_IIC> GetAll_IIC(int fichaID)
        {
            List<EN_CXC_IIC> list =
                (from i in db.UEP_CXC_IIC_GETALL(fichaID)
                 select new EN_CXC_IIC()
                 {
                     CXC_ProveedorID = i.CXC_ProveedorID,
                     Estado = 1,
                     FrecuenciaPagoID = i.FrecuenciaPagoID,
                     ID = i.ID,
                     IICTipoID = i.IICTipoID,
                     ListaCategorias = i.ListaCategorias,
                     NegociacionID = i.NegociacionID,
                     ValorNegMonetizado = (double)(i.ValorNegMonetizado ?? 0),
                     ValorNegPorcentaje = (float)(i.ValorNegPorcentaje ?? 0)
                 }).ToList();
            return list;
        }

        public Dictionary<string, object> Save_CXC_Proveedor(EN_CXC_Proveedor ficha)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@CasaComercialID", ficha.CasaComercialID),
                new SqlParameter("@FormaRebajoID", ficha.FormaRebajoID),
                new SqlParameter("@ProveedorID", ficha.ProveedorID),
                new SqlParameter("@VigenciaDesde", ficha.VigenciaDesde),
                new SqlParameter("@VigenciaHasta", ficha.VigenciaHasta)
            };
            Dictionary<string, object> data = new Dictionary<string, object>();
            try
            {
                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_PROVEEDOR_INSERT", parameters);

                if (dataReader.Read())
                {
                    string result = dataReader.IsDBNull(dataReader.GetOrdinal("RESPUESTA")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("RESPUESTA"));
                    string contrato = dataReader.IsDBNull(dataReader.GetOrdinal("NUM_CONTRATO")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("NUM_CONTRATO"));
                    int scope = dataReader.IsDBNull(dataReader.GetOrdinal("SCOPE")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("SCOPE"));
                    data.Add("RESULT", result);
                    data.Add("CONTRATO", result);
                    data.Add("SCOPE", scope);
                }
                else data.Add("RESULT", "error_reg_insertado");
            }
            catch (Exception)
            {
                data.Add("RESULT", "error_reg_insertado");
            }

            return data;
        }

        public Respuesta Save_IDC(EN_CXC_IDC idc, int CXC_ProveedorID)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@CXC_ProveedorID", CXC_ProveedorID),
                new SqlParameter("@IDCTipoID", idc.IDCTipoID),
                new SqlParameter("@FrecuenciaPagoID", idc.FrecuenciaPagoID),
                new SqlParameter("@NegociacionID", idc.NegociacionID),
                new SqlParameter("@ValorNegPorcentaje", (idc.ValorNegPorcentaje)),
                new SqlParameter("@ValorNegMonetizado", (idc.ValorNegMonetizado)),
                new SqlParameter("@ListaCategorias", idc.ListaCategorias)
            };
            try
            {
                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_IDC_INSERT", parameters);

                if (dataReader.Read())
                {
                    string result = dataReader.IsDBNull(dataReader.GetOrdinal("RESPUESTA")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("RESPUESTA"));
                    int scope = dataReader.IsDBNull(dataReader.GetOrdinal("SCOPE")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("SCOPE"));

                    if (result == "succ_insertado") return new Respuesta("", result, scope, true);
                    else return new Respuesta("", result, null, false);
                }
                return new Respuesta("RESULTADO: NO LEIDO", "error_leer_proc", null, false);
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "error_guardar", null, false);
            }
        }
        public Respuesta Save_IIC(EN_CXC_IIC iic, int CXC_ProveedorID)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@CXC_ProveedorID", CXC_ProveedorID),
                new SqlParameter("@iicTipoID", iic.IICTipoID),
                new SqlParameter("@FrecuenciaPagoID", iic.FrecuenciaPagoID),
                new SqlParameter("@NegociacionID", iic.NegociacionID),
                new SqlParameter("@ValorNegPorcentaje", (iic.ValorNegPorcentaje)),
                new SqlParameter("@ValorNegMonetizado", (iic.ValorNegMonetizado)),
                new SqlParameter("@ListaCategorias", iic.ListaCategorias)
            };

            try
            {
                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_IIC_INSERT", parameters);

                if (dataReader.Read())
                {
                    string result = dataReader.IsDBNull(dataReader.GetOrdinal("RESPUESTA")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("RESPUESTA"));
                    int scope = dataReader.IsDBNull(dataReader.GetOrdinal("SCOPE")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("SCOPE"));

                    if (result == "succ_insertado") return new Respuesta("", result, scope, true);
                    else return new Respuesta("", result, null, false);
                }
                return new Respuesta("RESULTADO: NO LEIDO", "error_leer_proc", null, false);
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "error_guardar", null, false);
            }
        }

        public Respuesta DeleteIDC(int ID)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@ID", ID) };
                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_IDC_DELETE", parameters);

                if (dataReader.Read())
                {
                    string result = dataReader.IsDBNull(dataReader.GetOrdinal("RESPUESTA")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("RESPUESTA"));

                    if (result == "succ_delete") return new Respuesta("", result, null, true);
                    else return new Respuesta("", result, null, false);
                }
                return new Respuesta("RESULTADO: NO LEIDO", "error_leer_proc", null, false);
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "Error eliminar", null, false);
            }
        }

        public Respuesta DeleteIIC(int ID)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@ID", ID) };
                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_IIC_DELETE", parameters);

                if (dataReader.Read())
                {
                    string result = dataReader.IsDBNull(dataReader.GetOrdinal("RESPUESTA")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("RESPUESTA"));

                    if (result == "succ_delete") return new Respuesta("", result, null, true);
                    else return new Respuesta("", result, null, false);
                }
                return new Respuesta("RESULTADO: NO LEIDO", "error_leer_proc", null, false);
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "Error eliminar", null, false);
            }
        }

        public Respuesta ChangeStatus_CXC_Proveedor(int proveedorID, string status)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[] {
                    new SqlParameter("@CXC_ProveedorID", proveedorID),
                    new SqlParameter("@Estado", status)
                };

                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_CXC_PROVEEDOR_CHANGE_STATUS", parameters);

                if (dataReader.Read())
                {
                    string result = dataReader.IsDBNull(dataReader.GetOrdinal("RESPUESTA")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("RESPUESTA"));

                    if (result == "cambio_success") return new Respuesta("", "FICHA GUARDADA", null, true);
                    else return new Respuesta("", "FICHA NO GUARDADA", null, false);
                }
                return new Respuesta("RESULTADO: NO LEIDO", "error_leer_proc", null, false);
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "Error eliminar", null, false);
            }
        }
    }
}
