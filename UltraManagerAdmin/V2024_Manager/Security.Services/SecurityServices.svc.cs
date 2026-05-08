using Security.Entities;
using Security.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using static Security.Entities.SC_Enum;

namespace Security.WebServices
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "SecurityServices" en el código, en svc y en el archivo de configuración a la vez.
    // NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione SecurityServices.svc o SecurityServices.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class SecurityServices : ISecurityServices
    {
        #region User
        public (EN_SC_User, int, string) User_ValidateUser(string account, int autoID, int systemID)
        {
            return new CT_SC_User().ValidateUser(account, autoID, systemID);
        }

        public bool User_ValidateUserSession(int iD, int autoID, int systemID)
        {
            return new CT_SC_User().validateUserSession(iD, autoID, systemID);
        }
        #endregion

        #region System
        public (ESystemValidateStatus, bool) System_ValidateSystem(int userID, string systemCode)
        {
            return new CT_SC_System().ValidateSystem(userID, systemCode);
        }
        #endregion

        #region SC_Module
        public EN_SC_Module[] Module_ValidateModules(int userID, string systemCode)
        {
            return new CT_SC_Module().ValidateModules(userID, systemCode).ToArray();
        }
        #endregion

        #region SC_View
        public List<EN_SC_View> View_ValidateViews(int userID, string systemCode)
        {
            return new CT_SC_View().ValidateViews(userID, systemCode);
        }
        #endregion

        #region SC_DataAccess
        public List<EN_SC_DataAccess> DataAccess_ValidateDataAccess(int userID, string systemCode, string dataCode)
        {
            return new CT_SC_DataAccess().ValidateDataAccess(userID, systemCode, dataCode);
        }
        #endregion
    }
}
