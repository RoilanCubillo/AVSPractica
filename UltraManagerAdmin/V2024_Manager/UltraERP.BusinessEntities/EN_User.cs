using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    //public class EN_User
    //{
    //    public int ID { get; set; }
    //    public string Account { get; set; }
    //    public string Password { get; set; }
    //    public string Name { get; set; }
    //    public string EmailAddress { get; set; }
    //    public int UserPrivileges { get; set; }
    //    public int SecurityLevel { get; set; }
    //}

    [DataContract]
    public class EN_User
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Account { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string EmailAddress { get; set; }

        [DataMember]
        public int UserPrivileges { get; set; }

        [DataMember]
        public int SecurityLevel { get; set; }
    }

    [DataContract]
    public class EN_User_ValidateUserResponse
    {
        [DataMember]
        public EN_User User{ get; set; }

        [DataMember]
        public int Status { get; set; }
    }
}
