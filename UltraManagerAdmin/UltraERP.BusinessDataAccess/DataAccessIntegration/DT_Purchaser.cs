using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_Purchaser : DT
    {
        public DT_Purchaser() : base() { }

        public List<EN_Purchaser> GetAll()
        {
            try
            {
                return (from p in db.UEP_PURCHASER_GETALL()
                        select new EN_Purchaser
                        {
                            ID = p.ID,
                            Code = p.Code,
                            Name = p.Name,
                            Inactive = p.Inactive,
                            EmailAddress = p.EmailAddress,
                            ExtCode = p.ExtCode,
                            Telephone = p.Telephone,
                            LastUpdated = p.LastUpdated
                        }).ToList();
            }
            catch (SqlException)
            {
                return GetAllDirect(null);
            }
        }

        public List<EN_Purchaser> GetAllByInactive(bool inactive)
        {
            try
            {
                return (from p in db.UEP_PURCHASER_GETALL_BY_INACTIVE(inactive)
                        select new EN_Purchaser
                        {
                            ID = p.ID,
                            Code = p.Code,
                            Name = p.Name,
                            Inactive = p.Inactive,
                            EmailAddress = p.EmailAddress,
                            ExtCode = p.ExtCode,
                            Telephone = p.Telephone,
                            LastUpdated = p.LastUpdated
                        }).ToList();
            }
            catch (SqlException)
            {
                return GetAllDirect(inactive);
            }
        }

        public Respuesta Save(EN_Purchaser pur)
        {
            if (pur == null)
                return new Respuesta("PURCHASER_NULL", "No se recibio la casa comercial.", null, false);

            try
            {
                if (ProcedureExists("UEP_PURCHASER_INSERT_UPDATE"))
                {
                    Respuesta respuesta = new Respuesta("NO ACCIONADO", "error_guardar", null, false);

                    foreach (var i in db.UEP_PURCHASER_INSERT_UPDATE(
                        pur.ID,
                        pur.Code,
                        pur.ExtCode,
                        pur.Name,
                        pur.EmailAddress,
                        pur.Telephone,
                        pur.Inactive))
                    {
                        respuesta = new Respuesta("", i.RESPUESTA, i.SCOPE, !i.RESPUESTA.Contains("error"));
                    }

                    return respuesta;
                }
            }
            catch (SqlException ex)
            {
                if (!IsMissingProcedure(ex))
                    throw;
            }

            return SaveDirect(pur);
        }

        private List<EN_Purchaser> GetAllDirect(bool? inactive)
        {
            var purchasers = new List<EN_Purchaser>();

            using (var connection = new SqlConnection(con.CadenaConexion))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
                    SELECT ID, Code, ExtCode, Name, EmailAddress, Telephone, Inactive, LastUpdated
                    FROM dbo.POA_Purchaser
                    WHERE (@Inactive IS NULL OR Inactive = @Inactive)
                    ORDER BY Name, Code";

                command.Parameters.AddWithValue("@Inactive", inactive.HasValue ? (object)inactive.Value : DBNull.Value);
                connection.Open();

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                        purchasers.Add(MakePurchaser(dataReader));
                }
            }

            return purchasers;
        }

        private Respuesta SaveDirect(EN_Purchaser pur)
        {
            using (var connection = new SqlConnection(con.CadenaConexion))
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandType = CommandType.Text;

                bool exists = false;

                if (pur.ID > 0)
                {
                    command.CommandText = "SELECT COUNT(1) FROM dbo.POA_Purchaser WHERE ID = @ID";
                    command.Parameters.AddWithValue("@ID", pur.ID);
                    exists = Convert.ToInt32(command.ExecuteScalar()) > 0;
                    command.Parameters.Clear();
                }

                command.Parameters.AddWithValue("@Code", SafeValue(pur.Code, 20));
                command.Parameters.AddWithValue("@ExtCode", SafeValue(pur.ExtCode, 20));
                command.Parameters.AddWithValue("@Name", SafeValue(pur.Name, 50));
                command.Parameters.AddWithValue("@EmailAddress", SafeValue(pur.EmailAddress, 255));
                command.Parameters.AddWithValue("@Telephone", SafeValue(pur.Telephone, 30));
                command.Parameters.AddWithValue("@Inactive", pur.Inactive);

                if (exists)
                {
                    command.CommandText = @"
                        UPDATE dbo.POA_Purchaser
                        SET Code = @Code,
                            ExtCode = @ExtCode,
                            Name = @Name,
                            EmailAddress = @EmailAddress,
                            Telephone = @Telephone,
                            Inactive = @Inactive,
                            LastUpdated = GETDATE()
                        WHERE ID = @ID;

                        SELECT @ID;";

                    command.Parameters.AddWithValue("@ID", pur.ID);
                }
                else
                {
                    command.CommandText = @"
                        INSERT INTO dbo.POA_Purchaser
                        (
                            Code,
                            ExtCode,
                            Name,
                            EmailAddress,
                            Telephone,
                            Inactive,
                            LastUpdated,
                            HQId,
                            SyncGuid
                        )
                        VALUES
                        (
                            @Code,
                            @ExtCode,
                            @Name,
                            @EmailAddress,
                            @Telephone,
                            @Inactive,
                            GETDATE(),
                            NULL,
                            NEWID()
                        );

                        SELECT CAST(SCOPE_IDENTITY() AS INT);";
                }

                object scope = command.ExecuteScalar();
                return new Respuesta("", "Casa comercial guardada correctamente.", scope, true);
            }
        }

        private bool ProcedureExists(string procedureName)
        {
            using (var connection = new SqlConnection(con.CadenaConexion))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = @"
                    SELECT COUNT(1)
                    FROM sys.objects
                    WHERE type = 'P' AND name = @ProcedureName";

                command.Parameters.AddWithValue("@ProcedureName", procedureName);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        private static bool IsMissingProcedure(SqlException ex)
        {
            return ex != null &&
                   (ex.Number == 2812 ||
                    ex.Message.IndexOf("Could not find stored procedure", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static string SafeValue(string value, int maxLength)
        {
            string cleanValue = (value ?? String.Empty).Trim();
            return cleanValue.Length > maxLength ? cleanValue.Substring(0, maxLength) : cleanValue;
        }

        private static EN_Purchaser MakePurchaser(SqlDataReader dataReader)
        {
            return new EN_Purchaser
            {
                ID = dataReader.IsDBNull(dataReader.GetOrdinal("ID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ID")),
                Code = dataReader.IsDBNull(dataReader.GetOrdinal("Code")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Code")),
                ExtCode = dataReader.IsDBNull(dataReader.GetOrdinal("ExtCode")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("ExtCode")),
                Name = dataReader.IsDBNull(dataReader.GetOrdinal("Name")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Name")),
                EmailAddress = dataReader.IsDBNull(dataReader.GetOrdinal("EmailAddress")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("EmailAddress")),
                Telephone = dataReader.IsDBNull(dataReader.GetOrdinal("Telephone")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Telephone")),
                Inactive = !dataReader.IsDBNull(dataReader.GetOrdinal("Inactive")) && dataReader.GetBoolean(dataReader.GetOrdinal("Inactive")),
                LastUpdated = dataReader.IsDBNull(dataReader.GetOrdinal("LastUpdated")) ? DateTime.MinValue : dataReader.GetDateTime(dataReader.GetOrdinal("LastUpdated"))
            };
        }
    }
}
