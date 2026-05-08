using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_RR_NumberSeries : DT
    {
        #region Variables
        protected EN_RR_NumberSeries oEN_RR_NumberSeries = new EN_RR_NumberSeries();
        #endregion
        #region Constructors
        public DT_RR_NumberSeries() : base() { }
        #endregion
        #region Methods
        public List<EN_RR_NumberSeries> GetAll()
        {
            return(from p in db.UEP_RR_NUMBERSERIES_GETALL("", true, 0)
                   select new EN_RR_NumberSeries()
                   {
                       ID = p.ID,
                       Code = p.Code,
                       Name = p.Name,
                       Inactive = p.Inactive,
                       Prefix = p.Prefix,
                       NoOfDigit = p.NoOfDigit,
                       LastUsed = p.LastUsed
                   }).ToList();
        }
        #endregion
    }
}
