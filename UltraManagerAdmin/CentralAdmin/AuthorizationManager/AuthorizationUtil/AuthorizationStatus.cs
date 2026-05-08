using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Security.EntitiesAVS.SC_Enum;

namespace CentralAdmin.AuthorizationManager.AuthorizationUtil
{
    public class AuthorizationStatus
    {
        private static AuthorizationStatus SINGLETON = null;

        private bool lastStatus;
        private ActionResult result;
        private ESystemValidateStatus systemValidateStatus;

        private AuthorizationStatus()
        {
            lastStatus = false;
            systemValidateStatus = 0;
        }

        public static AuthorizationStatus Instance
        {
            get
            {
                if (SINGLETON == null)
                    SINGLETON = new AuthorizationStatus();

                return SINGLETON;
            }
        }

        public void Set(bool lastStatus, ActionResult result)
        {
            this.result = result;
            this.lastStatus = lastStatus;
        }

        public void Set(bool lastStatus, ActionResult result, ESystemValidateStatus systemValidateStatus)
        {
            this.result = result;
            this.lastStatus = lastStatus;
            this.systemValidateStatus = systemValidateStatus;
        }

        public void Clear()
        {
            lastStatus = false;
            result = null;
            systemValidateStatus = 0;
        }

        public bool LastStatus
        {
            get { return lastStatus; }
            set { lastStatus = value; }
        }

        public ActionResult Result
        {
            get { return result; }
            set { result = value; }
        }

        public ESystemValidateStatus SystemValidateStatus
        {
            get { return systemValidateStatus; }
            set { systemValidateStatus = value; }
        }
    }
}