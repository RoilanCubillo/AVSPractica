namespace Security.EntitiesAVS
{
   public class EN_SC_User
    {
        public int ID { get; set; }
        public int AutoID { get; set; }
        public int SystemID { get; set; }
        public int ID_Company { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public bool EnableCloseSession { get; set; }
        public bool Enable { get; set; }
    }
}
