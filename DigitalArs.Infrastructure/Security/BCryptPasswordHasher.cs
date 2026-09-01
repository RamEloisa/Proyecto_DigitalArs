using System;
using System.Collections.Generic;
using System.Text;

using DigitalArs.Application.Security;
using BCrypt.Net;

namespace DigitalArs.Infrastructure.Security
{
    //implementamos ipasswordhasher con bcrypt
    //BCrypt genera automaticamente un "salt" distinto para cada contraseña
    //por eso hashear la misma contraseña dos veces da resultados distintos
    //con Verify reconocemos q coinciden
    
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string plainPassword)
        {
            //BCrypt.HashPassword genera el salt internamente y lo incluye en el resultado
            return BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        public bool Verify(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }
    }
}