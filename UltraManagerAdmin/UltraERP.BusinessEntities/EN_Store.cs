using System;



namespace UltraERP.BusinessEntities
{
	public class EN_Store
	{
		#region Fields


		private int idS;
		private string nameS;
		private string codeS;
		#endregion


		#region Constructors

		public EN_Store()
		{

		}

		public EN_Store(int idS, string nameS, string codeS)
		{

			this.idS = idS;
			this.nameS = nameS;
			this.codeS = codeS;
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the ID value.
		/// </summary>
		public virtual int IDS
		{
			get { return idS; }
			set { idS = value; }
		}



		/// <summary>
		/// Gets or sets the Name value.
		/// </summary>
		public virtual string NameS
		{
			get { return nameS; }
			set { nameS = value; }
		}

		/// <summary>
		/// Gets or sets the Code value.
		/// </summary>
		public virtual string CodeS
		{
			get { return codeS; }
			set { codeS = value; }
		}

		public bool ItemStatus { get; set; }

		#endregion
	}


}
