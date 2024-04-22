using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using BAL.Interface;
using BAL.Repository;

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
        public AspNetUser AspuserbyEmail(string email)
        {
            return _context.AspNetUsers.Where(u => u.Email == email).First();
        }

        public AspNetUserRole getUserRoleById(string id)
        {
            return _context.AspNetUserRoles.FirstOrDefault(u => u.UserId == id);
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
        public AspNetRole roleByRoleId(int roleId)
        {
            return _context.AspNetRoles.Where(i => i.Id == roleId).First();
        }

        public bool isAdmin(string email)
        {
            return _context.Admins.Any(u => u.Email == email);
        }
        public bool isPatient(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }
        public bool isProvider(string email)
        {
            return _context.Physicians.Any(u => u.Email == email);
        }
        public Physician physicianByAspUserId(string userId)
        {
            return _context.Physicians.Where(u => u.AspNetUserId == userId).FirstOrDefault();
        }
        public bool isAspNetUser(string email)
        {
            return _context.AspNetUsers.Any(u => u.Email == email);
        }
        public User userByEmail(string email)
        {
            return _context.Users.Where(x => x.Email == email).FirstOrDefault();
        }
        public void updateAspNetUser(AspNetUser user)
        {
            _context.AspNetUsers.Update(user);
            _context.SaveChanges();
        }
        public string getRoleIdFromAdmin(string userID)
        {
            return _context.Admins.FirstOrDefault(u => u.AspNetUserId == userID).RoleId.ToString();
        }
        public string getRoleIdFromPhy(string userId)
        {
            return _context.Physicians.FirstOrDefault(u => u.AspNetUserId == userId).RoleId.ToString();
        }
        public List<RoleMenu> getRoleMenuforAuth(string roleId, string menuId)
        {
            return _context.RoleMenus.Where(u => u.RoleId == int.Parse(roleId) && u.MenuId == int.Parse(menuId)).ToList();
        }
    }

    // Authorization 

    public class CustomAuthorize : Attribute , IAuthorizationFilter
    {
        private readonly string[] _Asprole;
        private readonly string _menuId;
        private readonly ApplicationDbContext _context;
        public CustomAuthorize(string [] Asprole, string role)
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
            var roleId = jwtToken.Claims.FirstOrDefault(c => c.Type == "roleId");
            var roleMenu = loginRepo.getRoleMenuforAuth(roleId.Value, _menuId);
            // Access Denied if Role Not matched
            if(roleClaim == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Home", Action = "PatientLoginn" }));
                return;
            }
            if(_Asprole.Length < 1 || !_Asprole.Contains(roleClaim.Value) || (roleMenu.Count() == 0 && roleId.Value != "0")) { 
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Home", Action = "AccessDenied" }));
            }
        }
    }
}
