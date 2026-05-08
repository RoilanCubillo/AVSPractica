using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_QuantityDiscount
    {
        #region Variables
        EN_QuantityDiscount oEN_QuantityDiscount = new EN_QuantityDiscount();
        DT_QuantityDiscount oDT_QuantityDiscount = new DT_QuantityDiscount();
        #endregion

        #region Constructors
        public CT_QuantityDiscount() { }
        #endregion

        #region Methods
        public EN_QuantityDiscount Get(int iD)
        {
            return oDT_QuantityDiscount.Get(iD);
        }

        public Respuesta Save(EN_QuantityDiscount quantitydiscount)
        {
            return oDT_QuantityDiscount.Save(quantitydiscount);
        }

        public List<EN_QuantityDiscount> GetAll(int estado = 0, int cantidad = 0)
        {
            return oDT_QuantityDiscount.GetAll(estado, cantidad);
        }

        public List<EN_QuantityDiscount> GetAllSimpleByType(int type)
        {
            return oDT_QuantityDiscount.GetAllSimpleByType(type);
        }
        #endregion
    }
}
