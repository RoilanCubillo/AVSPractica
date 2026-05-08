using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_AR_PaymentTerms : DT
    {
        #region Constructors
        public DT_AR_PaymentTerms() : base() { }
        #endregion

        #region Methods
       public  List<EN_AR_PaymentTerms> GetAll()
       {
            return (from p in db.UEP_AR_PaymentTerms_GETALL("", true, 0)
             select new EN_AR_PaymentTerms()
             {
                 ID = p.ID,
                 Code = p.Code,
                 Name = p.Name,
                 ExtCode = p.ExtCode,
                 Inactive = p.Inactive,
                 MLText = p.MLText,
                 DateDueType = p.DateDueType,
                 GracePeriod = p.GracePeriod,
                 MinimunPayment = p.MinimumPayment,
                 SyncGuid = p.SyncGuid.Value
             }).ToList();
       }
       public Respuesta SavePaymentTerms (EN_AR_PaymentTerms paymentTerms)
       {
            Respuesta respuesta = new Respuesta("Proedimiento no ejecutado", "No se pudo guardar el metodo de pago", null, false);
            try
            {
                respuesta = (from i in db.UEP_AR_PAYMENTTERMS_INSERT_UPDATE(paymentTerms.Code, paymentTerms.Name, paymentTerms.Inactive, (byte)paymentTerms.DateDueType, paymentTerms.GracePeriod, paymentTerms.MinimunPayment)
                             select new Respuesta()
                             {
                                 InternalMessage = "Procedimiento Exitoso",
                                 Message = "Se guardo un nuevo metodo de pago",
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
