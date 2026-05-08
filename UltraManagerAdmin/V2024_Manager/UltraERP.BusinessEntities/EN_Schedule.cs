using System;

namespace UltraERP.BusinessEntities
{
	public class EN_Schedule
	{
		#region Fields
		private int iD;
		private int hQID;
		private string description;
		private int increment;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the EN_Schedule class.
		/// </summary>
		public EN_Schedule()
		{
		}

		/// <summary>
		/// Initializes a new instance of the EN_Schedule class.
		/// </summary>
		public EN_Schedule(int hQID, string description, int increment)
		{
			this.hQID = hQID;
			this.description = description;
			this.increment = increment;
		}

		/// <summary>
		/// Initializes a new instance of the EN_Schedule class.
		/// </summary>
		public EN_Schedule(int iD, int hQID, string description, int increment)
		{
			this.iD = iD;
			this.hQID = hQID;
			this.description = description;
			this.increment = increment;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the ID value.
		/// </summary>
		public virtual int ID
		{
			get { return iD; }
			set { iD = value; }
		}

		/// <summary>
		/// Gets or sets the HQID value.
		/// </summary>
		public virtual int HQID
		{
			get { return hQID; }
			set { hQID = value; }
		}

		/// <summary>
		/// Gets or sets the Description value.
		/// </summary>
		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}

		/// <summary>
		/// Gets or sets the Increment value.
		/// </summary>
		public virtual int Increment
		{
			get { return increment; }
			set { increment = value; }
		}
		#endregion
	}
}
