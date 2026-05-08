using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
	public class CT_Schedule
	{
		#region Variables
		DT_Schedule oDT_Schedule = new DT_Schedule();
		#endregion

		#region Constructors
		public CT_Schedule()
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Almacenar datos en la Entidad EN_Schedule
		/// </summary>
		public Respuesta Save(EN_Schedule schedule, List<EN_ScheduleSegment> segments)
		{
			return oDT_Schedule.Save(schedule, segments);
		}

		/// <summary>
		/// Selects a single record from the Schedule table.
		/// </summary>
		public Respuesta Get(int iD)
		{
			return oDT_Schedule.Get(iD);
		}

		/// <summary>
		/// Selects all records from the Schedule table.
		/// </summary>
		public List<EN_Schedule> GetAll(string busqueda, int estado, int cantidad)
		{
			return oDT_Schedule.GetAll(busqueda, estado, cantidad);
		}
		#endregion
	}
}
