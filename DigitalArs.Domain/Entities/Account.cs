using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DigitalArs.Domain.Entities
{
    public class Account
    {
        public int ID_Account { get; set; }
        public int ID_User { get; set; } //relacion con user mediante el id
        public string Name { get; set; } // nombre de la cuenta o lo traemos del user?
        [Column(TypeName = "decimal(18,2)")] //fuerza el tipo decimal en la bd de 18 digitos y 2 decimales
        public decimal Price { get; set; } //dinero de la cuenta
        public DateTime Date { get; set; } = DateTime.Now; //creacion de la cuenta?

        // Transacciones? (hechas x el user o recibidas / historial?)
    }
}
