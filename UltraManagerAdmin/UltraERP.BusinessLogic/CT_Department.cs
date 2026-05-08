using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
	public class CT_Department
	{
		#region Variables

		EN_Department oEN_Department = new EN_Department();
		DT_Department oDT_Department = new DT_Department();

		#endregion

		#region Constructors

		public CT_Department()
		{
		}

		#endregion

		#region Methods

		/// <summary>
		/// Selects a single record from the Department table.
		/// </summary>
		public EN_Department Get(int iD)
		{
			return oDT_Department.Get(iD);
		}

		/// <summary>
		/// Actualizar datos en la Entidad EN_Department
		/// </summary>
		public Respuesta Save(EN_Department department)
		{
			return oDT_Department.Save(department);
		}

		/// <summary>
		/// Selects all records from the Department table.
		/// </summary>
		public List<EN_Department> GetAll(string busqueda = "", int estado = 0, int cantidad = 0)
		{
			return oDT_Department.GetAll(busqueda, estado, cantidad);
		}

		#endregion
	}
}
