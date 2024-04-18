
using System.IdentityModel.Tokens.Jwt;

namespace BAL.Interface
{
    public interface IJwtToken
    {
        string generateJwtToken(string token, string role);

        bool ValidateToken( string token, out JwtSecurityToken jwtSecurityToken );
    }
}
