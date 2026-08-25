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
        public string Roles { get; set; }
    }
}
