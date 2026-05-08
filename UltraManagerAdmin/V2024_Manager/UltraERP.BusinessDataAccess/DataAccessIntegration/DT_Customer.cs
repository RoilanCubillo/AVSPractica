using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_Customer : DT
    {
        #region Constructors
        public DT_Customer() : base() { }
        #endregion

        #region Methods
        public List<EN_Customer> GetAll()
        {
            return(from p in db.UEP_CUSTOMERACCOUNTGROUP_GETALL("", true, 0)
                   select new EN_Customer()
                   {
                       AccountNumber = p.AccountNumber,
                       AccountTypeID = p.AccountTypeID,
                       Address2 = p.Address2,
                       AssessFinanceCharges = p.AssessFinanceCharges,
                       Company = p.Company,
                       Country = p.Country,
                       /*CustomDate1 = p.CustomDate1.HasValue ? p.CustomDate1.Value : DateTime.MinValue,
                       CustomDate2 = p.CustomDate2.HasValue ? p.CustomDate2.Value : DateTime.MinValue,
                       CustomDate3 = p.CustomDate3.HasValue ? p.CustomDate3.Value : DateTime.MinValue,
                       CustomDate4 = p.CustomDate4.HasValue ? p.CustomDate4.Value : DateTime.MinValue,
                       CustomDate5 = p.CustomDate5.HasValue ? p.CustomDate5.Value : DateTime.MinValue,*/
                       CustomNumber1 = (float)p.CustomNumber1,
                       CustomNumber2 = (float)p.CustomNumber2,
                       CustomNumber3 = (float)p.CustomNumber3,
                       CustomNumber4 = (float)p.CustomNumber4,
                       CustomNumber5 = (float)p.CustomNumber5,
                       CustomText1 = p.CustomText1,
                       CustomText2 = p.CustomText2,
                       CustomText3 = p.CustomText3,
                       CustomText4 = p.CustomText4,
                       CustomText5 = p.CustomText5,
                       GlobalCustomer = p.GlobalCustomer,
                       HQID = p.HQID,/*
                       LastStartingDate = p.LastStartingDate.Value,
                       LastClosingDate = p.LastClosingDate.Value,*/
                       LastUpdated = p.LastUpdated,
                       LimitPurchase = p.LimitPurchase,
                       LastClosingBalance = p.LastClosingBalance,
                       PrimaryShipToID = p.PrimaryShipToID,
                       State = p.State,
                       StoreID = p.StoreID,
                       ID = p.ID,
                       LayawayCustomer = p.LayawayCustomer,
                       Employee = p.Employee,
                       FirstName = p.FirstName,
                       LastName = p.LastName,
                       Address = p.Address,
                       City = p.City,
                       Zip = p.Zip,
                       AccountBalance = p.AccountBalance,
                       CreditLimit = p.CreditLimit,
                       TotalSales = p.TotalSales,
                       AccountOpened = p.AccountOpened,
                       LastVisit = p.LastVisit,
                       TotalVisits = p.TotalVisits,
                       TotalSavings = p.TotalSavings,
                       CurrentDiscount = p.CurrentDiscount,
                       PriceLevel = p.PriceLevel,
                       TaxExempt = p.TaxExempt,
                       Notes = p.Notes,
                       Title = p.Title,
                       EmailAddress = p.EmailAddress,
                       TaxNumber = p.TaxNumber,
                       PictureName = p.PictureName,
                       DefaultShippingServiceID = p.DefaultShippingServiceID,
                       AutoID = p.AutoID,
                       PhoneNumber = p.PhoneNumber,
                       FaxNumber = p.FaxNumber,
                       CashierID = p.CashierID,
                       SalesRepID = p.SalesRepID,
                       Vouchers = p.Vouchers,
                       SyncGuid = p.SyncGuid.Value
                   }).ToList();
        }
        public Respuesta SaveCustomer(string prefijo, int accountGroupID, EN_Customer customer)
        {
            Respuesta respuesta = new Respuesta("Proedimiento no ejecutado", "No se pudo guardar el cliente", null, false);
            try
            {
                respuesta = (from i in db.UEP_CUSTOMER_INSERT_UPDATE(prefijo,accountGroupID,customer.AccountNumber,customer.AccountTypeID,
                    customer.Address2,customer.AssessFinanceCharges, customer.Company, customer.Country, customer.CustomDate1, customer.CustomDate2,
                    customer.CustomDate3, customer.CustomDate4, customer.CustomDate5, customer.CustomNumber1, customer.CustomNumber2, customer.CustomNumber3,
                    customer.CustomNumber4, customer.CustomNumber5, customer.CustomText1, customer.CustomText2, customer.CustomText3, customer.CustomText4,
                    customer.CustomText5, customer.GlobalCustomer, customer.HQID, customer.LastStartingDate, customer.LastClosingDate, customer.LastUpdated,
                    customer.LimitPurchase, customer.LastClosingBalance, customer.PrimaryShipToID, customer.State, customer.StoreID, customer.ID,
                    customer.LayawayCustomer, customer.Employee, customer.FirstName, customer.LastName, customer.Address, customer.City, customer.Zip,
                    customer.AccountBalance, customer.CreditLimit, customer.TotalSales, customer.AccountOpened, customer.LastVisit, customer.TotalVisits,
                    customer.TotalSavings, customer.CurrentDiscount, customer.PriceLevel, customer.TaxExempt, customer.Notes, customer.Title, customer.EmailAddress,
                    customer.TaxNumber, customer.PictureName, customer.DefaultShippingServiceID, customer.PhoneNumber, customer.FaxNumber, customer.CashierID,
                    customer.SalesRepID, customer.Vouchers)
                             select new Respuesta()
                             {
                                 InternalMessage = "Procedimiento Exitoso",
                                 Message = "Se guardo un nuevo cliente",
                                 Result = i.RESPUESTA,
                                 Status = i.RESPUESTA != ""
                             }).Single();
            }
            catch (Exception e)
            {
                respuesta.InternalMessage = e.Message;
                Console.WriteLine("Error en SaveCustomer: " + e.Message);
            }
            return respuesta;

        }
        #endregion
    }
}
