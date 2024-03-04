using BAL.Interface;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using DAL.DataContext;

namespace BAL.Repository
{
    public class PasswordHashing : IPasswordHasher
    {
        private readonly ApplicationDbContext _context;
        
        public PasswordHashing(ApplicationDbContext context)
        {
            _context = context;
        }
        public string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];

            using (var rnd = RandomNumberGenerator.Create())
            {
                rnd.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
                ));

            return hashed;
        }

        public bool varifyPassword(string email , string password)
        {
            string hashedpasscode = _context.AspNetUsers.FirstOrDefault(u => u.Email == email).PasswordHash;

            string[] parts = hashedpasscode.Split('.', 2);
            byte[] salt = Convert.FromBase64String(parts[0]);
            string hashed = parts[1];

            string hashedProvided = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8
                ));

            return hashed == hashedProvided;
        }
    }
}
