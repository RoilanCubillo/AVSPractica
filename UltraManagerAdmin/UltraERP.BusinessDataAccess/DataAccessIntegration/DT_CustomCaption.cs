using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_CustomCaption : DT
    {
        #region Constructors
        public DT_CustomCaption() : base() { }
        #endregion

        #region Methods
        public List<EN_CustomCaption> GetAll()
        {
            return(from p in db.UEP_CUSTOMCAPTION_GETALL("", true, 0)
                   select new EN_CustomCaption()
                   {
                       ID = p.ID,
                       HQID = p.HQID,
                       Style = p.Style,
                       Caption = p.Caption,
                       SyncGuid = p.SyncGuid.Value
                   }).ToList();
        }
        #endregion
    }
}
