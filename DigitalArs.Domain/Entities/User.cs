using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalArs.Domain.Entities
{
    public class User
    {
        public int ID_User { get; set; }
        public string Full_Name { get; set; } = string.Empty;
        public string Email { get; set; } 
        public string Password_Hasheada { get; set; }
        public string DNI { get; set; }
        public string Alias { get; set; }
        
        //Relacion con Role
        public int ID_Role { get; set; }
        public Role Role { get; set; }

        //Relacion 1:1 con Account
        public Account Account { get; set; }
    }
}
