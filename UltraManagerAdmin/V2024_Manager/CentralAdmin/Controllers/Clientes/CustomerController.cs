using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.wcfClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UltraERP.BusinessEntities;

namespace CentralAdmin.Controllers.Clientes
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly UltraERPServiceClient _objService = new UltraERPServiceClient();

        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Clientes/Customer.cshtml");
        }
        public JsonResult GetAll()
        {
            try
            {
                using(_objService)
                {
                    List<EN_Customer> list_customers = _objService.Customers_GetAll().ToList();
                    List<EN_ExtCentral_AR_AccountGroup> listExtCentralAccountGroup = _objService.ExtCentral_AR_AccountGroup_GetAll().ToList();
                    List<EN_ExtCentral_Customer> listExtCentralCustomer = _objService.ExtCentral_Customer_GetAll().ToList();
                    List<EN_CustomCaption> listCustomCaption = _objService.CustomCaption_GetAll().ToList();
                    List<EN_AR_Account> listAccounts = _objService.AR_Account_GetAll().ToList();
                    List<EN_AR_CustomerBalance> listCustomerBalance = _objService.AR_CustomerBalance_GetAll().ToList();
                    List<EN_AR_AccountBalance> listAccountBalance = _objService.AR_AccountBalance_GetAll().ToList();
                    List<EN_AR_AccountGroup> listAccountGroups = _objService.AR_AccountGroup_GetAll().ToList();
                    List<EN_AR_AccountManager> listAccountManager = _objService.AR_AccountManager_GetAll().ToList();
                    List<EN_AR_PaymentTerms> listPaymentTerms = _objService.AR_PaymentTerms_GetAll().ToList();
                    List<EN_AR_FinanceCharge> listFinanceCharge = _objService.AR_FinanceCharge_GetAll().ToList();
                    List<EN_AR_StatementType> listStatementType = _objService.AR_StatementType_GetAll().ToList();
                    List<EN_Store> listStores = _objService.GetAllStore("%",0,0).ToList();
                    List<EN_ExtCentral_AR_AccountDynamic> listAccountDynamic = _objService.ExtCentral_AR_AccountDynamic_GetAll().ToList();
                    
                    return Json(new Respuesta("", "",
                        new
                        {
                            list_customers = list_customers,
                            listExtCentralAccountGroup = listExtCentralAccountGroup,
                            listExtCentralCustomer = listExtCentralCustomer,
                            listCustomCaption = listCustomCaption,
                            listAccounts = listAccounts,
                            listCustomerBalance = listCustomerBalance,
                            listAccountBalance = listAccountBalance,
                            listAccountGroups = listAccountGroups,
                            listAccountManager = listAccountManager,
                            listPaymentTerms = listPaymentTerms,
                            listFinanceCharge = listFinanceCharge,
                            listStatementType = listStatementType,
                            listStores = listStores,
                            listAccountDynamic = listAccountDynamic

                        },
                        true), JsonRequestBehavior.AllowGet);
                }

            }catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAccountStoreBalance(int accountID, int groupID)
        {
            try
            {
                using (_objService)
                {
                    List<EN_ExtCentral_AR_AccountDynamic> AccountStoreBalance = _objService.ExtCentral_AR_AccountDynamic_GetStoreBalance(accountID, groupID).ToList();
                    return Json(new Respuesta("", "",
                        new
                        {
                            AccountStoreBalance = AccountStoreBalance

                        },
                        true), JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save(string prefijo, int accountGroupID, EN_Customer customer)
        {
            try
            {
                Respuesta r = _objService.EN_CUSTOMER_SAVE(prefijo,accountGroupID,customer);
                r.Message = Resources.messages.ResourceManager.GetString(r.Message);
                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.ToString(), Resources.messages.error_interno + Resources.messages.error_intento_guardar, null, false), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SaveAccount(EN_AR_Account account, EN_ExtCentral_AR_AccountDynamic accountDynamic, EN_AR_AccountLink accountLink)
        {
            try
            {
                Respuesta r = _objService.EN_AR_ACCOUNT_SAVE(account, accountDynamic, accountLink);
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