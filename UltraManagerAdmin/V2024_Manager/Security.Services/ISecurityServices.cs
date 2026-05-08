using Security.Entities;
using System.Collections.Generic;
using System.ServiceModel;
using static Security.Entities.SC_Enum;

namespace Security.WebServices
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "ISecurityServices" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface ISecurityServices
    {
        #region SC_User
        [OperationContract]
        (EN_SC_User, int, string) User_ValidateUser(string account, int autoID, int systemID);
        [OperationContract]
        bool User_ValidateUserSession(int iD, int autoID, int systemID);
        #endregion

        #region SC_System
        [OperationContract]
        (ESystemValidateStatus, bool) System_ValidateSystem(int userID, string systemCode);
        #endregion

        #region SC_Module
        [OperationContract]
        EN_SC_Module[] Module_ValidateModules(int userID, string systemCode);
        #endregion

        #region SC_View
        [OperationContract]
        List<EN_SC_View> View_ValidateViews(int userID, string systemCode);
        #endregion

        #region DataAccess
        [OperationContract]
        List<EN_SC_DataAccess> DataAccess_ValidateDataAccess(int userID, string systemCode, string dataCode);
        #endregion
    }
}
