using DigitalArs.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DigitalArs.Domain.Entities
{
    public class Transaction
    {
        public int ID_Transaction { get; set; }

        //Relacion con Account
        public int ID_Account { get; set; }
        public Account Account { get; set; }
        public TransactionType Type { get; set; } //llamamos el enum transactiontype
        //[Column(TypeName = "decimal(18,2")]
        public decimal Amount { get; set; }
        public DateTime Date_Transaction { get; set; } = DateTime.UtcNow;
        
        //MANEJO DE TRANSACCIONES
        //relaciones entre cuentas



    }
}
