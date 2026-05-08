using Security.EntitiesAVS;
using System.Collections.Generic;
using System.ServiceModel;

namespace Security.WebServices
{
    [ServiceContract]
    public interface ISecurityServices
    {
        [OperationContract]
        (EN_SC_User, int, string) User_ValidateUser(string account, int autoID, int systemID);

        [OperationContract]
        bool User_ValidateUserSession(int iD, int autoID, int systemID);

        [OperationContract]
        List<EN_SC_Company> User_GetCompanies(string account, int autoID, int systemID);

        [OperationContract]
        (EN_SC_User, int, string) User_ValidateUserCompany(string account, int autoID, int systemID, int idCompany);

        [OperationContract]
        bool User_ValidateUserSessionCompany(int iD, int autoID, int systemID, int idCompany);

        [OperationContract]
        (SC_Enum.ESystemValidateStatus, bool) System_ValidateSystem(int userID, string systemCode);

        [OperationContract]
        EN_SC_Module[] Module_ValidateModules(int userID, string systemCode);

        [OperationContract]
        List<EN_SC_View> View_ValidateViews(int userID, string systemCode);

        [OperationContract]
        List<EN_SC_DataAccess> DataAccess_ValidateDataAccess(int userID, string systemCode, string dataCode);
    }
}
