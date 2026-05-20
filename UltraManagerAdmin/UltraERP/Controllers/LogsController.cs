using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using UltraERP.Models;
using UltraERP.Services;

namespace UltraERP.Controllers
{
    [Authorize]
    public class LogsController : Controller
    {
        public ActionResult Inicio(string search, int? pageSize)
        {
            ViewBag.Title = "Logs";
            ViewBag.ActiveMenu = "Logs";
            ViewBag.Search = search ?? "";
            ViewBag.PageSize = NormalizePageSize(pageSize);

            var model = new List<LogEntryViewModel>();

            try
            {
                model = GetLogs(search, Convert.ToInt32(ViewBag.PageSize));
            }
            catch (Exception ex)
            {
                ErrorLogService.Log(ex, "Logs", "Consultar bitacora", new { search, pageSize });
                ViewBag.LogError = "No se pudo consultar la bitacora. Revise la conexion con SQL o la tabla UEP_ERROR_LOG.";
            }

            return View(model);
        }

        private static List<LogEntryViewModel> GetLogs(string search, int take)
        {
            var logs = new List<LogEntryViewModel>();
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["UltraERP.BusinessDataAccess.Properties.Settings.MasterDB"];
            if (settings == null || String.IsNullOrWhiteSpace(settings.ConnectionString))
                throw new InvalidOperationException("No se encontro la conexion MasterDB.");

            using (var connection = new SqlConnection(BuildFastFailConnectionString(settings.ConnectionString)))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 8;
                command.CommandText = @"
IF OBJECT_ID('dbo.UEP_ERROR_LOG', 'U') IS NULL
BEGIN
    SELECT TOP (0)
        CAST(0 AS INT) AS ID,
        GETDATE() AS CreatedAt,
        CAST(0 AS INT) AS UserID,
        CAST('' AS NVARCHAR(150)) AS UserName,
        CAST('' AS NVARCHAR(80)) AS UserAccount,
        CAST('' AS NVARCHAR(120)) AS Screen,
        CAST('' AS NVARCHAR(120)) AS ActionName,
        CAST('' AS NVARCHAR(4000)) AS ErrorMessage,
        CAST('' AS NVARCHAR(MAX)) AS ErrorDetail,
        CAST('' AS NVARCHAR(MAX)) AS RequestData,
        CAST('' AS NVARCHAR(500)) AS Url;
END
ELSE
BEGIN
    SELECT TOP (@Take)
        ID,
        CreatedAt,
        UserID,
        UserName,
        UserAccount,
        Screen,
        ActionName,
        ErrorMessage,
        ErrorDetail,
        RequestData,
        Url
    FROM dbo.UEP_ERROR_LOG
    WHERE
        @Search = ''
        OR ISNULL(UserName, '') LIKE @Pattern
        OR ISNULL(UserAccount, '') LIKE @Pattern
        OR ISNULL(Screen, '') LIKE @Pattern
        OR ISNULL(ActionName, '') LIKE @Pattern
        OR ISNULL(ErrorMessage, '') LIKE @Pattern
        OR ISNULL(Url, '') LIKE @Pattern
    ORDER BY CreatedAt DESC, ID DESC;
END";

                string cleanSearch = (search ?? "").Trim();
                command.Parameters.Add("@Take", SqlDbType.Int).Value = take;
                command.Parameters.Add("@Search", SqlDbType.NVarChar, 120).Value = cleanSearch;
                command.Parameters.Add("@Pattern", SqlDbType.NVarChar, 260).Value = "%" + cleanSearch + "%";

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logs.Add(new LogEntryViewModel
                        {
                            ID = ReadInt(reader, "ID"),
                            CreatedAt = ReadDate(reader, "CreatedAt"),
                            UserID = ReadInt(reader, "UserID"),
                            UserName = ReadString(reader, "UserName"),
                            UserAccount = ReadString(reader, "UserAccount"),
                            Screen = ReadString(reader, "Screen"),
                            ActionName = ReadString(reader, "ActionName"),
                            ErrorMessage = ReadString(reader, "ErrorMessage"),
                            ErrorDetail = ReadString(reader, "ErrorDetail"),
                            RequestData = ReadString(reader, "RequestData"),
                            Url = ReadString(reader, "Url")
                        });
                    }
                }
            }

            return logs;
        }

        private static int NormalizePageSize(int? pageSize)
        {
            int value = pageSize.GetValueOrDefault(50);
            if (value == 10 || value == 20 || value == 50 || value == 100 || value == 200)
                return value;

            return 50;
        }

        private static string BuildFastFailConnectionString(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                ConnectTimeout = 8
            };

            return builder.ConnectionString;
        }

        private static string ReadString(IDataRecord record, string field)
        {
            object value = record[field];
            return value == DBNull.Value ? "" : Convert.ToString(value);
        }

        private static int ReadInt(IDataRecord record, string field)
        {
            object value = record[field];
            return value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        private static DateTime ReadDate(IDataRecord record, string field)
        {
            object value = record[field];
            return value == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(value);
        }
    }
}
