using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalArs.Application.Security
{
    public interface IPasswordHasher
    {
        //Genera el hash de una contraseña en texto plano
        //Se usa al registrar un usuario, antes de guardar la contraseña en la BD
        string Hash(string plainPassword);

        //Verificamos si una contraseña en texto plano coincide con un hash ya guardado (esto para usar el salt dsp)
        //Se usa al hacer login, para comparar sin necesidad de desencriptar
        bool Verify(string plainPassword, string hashedPassword);
    }
}