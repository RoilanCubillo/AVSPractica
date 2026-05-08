using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
	public class CT_ItemMessage
	{
		#region Variables
		EN_ItemMessage oEN_ItemMessage = new EN_ItemMessage();
		DT_ItemMessage oDT_ItemMessage = new DT_ItemMessage();
		#endregion

		#region Constructors
		public CT_ItemMessage()
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Selects a single record from the ItemMessage table.
		/// </summary>
		public Respuesta Get(int iD)
		{
			return oDT_ItemMessage.Get(iD);
		}

		/// <summary>
		/// Actualizar datos en la Entidad EN_ItemMessage
		/// </summary>
		public Respuesta Save(EN_ItemMessage itemmessage)
		{
			return oDT_ItemMessage.Save(itemmessage);
		}

		/// <summary>
		/// Selects all records from the ItemMessage table.
		/// </summary>
		public Respuesta GetAll(string busqueda = "", int estado = 0, int cantidad = 0)
		{
			return oDT_ItemMessage.GetAll(busqueda, estado, cantidad);
		}
		#endregion
	}
}
