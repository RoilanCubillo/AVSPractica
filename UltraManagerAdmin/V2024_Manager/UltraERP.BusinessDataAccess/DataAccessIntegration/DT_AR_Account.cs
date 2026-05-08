using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_AR_Account : DT
    {
        #region Constructors
        public DT_AR_Account() : base() { }
        #endregion

        #region Methods
        public List<EN_AR_Account> GetAll()
        {
            return(from p in db.UEP_AR_ACCOUNT_GETALL("", true, 0)
                   select new EN_AR_Account()
                   {
                       ID = p.ID,
                       LastUpdated = p.LastUpdated,
                       Number = p.Number,
                       Name = p.Name,
                       ExtCode = p.ExtCode,
                       Title = p.Title,
                       FirstName = p.FirstName,
                       MiddleName = p.MiddleName,
                       LastName = p.LastName,
                       Suffix = p.Suffix,
                       Company = p.Company,
                       JobTitle = p.JobTitle,
                       Type = p.Type,
                       Role = p.Role,
                       Status = p.Status,
                       BillingAccount = p.BillingAccount,
                       GroupID = p.GroupID,
                       PayTermsID = p.PayTermsID,
                       FinChargeID = p.FinChargeID,
                       CurrencyID = p.CurrencyID,
                       ManagerID = p.ManagerID,
                       CountryID = p.CountryID,
                       RegionID = p.RegionID,
                       LocationID = p.LocationID,
                       Address1 = p.Address1,
                       Address2 = p.Address2,
                       City = p.City,
                       State = p.State,
                       Zip = p.Zip,
                       Country = p.Country,
                       PhoneNumber = p.PhoneNumber,
                       MovileNumber = p.MobileNumber,
                       FaxNumber = p.FaxNumber,
                       EMail = p.EMail,
                       HomePage = p.HomePage,
                       IMAddress = p.IMAddress,
                       CreditLimit = p.CreditLimit,
                       CreditLimitCheck = p.CreditLimitCheck,
                       StatementType = p.StatementType,
                       StatementDelivery = p.StatementDelivery,
                       StatementAddress = p.StatementAddress,
                       ApplicationMethod = p.ApplicationMethod,
                       ClosingDate = p.ClosingDate,
                       ClosingBalance = p.ClosingBalance,
                       LastStatement = p.LastStatement,
                       CustomCode1 = p.CustomCode1,
                       CustomCode2 = p.CustomCode2,
                       CustomCode3 = p.CustomCode3,
                       CustomCode4 = p.CustomCode4,
                       CustomCode5 = p.CustomCode5,
                       CustomText1 = p.CustomText1,
                       CustomText2 = p.CustomText2,
                       CustomText3 = p.CustomText3,
                       CustomText4 = p.CustomText4,
                       CustomText5 = p.CustomText5,
                       CustomNumber1 = (float)p.CustomNumber1,
                       CustomNumber2 = (float)p.CustomNumber2,
                       CustomNumber3 = (float)p.CustomNumber3,
                       CustomNumber4 = (float)p.CustomNumber4,
                       CustomNumber5 = (float)p.CustomNumber5,
                       CustomDate1 = p.CustomDate1,
                       CustomDate2 = p.CustomDate2,
                       CustomDate3 = p.CustomDate3,
                       CustomDate4 = p.CustomDate4,
                       CustomDate5 = p.CustomDate5,
                       Notes = p.Notes,
                       ExtData = p.ExtData,
                       DateOpened = p.DateOpened,
                       LastActivity = p.LastActivity,
                       StoreID = p.StoreID,
                       SyncGuid = p.SyncGuid.Value
                   }).ToList();
        }
        public Respuesta SaveAccount(EN_AR_Account account, EN_ExtCentral_AR_AccountDynamic accountDynamic, EN_AR_AccountLink accountLink)
        {
            Respuesta respuesta = new Respuesta("Proedimiento no ejecutado", "No se pudo guardar la cuenta", null, false);
            try
            {
                respuesta = (from i in db.UEP_AR_ACCOUNT_INSERT_UPDATE(account.LastUpdated, account.Number, account.Name, account.ExtCode, account.Title,
                    account.FirstName, account.MiddleName, account.LastName, account.Suffix, account.Company, account.JobTitle, (byte)account.Type, (byte)account.Status,
                    account.BillingAccount, account.GroupID, account.PayTermsID, account.FinChargeID, account.CurrencyID, account.ManagerID, account.CountryID,
                    account.RegionID, account.LocationID, account.Address1, account.Address2, account.City, account.State, account.Zip, account.Country, account.PhoneNumber,
                    account.MovileNumber, account.FaxNumber, account.EMail, account.HomePage, account.IMAddress, account.CreditLimit, (byte)account.CreditLimitCheck,
                    account.StatementType, (byte)account.StatementDelivery, account.StatementAddress, (byte)account.ApplicationMethod, account.ClosingDate, account.ClosingBalance,
                    account.LastStatement, account.CustomCode1, account.CustomCode2, account.CustomCode3, account.CustomCode4, account.CustomCode5, account.CustomText1,
                    account.CustomText2, account.CustomText3, account.CustomText4, account.CustomText5, account.CustomNumber1, account.CustomNumber2, account.CustomNumber3,
                    account.CustomNumber4, account.CustomNumber5, account.CustomDate1, account.CustomDate2, account.CustomDate3, account.CustomDate4, account.CustomDate5,
                    account.Picture, account.Notes, account.ExtData, account.DateOpened, account.LastActivity, account.StoreID,accountDynamic.StoreID,
                    (byte)accountDynamic.Status, accountDynamic.CreditLimit, accountDynamic.LastUpdated, accountLink.LinkID)
                             select new Respuesta()
                             {
                                 InternalMessage = "Procedimiento Exitoso",
                                 Message = "Se guardo una nueva cuenta",
                                 Result = i.RESPUESTA,
                                 Status = i.RESPUESTA != ""
                             }
                ).Single();

            }
            catch (Exception e)
            {
                respuesta.InternalMessage = e.Message;
            }
            return respuesta;
        }
        #endregion
    }
}
