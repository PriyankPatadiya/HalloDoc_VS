using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IJwtToken
    {
        string generateJwtToken(string token, string role);

        bool ValidateToken( string token, out JwtSecurityToken jwtSecurityToken );
    }
}
