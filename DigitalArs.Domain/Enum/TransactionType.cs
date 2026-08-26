using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalArs.Domain.Enum
{
    /* MOVIMIENTOS PARA LA TRANSACCION
     * DEPOSIT: dinero externo a una cuenta
     * TRANSFERIN: recibe dinero
     * TRANSFEROUT
     */
    public enum TransactionType
    {
        Deposit,
        Transfer_In,
        Transfer_Out
    }
}
