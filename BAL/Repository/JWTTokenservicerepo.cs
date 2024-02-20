//using BAL.Interface;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;


//namespace BAL.Repository
//{
//    public class JWTTokenservicerepo : IJWTTokenservice
//    {
//        private readonly SymmetricSecurityKey _key;
//        private readonly TimeSpan _expires;

//        public JWTTokenservicerepo(SymmetricSecurityKey key, TimeSpan expires)
//        {
//            _key = key;
//            _expires = expires;
//        }   

//        public string generatetoken(string email)
//        {
//            var tokenHandler = new JwtSecurityTokenHandler();
//            var tokenDescriptor = new SecurityTokenDescriptor
//            {
//                Subject = new ClaimsIdentity(new Claim[]
//                {
//                    new Claim(ClaimTypes.Email, email)
//                }),
//                Expires = DateTime.UtcNow.Add(_expires),
//                SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature)
//            };

//            var token = tokenHandler.CreateToken(tokenDescriptor);
//            return tokenHandler.WriteToken(token);

//        }

//        public string ValidateToken(string token)
//        {
//            var tokenHandler = new JwtSecurityTokenHandler();

//            try
//            {
//                tokenHandler.ValidateToken(token, new TokenValidationParameters
//                {
//                    ValidateIssuerSigningKey = true,
//                    IssuerSigningKey = _key,
//                    ValidateIssuer = false,
//                    ValidateAudience = false,
//                    ClockSkew = TimeSpan.Zero
//                }, out SecurityToken validatedToken);

//                var jwtToken = (JwtSecurityToken)validatedToken;
//                var userEmail = jwtToken.Claims.First(u => u.Type == ClaimTypes.Email).Value;
//                return userEmail;
//            }
//            catch
//            {
//                return null;
//            }
//        }
//    }
//}
