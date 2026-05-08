using System;

namespace UltraERP.BusinessEntities
{
	public class EN_Category
	{
		#region Fields

		private int hQID;
		private int iD;
		private int departmentID;
		private string name;
		private string code;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the EN_Category class.
		/// </summary>
		public EN_Category()
		{
		}

		/// <summary>
		/// Initializes a new instance of the EN_Category class.
		/// </summary>
		public EN_Category(int hQID, int departmentID, string name, string code)
		{
			this.hQID = hQID;
			this.departmentID = departmentID;
			this.name = name;
			this.code = code;
		}

		/// <summary>
		/// Initializes a new instance of the EN_Category class.
		/// </summary>
		public EN_Category(int hQID, int iD, int departmentID, string name, string code)
		{
			this.hQID = hQID;
			this.iD = iD;
			this.departmentID = departmentID;
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
		/// Gets or sets the DepartmentID value.
		/// </summary>
		public virtual int DepartmentID
		{
			get { return departmentID; }
			set { departmentID = value; }
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

		#endregion
	}
}
