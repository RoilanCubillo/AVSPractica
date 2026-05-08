using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_AR_StatementType : DT
    {
        #region Variables
        protected EN_AR_StatementType oEN_AR_StatementTyp = new EN_AR_StatementType();
        #endregion
        #region Constructors
        public DT_AR_StatementType() : base() { }
        #endregion
        #region Methods
        public List<EN_AR_StatementType> GetAll()
        {
            return(from p in db.UEP_AR_STATEMENTTYPE_GETALL("", true, 0)
                   select new EN_AR_StatementType()
                   {
                       ID = p.ID,
                       Code = p.Code,
                       Name = p.Name,
                       ExtCode = p.ExtCode,
                       Inactive = p.Inactive,
                       MLText = p.MLText,
                       DataSet = p.DataSet,
                       Template = p.Template,
                       Options = p.Options
                   }).ToList();
        }
        #endregion
    }
}
