using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalArs.Domain.Entities
{
    public class Role
    {
        public int ID_Role { get; set; }
        public string Name { get; set; } = string.Empty; // Admin o User

        // Relación 1:N con User
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
