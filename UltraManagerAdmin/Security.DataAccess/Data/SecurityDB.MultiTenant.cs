using System.Collections.Generic;
using System.Data.Linq.Mapping;

namespace Security.DataAccess.Data
{
    public partial class SecurityDBDataContext
    {
        [Function(Name = "dbo.SC_COMPANY_GET_BY_USER")]
        public IEnumerable<SC_COMPANY_GET_BY_USERResult> SC_COMPANY_GET_BY_USER(
            [Parameter(Name = "Account", DbType = "NVarChar(100)")] string account,
            [Parameter(Name = "AutoID", DbType = "Int")] int? autoID,
            [Parameter(Name = "SystemID", DbType = "Int")] int? systemID)
        {
            return ExecuteQuery<SC_COMPANY_GET_BY_USERResult>(
                "EXEC [dbo].[SC_COMPANY_GET_BY_USER] {0}, {1}, {2}",
                account,
                autoID,
                systemID);
        }

        [Function(Name = "dbo.SC_USER_VALIDATE_COMPANY")]
        public IEnumerable<SC_USER_VALIDATE_COMPANYResult> SC_USER_VALIDATE_COMPANY(
            [Parameter(Name = "Account", DbType = "NVarChar(100)")] string account,
            [Parameter(Name = "AutoID", DbType = "Int")] int? autoID,
            [Parameter(Name = "SystemID", DbType = "Int")] int? systemID,
            [Parameter(Name = "ID_Company", DbType = "Int")] int? idCompany)
        {
            return ExecuteQuery<SC_USER_VALIDATE_COMPANYResult>(
                "EXEC [dbo].[SC_USER_VALIDATE_COMPANY] {0}, {1}, {2}, {3}",
                account,
                autoID,
                systemID,
                idCompany);
        }

        [Function(Name = "dbo.SC_USER_VALIDATE_SESSION_COMPANY")]
        public IEnumerable<SC_USER_VALIDATE_SESSION_COMPANYResult> SC_USER_VALIDATE_SESSION_COMPANY(
            [Parameter(Name = "ID", DbType = "Int")] int? id,
            [Parameter(Name = "AutoID", DbType = "Int")] int? autoID,
            [Parameter(Name = "SystemID", DbType = "Int")] int? systemID,
            [Parameter(Name = "ID_Company", DbType = "Int")] int? idCompany)
        {
            return ExecuteQuery<SC_USER_VALIDATE_SESSION_COMPANYResult>(
                "EXEC [dbo].[SC_USER_VALIDATE_SESSION_COMPANY] {0}, {1}, {2}, {3}",
                id,
                autoID,
                systemID,
                idCompany);
        }
    }

    public class SC_COMPANY_GET_BY_USERResult
    {
        [Column(DbType = "Int NOT NULL")]
        public int ID_Company { get; set; }

        [Column(DbType = "VarChar(30)")]
        public string Code { get; set; }

        [Column(DbType = "VarChar(100)")]
        public string Name { get; set; }

        [Column(DbType = "VarChar(MAX)")]
        public string Connection_String { get; set; }

        [Column(DbType = "Bit NOT NULL")]
        public bool Enable { get; set; }
    }

    public class SC_USER_VALIDATE_COMPANYResult
    {
        [Column(DbType = "Int NOT NULL")]
        public int ID { get; set; }

        [Column(DbType = "Int NOT NULL")]
        public int AutoID { get; set; }

        [Column(DbType = "Int NOT NULL")]
        public int SystemID { get; set; }

        [Column(DbType = "VarChar(50) NOT NULL")]
        public string Account { get; set; }

        [Column(DbType = "VarChar(100)")]
        public string Name { get; set; }

        [Column(DbType = "Bit NOT NULL")]
        public bool EnCloseSession { get; set; }

        [Column(DbType = "Bit NOT NULL")]
        public bool Enable { get; set; }

        [Column(DbType = "Int NOT NULL")]
        public int ID_Company { get; set; }
    }

    public class SC_USER_VALIDATE_SESSION_COMPANYResult
    {
        [Column(DbType = "Int NOT NULL")]
        public int ID { get; set; }
    }
}
