using CentralAdmin.wcfClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UltraERP.BusinessEntities;

namespace CentralAdmin.Controllers.Clientes
{
    public class ARAccountGroupController : Controller
    {
        private readonly UltraERPServiceClient _objService = new UltraERPServiceClient();
        // GET: ARAccountGroup
        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Clientes/ARAccountGroup.cshtml");
        }

        public JsonResult GetAll()
        {
            try
            {
                using (_objService)
                {
                    List<EN_AR_PaymentTerms> listPaymentTerms = _objService.AR_PaymentTerms_GetAll().ToList();
                    List<EN_AR_FinanceCharge> listFinanceCharge = _objService.AR_FinanceCharge_GetAll().ToList();
                    List<EN_AR_AccountManager> listAccountManager = _objService.AR_AccountManager_GetAll().ToList();
                    List<EN_AR_StatementType> listStatementType = _objService.AR_StatementType_GetAll().ToList();
                    List<EN_Store> listStore = _objService.GetAllStore("1,2,3", 0, 0).ToList();
                    List<EN_RR_NumberSeries> listRRNumberSeries = _objService.RR_NumberSeries_GetAll().ToList();
                    List<EN_ExtCentral_AR_AccountGroup> listExtCentralAccountGroup = _objService.ExtCentral_AR_AccountGroup_GetAll().ToList();
                    List<EN_AR_AccountGroup> list = _objService.AR_AccountGroup_GetAll().ToList();
                    var combinedList = (from a in list
                                        join e in listExtCentralAccountGroup
                                        on a.ID equals e.AccountGroupID
                                        select new
                                        {
                                            a.ID,
                                            a.Code,
                                            a.Name,
                                            a.ExtCode,
                                            a.Inactive,
                                            a.PayTermsId,
                                            a.FinChargeID,
                                            a.CurrencyId,
                                            a.ManagerId,
                                            a.CreditLimit,
                                            a.CreditLimitCheck,
                                            a.StatementType,
                                            a.ApplicationMethod,
                                            a.NumberSeries,
                                            a.SyncGuid,
                                            e.StoreID,
                                            e.Prefix
                                        }).ToList();
                    return Json(new Respuesta("", "",
                        new { list = list, listPaymentTerms = listPaymentTerms, listFinanceCharge = listFinanceCharge, 
                            listAccountManager = listAccountManager, listStatementType = listStatementType, listStore = listStore, listExtCentralAccountGroup = listExtCentralAccountGroup,
                            combinedList = combinedList,
                            listRRNumberSeries = listRRNumberSeries},
                        true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Save(EN_AR_AccountGroup accountGroup, EN_ExtCentral_AR_AccountGroup extCentral_AR_AccountGroup)
        {
            try
            {
                Respuesta r = _objService.EN_AR_ACCOUNTGROUP_SAVE(accountGroup, extCentral_AR_AccountGroup);
                r.Message = Resources.messages.ResourceManager.GetString(r.Message);
                return Json(r, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.ToString(), Resources.messages.error_interno + Resources.messages.error_intento_guardar, null, false), JsonRequestBehavior.AllowGet);
            }
        }
       

    }
}