using BAL.Interface;
using DAL.DataContext;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;

namespace BAL.Repository
{

    // LoginRepository Contains LoginVarify method defination. 

    public class LoginRepo : ILogin
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<LoginVM> _passwordHasher;

        public LoginRepo(ApplicationDbContext context, IPasswordHasher<LoginVM> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        } 

        public bool LoginVarify(LoginVM model)
        {
            bool isexistuser = _context.AspNetUsers.Any(u => u.Email == model.Email);
            if (!isexistuser)
            {
                return false;
            }
            var user = _context.AspNetUsers.FirstOrDefault(u => u.Email == model.Email);
            var result = _passwordHasher.VerifyHashedPassword(model, user.PasswordHash, model.PasswordHash);
            bool IsTruePassword = result == PasswordVerificationResult.Success;
            if(isexistuser && IsTruePassword)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class CustomAuthorize : Attribute , IAuthorizationFilter
    {
        private readonly string _role;
        private readonly ApplicationDbContext _context;
        public CustomAuthorize(string role = "Patient")
        {
            this._role = role;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwtService = context.HttpContext.RequestServices.GetService<IJwtToken>();
            var email = context.HttpContext.Session.GetString("Email");
            var roLe = context.HttpContext.Session.GetString("Role");
            
            if(jwtService == null)
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
            // Access Denied if Role Not matched
            if(roleClaim == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Home", Action = "PatientLoginn" }));
                return;
            }
            if(string.IsNullOrWhiteSpace(_role) || roleClaim.Value != _role) { 
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Home", Action = "AccessDenied" }));
            }
        }
    }
}
