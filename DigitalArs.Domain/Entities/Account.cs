using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DigitalArs.Domain.Entities
{
    public class Account
    {
        public int ID_Account { get; set; }
        
        //Relacion con User
        public int ID_User { get; set; } //relacion con user mediante el id
        public User User { get; set; } //relacion con user mediante el objeto
        
        public string Name { get; set; } = string.Empty; // nombre de la cuenta o lo traemos del user?
        
        //[Column(TypeName = "decimal(18,2)")] //esto creo que hay que configurarlo con Fluent API
        public decimal Price { get; set; } //dinero de la cuenta
        public DateTime Date { get; set; } = DateTime.Now; //creacion de la cuenta?

        // Transacciones? (hechas x el user o recibidas / historial?)
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
