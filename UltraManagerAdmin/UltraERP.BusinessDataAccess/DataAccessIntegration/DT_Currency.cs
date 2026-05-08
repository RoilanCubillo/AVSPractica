using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessDataAccess.DataAccessIntegration
{
	public class DT_Currency : DT
	{
		#region Variables

		protected EN_Currency oEN_Currency = new EN_Currency();

		#endregion

		#region Constructors

		public DT_Currency() : base() { }

		#endregion

		#region Methods

		/// <summary>
		/// Insertar filas dentro de la tablaCurrency
		/// </summary>
		public virtual string Insert(EN_Currency currency)
		{
			SqlCommand comComando = new SqlCommand();
			comComando.Connection = cn;

			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@HQID", (currency.HQID==0)?Convert.DBNull:currency.HQID),
				new SqlParameter("@ExchangeRate", (currency.ExchangeRate==0)?Convert.DBNull:currency.ExchangeRate),
				new SqlParameter("@Description", (currency.Description==String.Empty)?Convert.DBNull:currency.Description),
				new SqlParameter("@Code", (currency.Code==String.Empty)?Convert.DBNull:currency.Code),
				new SqlParameter("@LocaleID", (currency.LocaleID==0)?Convert.DBNull:currency.LocaleID),
				new SqlParameter("@DBTimeStamp", (currency.DBTimeStamp==new DateTime(1900,01,01))?Convert.DBNull:currency.DBTimeStamp),
				new SqlParameter("@SyncGuid", (currency.SyncGuid==Guid.Empty)?Convert.DBNull:currency.SyncGuid)
			};

			try
			{
			int CodigoIdentity = -1;
			currency.ID = (int) SqlHelper.ExecuteScalar(cn, CommandType.StoredProcedure, "sp_CurrencyInsert", parameters);
			CodigoIdentity = currency.ID;
			return "Operacion Exitosa " + CodigoIdentity;
			}
			catch (Exception e)
			{
			return "Error: "+e;
			}
		}

		/// <summary>
		/// Actualizar filas en Currency table.
		/// </summary>
		public virtual string Update(EN_Currency currency)
		{
			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@ID", (currency.ID==0)?Convert.DBNull:currency.ID),
				new SqlParameter("@HQID", (currency.HQID==0)?Convert.DBNull:currency.HQID),
				new SqlParameter("@ExchangeRate", (currency.ExchangeRate==0)?Convert.DBNull:currency.ExchangeRate),
				new SqlParameter("@Description", (currency.Description==String.Empty)?Convert.DBNull:currency.Description),
				new SqlParameter("@Code", (currency.Code==String.Empty)?Convert.DBNull:currency.Code),
				new SqlParameter("@LocaleID", (currency.LocaleID==0)?Convert.DBNull:currency.LocaleID),
				new SqlParameter("@DBTimeStamp", (currency.DBTimeStamp==new DateTime(1900,01,01))?Convert.DBNull:currency.DBTimeStamp),
				new SqlParameter("@SyncGuid", (currency.SyncGuid==Guid.Empty)?Convert.DBNull:currency.SyncGuid)
			};

			try
			{
			SqlHelper.ExecuteNonQuery(cn, CommandType.StoredProcedure, "sp_CurrencyUpdate", parameters);
			return "Operacion Exitosa";
			}
			catch (Exception e)
			{
			return "Error: "+e;
			}
		}

		/// <summary>
		/// Elimina filas desde Currency table by its primary key.
		/// </summary>
		public virtual string Delete(int iD)
		{
			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@ID", iD)
			};

			try
			{
			SqlHelper.ExecuteNonQuery(cn, CommandType.StoredProcedure, "sp_CurrencyDelete", parameters);
			return "Operacion Exitosa";
			}
			catch (Exception e)
			{
			return "Error: "+e;
			}
		}

		/// <summary>
		/// Seleccionar una fila desde Currency table.
		/// </summary>
		public virtual EN_Currency Select(int iD)
		{
			SqlParameter[] parameters = new SqlParameter[]
			{
				new SqlParameter("@ID", iD)
			};

			using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "sp_CurrencySelect", parameters))
			{
				if (dataReader.Read())
				{
					return MakeEN_Currency(dataReader);
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Selects all records from the Currency table.
		/// </summary>
		public DataSet SelectAll()
		{
			SqlCommand cmd = new SqlCommand("sp_CurrencySelectAll",cn);
			cmd.CommandType = CommandType.StoredProcedure;
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);
				return ds;
			}

		/// <summary>
		/// Selects all records from the Currency table.
		/// </summary>
		public virtual List<EN_Currency> SelectAllList()
		{
			using (SqlDataReader dataReader = SqlHelper.ExecuteReader(cn, CommandType.StoredProcedure, "sp_CurrencySelectAll"))
			{
				List<EN_Currency> currencyList = new List<EN_Currency>();
				while (dataReader.Read())
				{
					EN_Currency currency = MakeEN_Currency(dataReader);
					currencyList.Add(currency);
				}

				return currencyList;
			}
		}

		/// <summary>
		/// Creates a new instance of the Currency class and populates it with data from the specified SqlDataReader.
		/// </summary>
		protected virtual EN_Currency MakeEN_Currency(SqlDataReader dataReader)
		{
			EN_Currency oeN_Currency = new EN_Currency();
			oeN_Currency.ID =dataReader.IsDBNull(dataReader.GetOrdinal("ID"))? 0 : dataReader.GetInt32(dataReader.GetOrdinal("ID"));
			oeN_Currency.HQID =dataReader.IsDBNull(dataReader.GetOrdinal("HQID"))? 0 : dataReader.GetInt32(dataReader.GetOrdinal("HQID"));
			oeN_Currency.ExchangeRate =dataReader.IsDBNull(dataReader.GetOrdinal("ExchangeRate"))? 0.00 : dataReader.GetDouble(dataReader.GetOrdinal("ExchangeRate"));
			oeN_Currency.Description =dataReader.IsDBNull(dataReader.GetOrdinal("Description"))? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Description"));
			oeN_Currency.Code =dataReader.IsDBNull(dataReader.GetOrdinal("Code"))? String.Empty : dataReader.GetString(dataReader.GetOrdinal("Code"));
			oeN_Currency.LocaleID =dataReader.IsDBNull(dataReader.GetOrdinal("LocaleID"))? 0 : dataReader.GetInt32(dataReader.GetOrdinal("LocaleID"));
			//oeN_Currency.DBTimeStamp =dataReader.IsDBNull(dataReader.GetOrdinal("DBTimeStamp"))? new DateTime(1900,01,01) : dataReader.GetDateTime(dataReader.GetOrdinal("DBTimeStamp"));
			//oeN_Currency.SyncGuid =dataReader.IsDBNull(dataReader.GetOrdinal("SyncGuid"))? Guid.Empty : dataReader.GetGuid(dataReader.GetOrdinal("SyncGuid"));

			return oeN_Currency;
		}

		#endregion
	}
}
