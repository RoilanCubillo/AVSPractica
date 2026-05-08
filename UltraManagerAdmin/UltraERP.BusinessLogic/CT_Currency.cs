using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
	public class CT_Currency
	{
		#region Variables

		EN_Currency oEN_Currency = new EN_Currency();
		DT_Currency oDT_Currency = new DT_Currency();

		#endregion

		#region Constructors

		public CT_Currency()
		{
		}

		#endregion

		#region Methods

		/// <summary>
		/// Almacenar datos en la Entidad EN_Currency
		/// </summary>
		public string Insert(EN_Currency currency)
		{
			string resultado = oDT_Currency.Insert(currency);
			if (resultado.Contains("Error")) return resultado;
			else
			{
				return resultado;
			}
		}

		/// <summary>
		/// Selects a single record from the Currency table.
		/// </summary>
		public EN_Currency Select(int iD)
		{
			return oDT_Currency.Select(iD);
		}

		/// <summary>
		/// Actualizar datos en la Entidad EN_Currency
		/// </summary>
		public string Update(EN_Currency currency)
		{
			string resultado = oDT_Currency.Update(currency);
			if (resultado.Contains("Error")) return resultado;
			else
			{
				return resultado;
			}
		}

		/// <summary>
		/// Deletes a record from the Currency table by its primary key.
		/// </summary>
		public string Delete(int iD)
		{
			string resultado = oDT_Currency.Delete(iD);
			if (resultado.Contains("Error")) return resultado;
			else
			{
				return resultado;
			}
		}

		/// <summary>
		/// Selects all records from the Currency table.
		/// </summary>
		public DataSet CurrencySelectAll()
		{
			return oDT_Currency.SelectAll();
		}

		/// <summary>
		/// Selects all records from the Currency table.
		/// </summary>
		public List<EN_Currency> SelectAllList()
		{
			return oDT_Currency.SelectAllList();
		}

#region En desarrollo. . .
/*
		/// <summary>
		/// Creates a new instance of the Currency class and populates it with data from the specified SqlDataReader.
		/// </summary>
		protected EN_Currency MakeCurrency()
		{
			Currency currency = new Currency();
			return currency;
		}

*/
#endregion

		#endregion
	}
}
