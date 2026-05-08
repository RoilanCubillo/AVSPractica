using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
	public class CT_ScheduleSegment
	{
		#region Variables
		DT_ScheduleSegment oDT_ScheduleSegment = new DT_ScheduleSegment();
		#endregion

		#region Constructors
		public CT_ScheduleSegment()
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Selects all records from the ScheduleSegment table.
		/// </summary>
		public List<EN_ScheduleSegment> GetAll()
		{
			return oDT_ScheduleSegment.GetAll();
		}

		/// <summary>
		/// Selects all records from the ScheduleSegment table.
		/// </summary>
		public List<EN_ScheduleSegment> GetByScheduleID(int scheduleID)
		{
			return oDT_ScheduleSegment.GetByScheduleID(scheduleID);
		}

		#endregion
	}
}
