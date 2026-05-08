using System;

namespace UltraERP.BusinessEntities
{
	public class EN_Currency
	{
		#region Fields

		private int iD;
		private int hQID;
		private double exchangeRate;
		private string description;
		private string code;
		private int localeID;
		private DateTime dBTimeStamp;
		private Guid syncGuid;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the EN_Currency class.
		/// </summary>
		public EN_Currency()
		{
		}

		/// <summary>
		/// Initializes a new instance of the EN_Currency class.
		/// </summary>
		public EN_Currency(int hQID, double exchangeRate, string description, string code, int localeID, DateTime dBTimeStamp, Guid syncGuid)
		{
			this.hQID = hQID;
			this.exchangeRate = exchangeRate;
			this.description = description;
			this.code = code;
			this.localeID = localeID;
			this.dBTimeStamp = dBTimeStamp;
			this.syncGuid = syncGuid;
		}

		/// <summary>
		/// Initializes a new instance of the EN_Currency class.
		/// </summary>
		public EN_Currency(int iD, int hQID, double exchangeRate, string description, string code, int localeID, DateTime dBTimeStamp, Guid syncGuid)
		{
			this.iD = iD;
			this.hQID = hQID;
			this.exchangeRate = exchangeRate;
			this.description = description;
			this.code = code;
			this.localeID = localeID;
			this.dBTimeStamp = dBTimeStamp;
			this.syncGuid = syncGuid;
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
		/// Gets or sets the ExchangeRate value.
		/// </summary>
		public virtual double ExchangeRate
		{
			get { return exchangeRate; }
			set { exchangeRate = value; }
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
		/// Gets or sets the Code value.
		/// </summary>
		public virtual string Code
		{
			get { return code; }
			set { code = value; }
		}

		/// <summary>
		/// Gets or sets the LocaleID value.
		/// </summary>
		public virtual int LocaleID
		{
			get { return localeID; }
			set { localeID = value; }
		}

		/// <summary>
		/// Gets or sets the DBTimeStamp value.
		/// </summary>
		public virtual DateTime DBTimeStamp
		{
			get { return dBTimeStamp; }
			set { dBTimeStamp = value; }
		}

		/// <summary>
		/// Gets or sets the SyncGuid value.
		/// </summary>
		public virtual Guid SyncGuid
		{
			get { return syncGuid; }
			set { syncGuid = value; }
		}

		#endregion
	}
}
