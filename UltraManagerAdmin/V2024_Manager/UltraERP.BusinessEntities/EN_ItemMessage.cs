using System;

namespace UltraERP.BusinessEntities
{
	public class EN_ItemMessage
	{
		#region Fields
		private int hQID;
		private int iD;
		private string title;
		private short ageLimit;
		private string message;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the EN_ItemMessage class.
		/// </summary>
		public EN_ItemMessage()
		{
		}

		/// <summary>
		/// Initializes a new instance of the EN_ItemMessage class.
		/// </summary>
		public EN_ItemMessage(int hQID, string title, short ageLimit, string message)
		{
			this.hQID = hQID;
			this.title = title;
			this.ageLimit = ageLimit;
			this.message = message;
		}

		/// <summary>
		/// Initializes a new instance of the EN_ItemMessage class.
		/// </summary>
		public EN_ItemMessage(int hQID, int iD, string title, short ageLimit, string message)
		{
			this.hQID = hQID;
			this.iD = iD;
			this.title = title;
			this.ageLimit = ageLimit;
			this.message = message;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the HQID value.
		/// </summary>
		public virtual int HQID
		{
			get { return hQID; }
			set { hQID = value; }
		}

		/// <summary>
		/// Gets or sets the ID value.
		/// </summary>
		public virtual int ID
		{
			get { return iD; }
			set { iD = value; }
		}

		/// <summary>
		/// Gets or sets the Title value.
		/// </summary>
		public virtual string Title
		{
			get { return title; }
			set { title = value; }
		}

		/// <summary>
		/// Gets or sets the AgeLimit value.
		/// </summary>
		public virtual short AgeLimit
		{
			get { return ageLimit; }
			set { ageLimit = value; }
		}

		/// <summary>
		/// Gets or sets the Message value.
		/// </summary>
		public virtual string Message
		{
			get { return message; }
			set { message = value; }
		}
		#endregion
	}
}
