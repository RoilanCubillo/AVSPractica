using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using UltraERP.BusinessDataAccess.Data;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
    public class DT_QuantityDiscount : DT
    {
        #region Variables
        protected EN_QuantityDiscount oEN_QuantityDiscount = new EN_QuantityDiscount();
        #endregion

        #region Constructors

        public DT_QuantityDiscount() : base() { }

        #endregion

        #region Methods
        public virtual Respuesta Save(EN_QuantityDiscount quantityDiscount)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Description", (quantityDiscount.Description==String.Empty)?Convert.DBNull:quantityDiscount.Description),
                new SqlParameter("@HQID", quantityDiscount.HQID),
                new SqlParameter("@ID", quantityDiscount.ID),
                new SqlParameter("@DiscountOddItems", quantityDiscount.DiscountOddItems),
                new SqlParameter("@Quantity1", quantityDiscount.Quantity1),
                new SqlParameter("@Price1", quantityDiscount.Price1),
                new SqlParameter("@Price1A", quantityDiscount.Price1A),
                new SqlParameter("@Price1B", quantityDiscount.Price1B),
                new SqlParameter("@Price1C", quantityDiscount.Price1C),
                new SqlParameter("@Quantity2", quantityDiscount.Quantity2),
                new SqlParameter("@Price2", quantityDiscount.Price2),
                new SqlParameter("@Price2A", quantityDiscount.Price2A),
                new SqlParameter("@Price2B", quantityDiscount.Price2B),
                new SqlParameter("@Price2C", quantityDiscount.Price2C),
                new SqlParameter("@Quantity3", quantityDiscount.Quantity3),
                new SqlParameter("@Price3", quantityDiscount.Price3),
                new SqlParameter("@Price3A", quantityDiscount.Price3A),
                new SqlParameter("@Price3B", quantityDiscount.Price3B),
                new SqlParameter("@Price3C", quantityDiscount.Price3C),
                new SqlParameter("@Quantity4", quantityDiscount.Quantity4),
                new SqlParameter("@Price4", quantityDiscount.Price4),
                new SqlParameter("@Price4A", quantityDiscount.Price4A),
                new SqlParameter("@Price4B", quantityDiscount.Price4B),
                new SqlParameter("@Price4C", quantityDiscount.Price4C),
                new SqlParameter("@Type", quantityDiscount.Type),
                new SqlParameter("@PercentOffPrice1", quantityDiscount.PercentOffPrice1),
                new SqlParameter("@PercentOffPrice1A", quantityDiscount.PercentOffPrice1A),
                new SqlParameter("@PercentOffPrice1B", quantityDiscount.PercentOffPrice1B),
                new SqlParameter("@PercentOffPrice1C", quantityDiscount.PercentOffPrice1C),
                new SqlParameter("@PercentOffPrice2", quantityDiscount.PercentOffPrice2),
                new SqlParameter("@PercentOffPrice2A", quantityDiscount.PercentOffPrice2A),
                new SqlParameter("@PercentOffPrice2B", quantityDiscount.PercentOffPrice2B),
                new SqlParameter("@PercentOffPrice2C", quantityDiscount.PercentOffPrice2C),
                new SqlParameter("@PercentOffPrice3", quantityDiscount.PercentOffPrice3),
                new SqlParameter("@PercentOffPrice3A", quantityDiscount.PercentOffPrice3A),
                new SqlParameter("@PercentOffPrice3B", quantityDiscount.PercentOffPrice3B),
                new SqlParameter("@PercentOffPrice3C", quantityDiscount.PercentOffPrice3C),
                new SqlParameter("@PercentOffPrice4", quantityDiscount.PercentOffPrice4),
                new SqlParameter("@PercentOffPrice4A", quantityDiscount.PercentOffPrice4A),
                new SqlParameter("@PercentOffPrice4B", quantityDiscount.PercentOffPrice4B),
                new SqlParameter("@PercentOffPrice4C", quantityDiscount.PercentOffPrice4C)
            };

            try
            {
                SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_QUANTITYDISCOUNT_INSERT_UPDATE", parameters);
                if (dataReader.Read())
                {
                    string result = dataReader.IsDBNull(dataReader.GetOrdinal("RESPUESTA")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("RESPUESTA"));
                    int scope = dataReader.IsDBNull(dataReader.GetOrdinal("SCOPE")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("SCOPE"));

                    if (String.IsNullOrEmpty(result)) return new Respuesta("RESULTADO: VACIO", "error_get_proc", scope, false);
                    else if (result.Contains("ERROR")) return new Respuesta("ERROR AL ACCIONAR EL PROCEDIMIENTO", result, scope, false);
                    return new Respuesta("", result, scope, true);
                }
                return new Respuesta("RESULTADO: NO LEIDO", "error_leer_proc", null, false);
            }
            catch (Exception e)
            {
                return new Respuesta(e.Message, "error_guardar", null, false);
            }
        }

        public virtual EN_QuantityDiscount Get(int iD)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ID", iD)
            };

            try
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_QUANTITYDISCOUNT_GET", parameters))
                {
                    if (dataReader.Read())
                    {
                        return MakeEN_QuantityDiscount(dataReader);
                    }

                    return null;
                }
            }
            catch
            {
                List<EN_QuantityDiscount> list = GetAll(0, 0);
                return list == null ? null : list.Find(x => x.ID == iD);
            }
        }

        public virtual List<EN_QuantityDiscount> GetAll(int estado = 0, int cantidad = 0)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Estado", estado),
                    new SqlParameter("@ResultCount", cantidad),
                };
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_QUANTITYDISCOUNT_GETALL", parameters))
                {
                    List<EN_QuantityDiscount> quantityDiscountList = new List<EN_QuantityDiscount>();
                    while (dataReader.Read())
                    {
                        EN_QuantityDiscount quantityDiscount = MakeEN_QuantityDiscount(dataReader);
                        quantityDiscountList.Add(quantityDiscount);
                    }

                    return quantityDiscountList;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<EN_QuantityDiscount> GetAllSimpleByType(int type)
        {
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@TYPE", type) };

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "UEP_QUANTITYDISCOUNT_GETALL_SIMPLE_BY_TYPE", parameters))
            {
                List<EN_QuantityDiscount> quantityDiscountList = new List<EN_QuantityDiscount>();
                while (dataReader.Read())
                {
                    quantityDiscountList.Add(new EN_QuantityDiscount()
                    {
                        ID = dataReader.IsDBNull(dataReader.GetOrdinal("ID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ID")),
                        Description = dataReader.IsDBNull(dataReader.GetOrdinal("Description")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Description"))
                    });
                }

                return quantityDiscountList;
            }
        }

        protected virtual EN_QuantityDiscount MakeEN_QuantityDiscount(SqlDataReader dataReader)
        {
            EN_QuantityDiscount oeN_QuantityDiscount = new EN_QuantityDiscount();
            oeN_QuantityDiscount.Description = dataReader.IsDBNull(dataReader.GetOrdinal("Description")) ? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Description"));
            oeN_QuantityDiscount.HQID = dataReader.IsDBNull(dataReader.GetOrdinal("HQID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("HQID"));
            oeN_QuantityDiscount.ID = dataReader.IsDBNull(dataReader.GetOrdinal("ID")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ID"));
            oeN_QuantityDiscount.DiscountOddItems = dataReader.IsDBNull(dataReader.GetOrdinal("DiscountOddItems")) ? false : dataReader.GetBoolean(dataReader.GetOrdinal("DiscountOddItems"));
            oeN_QuantityDiscount.Quantity1 = dataReader.IsDBNull(dataReader.GetOrdinal("Quantity1")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("Quantity1")));
            oeN_QuantityDiscount.Price1 = dataReader.IsDBNull(dataReader.GetOrdinal("Price1")) ? 0 : dataReader.GetDecimal(dataReader.GetOrdinal("Price1"));
            oeN_QuantityDiscount.Price1A = dataReader.IsDBNull(dataReader.GetOrdinal("Price1A")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Price1A"));
            oeN_QuantityDiscount.Price1B = dataReader.IsDBNull(dataReader.GetOrdinal("Price1B")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Price1B"));
            oeN_QuantityDiscount.Price1C = dataReader.IsDBNull(dataReader.GetOrdinal("Price1C")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Price1C"));
            oeN_QuantityDiscount.Quantity2 = dataReader.IsDBNull(dataReader.GetOrdinal("Quantity2")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("Quantity2")));
            oeN_QuantityDiscount.Price2 = dataReader.IsDBNull(dataReader.GetOrdinal("Price2")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Price2"));
            oeN_QuantityDiscount.Price2A = dataReader.IsDBNull(dataReader.GetOrdinal("Price2A")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Price2A"));
            oeN_QuantityDiscount.Price2B = dataReader.IsDBNull(dataReader.GetOrdinal("Price2B")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Price2B"));
            oeN_QuantityDiscount.Price2C = dataReader.IsDBNull(dataReader.GetOrdinal("Price2C")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Price2C"));
            oeN_QuantityDiscount.Quantity3 = dataReader.IsDBNull(dataReader.GetOrdinal("Quantity3")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("Quantity3")));
            oeN_QuantityDiscount.Price3 = dataReader.IsDBNull(dataReader.GetOrdinal("Price3")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Price3"));
            oeN_QuantityDiscount.Price3A = dataReader.IsDBNull(dataReader.GetOrdinal("Price3A")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Price3A"));
            oeN_QuantityDiscount.Price3B = dataReader.IsDBNull(dataReader.GetOrdinal("Price3B")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Price3B"));
            oeN_QuantityDiscount.Price3C = dataReader.IsDBNull(dataReader.GetOrdinal("Price3C")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Price3C"));
            oeN_QuantityDiscount.Quantity4 = dataReader.IsDBNull(dataReader.GetOrdinal("Quantity4")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("Quantity4")));
            oeN_QuantityDiscount.Price4 = dataReader.IsDBNull(dataReader.GetOrdinal("Price4")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Price4"));
            oeN_QuantityDiscount.Price4A = dataReader.IsDBNull(dataReader.GetOrdinal("Price4A")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Price4A"));
            oeN_QuantityDiscount.Price4B = dataReader.IsDBNull(dataReader.GetOrdinal("Price4B")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Price4B"));
            oeN_QuantityDiscount.Price4C = dataReader.IsDBNull(dataReader.GetOrdinal("Price4C")) ? Decimal.Zero : dataReader.GetDecimal(dataReader.GetOrdinal("Price4C"));
            oeN_QuantityDiscount.Type = dataReader.IsDBNull(dataReader.GetOrdinal("Type")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("Type"));
            oeN_QuantityDiscount.PercentOffPrice1 = dataReader.IsDBNull(dataReader.GetOrdinal("PercentOffPrice1")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("PercentOffPrice1")));
            oeN_QuantityDiscount.PercentOffPrice1A = dataReader.IsDBNull(dataReader.GetOrdinal("PercentOffPrice1A")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("PercentOffPrice1A")));
            oeN_QuantityDiscount.PercentOffPrice1B = dataReader.IsDBNull(dataReader.GetOrdinal("PercentOffPrice1B")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("PercentOffPrice1B")));
            oeN_QuantityDiscount.PercentOffPrice1C = dataReader.IsDBNull(dataReader.GetOrdinal("PercentOffPrice1C")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("PercentOffPrice1C")));
            oeN_QuantityDiscount.PercentOffPrice2 = dataReader.IsDBNull(dataReader.GetOrdinal("PercentOffPrice2")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("PercentOffPrice2")));
            oeN_QuantityDiscount.PercentOffPrice2A = dataReader.IsDBNull(dataReader.GetOrdinal("PercentOffPrice2A")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("PercentOffPrice2A")));
            oeN_QuantityDiscount.PercentOffPrice2B = dataReader.IsDBNull(dataReader.GetOrdinal("PercentOffPrice2B")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("PercentOffPrice2B")));
            oeN_QuantityDiscount.PercentOffPrice2C = dataReader.IsDBNull(dataReader.GetOrdinal("PercentOffPrice2C")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("PercentOffPrice2C")));
            oeN_QuantityDiscount.PercentOffPrice3 = dataReader.IsDBNull(dataReader.GetOrdinal("PercentOffPrice3")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("PercentOffPrice3")));
            oeN_QuantityDiscount.PercentOffPrice3A = dataReader.IsDBNull(dataReader.GetOrdinal("PercentOffPrice3A")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("PercentOffPrice3A")));
            oeN_QuantityDiscount.PercentOffPrice3B = dataReader.IsDBNull(dataReader.GetOrdinal("PercentOffPrice3B")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("PercentOffPrice3B")));
            oeN_QuantityDiscount.PercentOffPrice3C = dataReader.IsDBNull(dataReader.GetOrdinal("PercentOffPrice3C")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("PercentOffPrice3C")));
            oeN_QuantityDiscount.PercentOffPrice4 = dataReader.IsDBNull(dataReader.GetOrdinal("PercentOffPrice4")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("PercentOffPrice4")));
            oeN_QuantityDiscount.PercentOffPrice4A = dataReader.IsDBNull(dataReader.GetOrdinal("PercentOffPrice4A")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("PercentOffPrice4A")));
            oeN_QuantityDiscount.PercentOffPrice4B = dataReader.IsDBNull(dataReader.GetOrdinal("PercentOffPrice4B")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("PercentOffPrice4B")));
            oeN_QuantityDiscount.PercentOffPrice4C = dataReader.IsDBNull(dataReader.GetOrdinal("PercentOffPrice4C")) ? 0.0F : ((float)dataReader.GetDouble(dataReader.GetOrdinal("PercentOffPrice4C")));
            return oeN_QuantityDiscount;
        }
        #endregion
    }
}
