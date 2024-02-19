using Org.BouncyCastle.Asn1.Pkcs;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IPasswordHasher
    {
        public string HashPassword(string password);
        public bool varifyPassword(string email , string password);
    }
}
