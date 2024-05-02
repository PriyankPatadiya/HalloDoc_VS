using BAL.Interface;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using DAL.DataContext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace BAL.Repository
{
    public class AuthorizationRepo 
    {
        public class CustomAuthorize : Attribute, IAuthorizationFilter
        {
            private readonly string[] _Asprole;
            private readonly string _menuId;
            private readonly ApplicationDbContext _context;
            public CustomAuthorize(string[] Asprole, string role)
            {
                this._Asprole = Asprole;
                _menuId = role;
            }
            public void OnAuthorization(AuthorizationFilterContext context)
                {
                var jwtService = context.HttpContext.RequestServices.GetService<IJwtToken>();
                var email = context.HttpContext.Session.GetString("Email");
                var roLe = context.HttpContext.Session.GetString("Role");
                var loginRepo = context.HttpContext.RequestServices.GetService<ILogin>();

                if (jwtService == null)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Home", Action = "PatientLoginn" }));
                    return;
                }
                var request = context.HttpContext.Request;
                var token = request.Cookies["jwt"];

                // Redirect to login if not logged in 
                if (token == null || !jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Home", Action = "PatientLoginn" }));
                    return;
                }
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Role");
                var roleId = jwtToken.Claims.FirstOrDefault(c => c.Type == "roleId");
                var roleMenu = loginRepo.getRoleMenuforAuth(roleId.Value, _menuId);
                // Access Denied if Role Not matched
                if (roleClaim == null)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Home", Action = "PatientLoginn" }));
                    return;
                }
                if (_Asprole.Length < 1 || !_Asprole.Contains(roleClaim.Value) || (roleMenu.Count() == 0 && roleId.Value != "0"))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Home", Action = "AccessDenied" }));
                }
            }
        }
    }
}
