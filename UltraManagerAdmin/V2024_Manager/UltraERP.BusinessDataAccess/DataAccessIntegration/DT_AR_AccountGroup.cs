using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_AR_AccountGroup : DT
    {
        #region Constructors
        public DT_AR_AccountGroup(): base() { }
        #endregion

        #region Methods
        public List<EN_AR_AccountGroup> GetAll()
        {
            return(from p in db.UEP_AR_ACCOUNTGROUP_GETALL("", true, 0)
                   select new EN_AR_AccountGroup() {
                       ID = p.ID,
                       Code = p.Code,
                       Name = p.Name,
                       Inactive = p.Inactive,
                       FinChargeID = p.FinChargeID,
                       PayTermsId = p.PayTermsID,
                       ManagerId = p.ManagerID,
                       NumberSeries = p.NumberSeries.Value,
                       CreditLimit = p.CreditLimit,
                       StatementType = p.StatementType,
                       CreditLimitCheck = p.CreditLimitCheck,
                       ApplicationMethod = p.ApplicationMethod

                   }).ToList();
        }
        public Respuesta SaveAccountGroup(EN_AR_AccountGroup accountGroup, EN_ExtCentral_AR_AccountGroup extCentralAccountGroup)
        {
            Respuesta respuesta = new Respuesta("Proedimiento no ejecutado", "No se pudo guardar el grupo de cuentas", null, false);
            try
            {
               respuesta = (from i in db.UEP_AR_ACCOUNTGROUP_INSERT_UPDATE(accountGroup.ID, accountGroup.Code, accountGroup.Name, accountGroup.Inactive,
                    accountGroup.PayTermsId, accountGroup.FinChargeID, accountGroup.CurrencyId, accountGroup.ManagerId, accountGroup.CountryId, accountGroup.RegionId,
                    accountGroup.LocationId, accountGroup.CreditLimit, accountGroup.CreditLimitCheck, accountGroup.StatementType, accountGroup.ApplicationMethod,
                    accountGroup.NumberSeries, extCentralAccountGroup.StoreID, extCentralAccountGroup.Prefix)
                            select new Respuesta()
                            {
                                InternalMessage = "Procedimiento Exitoso",
                                Message = "Se guardo un nuevo grupo de cuentas",
                                Result = i.RESPUESTA,
                                Status = i.RESPUESTA != ""
                            }
                ).Single();

            }
            catch(Exception e)
            {
                respuesta.InternalMessage = e.Message;
            }
            return respuesta;
        } 
        #endregion
    }
    
}
