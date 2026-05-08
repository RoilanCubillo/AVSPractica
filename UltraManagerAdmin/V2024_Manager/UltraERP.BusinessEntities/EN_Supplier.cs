using System;


namespace UltraERP.BusinessEntities
{
    public class EN_Supplier
    {

		#region Fields

			
		private int iD;
		private string name;
		private string code;

		#endregion


		#region Constructors

		public EN_Supplier()
		{

		}

		public EN_Supplier(int id, string name, string code)
		{

			this.iD = id;
			this.name = name;
			this.code = code;
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
