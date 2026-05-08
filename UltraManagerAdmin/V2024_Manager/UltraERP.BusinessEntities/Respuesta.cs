using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class Respuesta
    {
        #region Fields
        private string internalMessage;
        private string message;
        private object result;
        private bool status;
        #endregion

        #region Constructores
        public Respuesta()
        {
        }
        public Respuesta(string internalMessage, string message, object result, bool status)
        {
            this.internalMessage = internalMessage;
            this.message = message;
            this.result = result;
            this.status = status;
        }
        #endregion

        #region Properties
        public virtual string InternalMessage { 
            get { return internalMessage; }
            set { internalMessage = value; }
        }
        public virtual string Message
        {
            get { return message; }
            set { message = value; }
        }
        public virtual bool Status
        {
            get { return status; }
            set { status = value; }
        }
        public virtual object Result { 
            get { return result; }
            set { result = value; } 
        }
        #endregion
    }
}
