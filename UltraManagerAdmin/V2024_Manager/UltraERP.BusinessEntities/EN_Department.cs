using System;

namespace UltraERP.BusinessEntities
{
	public class EN_Department
	{
		#region Fields

		private int hQID;
		private int iD;
		private string name;
		private string code;
		private int familyID;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the EN_Department class.
		/// </summary>
		public EN_Department()
		{
		}

		/// <summary>
		/// Initializes a new instance of the EN_Department class.
		/// </summary>
		public EN_Department(int hQID, string name, string code)
		{
			this.hQID = hQID;
			this.name = name;
			this.code = code;
		}

		/// <summary>
		/// Initializes a new instance of the EN_Department class.
		/// </summary>
		public EN_Department(int hQID, int iD, string name, string code)
		{
			this.hQID = hQID;
			this.iD = iD;
			this.name = name;
			this.code = code;
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
		/// Gets or sets the Name value.
		/// </summary>
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// Gets or sets the Code value.
		/// </summary>
		public virtual string Code
		{
			get { return code; }
			set { code = value; }
		}

		public int FamilyID
        {
            get { return familyID; }
            set { familyID = value; }
        }

		public string FamilyCode { get; set; }
		public string FamilyName { get; set; }
		#endregion
	}
}
