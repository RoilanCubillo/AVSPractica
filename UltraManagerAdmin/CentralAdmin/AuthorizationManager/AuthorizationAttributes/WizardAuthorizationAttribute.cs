using CentralAdmin.AuthorizationManager.AuthorizationUtil;
using Security.EntitiesAVS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace CentralAdmin.AuthorizationManager.AuthorizationAttributes
{
    public class WizardAuthorizationAttribute : ACustomAttribute
    {
        protected override void OnAuthorizationValidate(AuthorizationContext filterContext)
        {
            if (!VerifyAuthorization(filterContext.RequestContext.HttpContext.Session, Permissions))
            {
                filterContext.Result = AuthorizationView("Unauthorized");

                AuthorizationStatus.Instance.Clear();
                AuthorizationStatus.Instance.Set(true, filterContext.Result);
            }
        }

        public static bool VerifyAuthorization(HttpSessionStateBase session, string permissions)
        {
            EN_SC_DataAccess[] access = Array.FindAll(
                (EN_SC_DataAccess[])session["USER_DATAACCESS"],
                x => x.Code == WebConfigurationManager.AppSettings["DataWizardCode"]
            );

            if (Array.Exists(access, x => x.EnableAll)) return true;
            else
            {
                foreach (EN_SC_DataAccess i in access)
                {
                    if (Array.Exists(access, a => a.DataIDs != null && a.DataIDs.Split(',').Contains(permissions)))
                        return true;
                }

                return false;
            }
        }

        public static EN_SC_DataAccess[] GetDataAccessTasks(HttpSessionStateBase session)
        {
            EN_SC_DataAccess[] access = Array.FindAll(
                (EN_SC_DataAccess[])session["USER_DATAACCESS"],
                x => x.Code == WebConfigurationManager.AppSettings["DataWizardCode"]
            );

            return access;
        }

        public static string TasksAvailable(HttpSessionStateBase session)
        {
            EN_SC_DataAccess[] access = GetDataAccessTasks(session);

            if (Array.Exists(access, x => x.EnableAll))
                return "%";
            else
            {
                List<string> ids = new List<string>();

                foreach (EN_SC_DataAccess i in access)
                    if (!i.EnableAll && i.DataIDs != null)
                        ids.Add(i.DataIDs);

                return String.Join(",", ids);
            }
        }

        public static EN_SC_DataAccess[] GetDataAccessWorksheetUsers(HttpSessionStateBase session)
        {
            EN_SC_DataAccess[] access = Array.FindAll(
                (EN_SC_DataAccess[])session["USER_DATAACCESS"],
                x => x.Code == WebConfigurationManager.AppSettings["DataWorksheetUser"]
            );

            return access;
        }

        public static string WorkSheetUsersAvailable(HttpSessionStateBase session)
        {
            EN_SC_DataAccess[] access = GetDataAccessWorksheetUsers(session);

            if (Array.Exists(access, x => x.EnableAll))
                return "%";
            else
            {
                List<string> ids = new List<string>();

                foreach (EN_SC_DataAccess i in access)
                    if (!i.EnableAll && i.DataIDs != null)
                        ids.Add(i.DataIDs);

                return String.Join(",", ids);
            }
        }
    }
}