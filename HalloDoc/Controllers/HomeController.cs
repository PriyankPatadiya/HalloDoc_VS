using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DAL.DataContext;
using DAL.DataModels;
using BAL.Interface;
using DAL.ViewModels;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace HalloDoc.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ILogin _login;
        private readonly ICreateAcc _createacc;
        private readonly IEmailService _emailService;
        private readonly SymmetricSecurityKey _key;
        private readonly IConfiguration _config;
        private readonly IPasswordHasher<LoginVM> _passwordHasher;
        private readonly IJwtToken _jwttoken;
        private readonly IPasswordHasher<CreateAccVM> _passwordcreatehasher;

        //private readonly IJWTTokense


        public HomeController(IConfiguration config, ApplicationDbContext db, ILogin login, ICreateAcc createacc, IEmailService emailService, IPasswordHasher<LoginVM> passwordHasher, IJwtToken token, IPasswordHasher<CreateAccVM> passwordcreatehasher)
        {
            _context = db;
            _login = login;
            _createacc = createacc;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
            _jwttoken = token;
            //_jwtTokenService = jwtservice
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            _passwordcreatehasher = passwordcreatehasher;
        }
        public IActionResult Privacy(string name, int numTimes = 1)
        {
            ViewData["Messege"] = "Hello" + name;
            ViewData["numTimes"] = numTimes;
            return View();
        }
        public IActionResult SubmitRequest()
        {
            return View();
        }
        public IActionResult PatientSite()
        {
            return View();
        }
        public IActionResult PatientLoginn()
        {
            return View();
        }
        public IActionResult PatientDashboard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {

            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
           }

        //public IActionResult PatientCreateAcc()
        //{
        //    return View();
        //}
        public IActionResult PatientCreateAcc(string email)
        {
            CreateAccVM model = new CreateAccVM();
            model.UserName = email;
            return View(model);
        }
        public IActionResult Logout()
        { 
            HttpContext.Session.Remove("Email");
            HttpContext.Session.Remove("Role");
            Response.Cookies.Delete("jwt");
            return RedirectToAction("PatientLoginn");
        }


        //Post  Login Action


        [HttpPost]
        public async Task<IActionResult> PatientLoginn(LoginVM a)
        {

            if (ModelState.IsValid)
            {
                var user = _login.LoginVarify(a);

                if (user == true)
                {
                    HttpContext.Session.SetString("Email", a.Email);
                    var userid = _context.AspNetUsers.Where(u => u.Email == a.Email).First();
                    int roleid = (int)_context.AspNetUserRoles.FirstOrDefault(u => u.UserId == userid.Id).RoleId;
                    string role = _context.AspNetRoles.Where(i => i.Id == roleid).First().Name;
                    HttpContext.Session.SetString("Role", role);

                    string token = _jwttoken.generateJwtToken(a.Email, role);
                    Response.Cookies.Append("jwt", token);
                    
                    if (roleid == 1)
                    {
                        return RedirectToAction("MainPage", "AdminDashboard");
                    }
                    if(roleid == 2)
                    {
                        return RedirectToAction("PatientDashboard", "PatientDashBoard");
                    }
                    if(roleid == 3)
                    {
                        return RedirectToAction("Dashboard", "ProviderDashboard");
                    }
                }
                else
                {
                    TempData["Message"] = "Invalid Credentials!";
                    TempData["MessageType"] = "error";
                    return View();
                }
            }
            return View(a);
        }


        //POST Create Action

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PatientCreateAcc(CreateAccVM Obj)
        {
            if (ModelState.IsValid)
            {
                if (Obj.Password != Obj.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "The Password and Confirm Password do not match");
                    return View(Obj);
                }
                //var newUser = new AspNetUser
                //{
                //    Id = Guid.NewGuid().ToString(),
                //    UserName = Obj.UserName,
                //    PasswordHash = _passwordcreatehasher.HashPassword(null, Obj.Password),
                //};

                var client = _context.RequestClients.FirstOrDefault(u => u.Email == Obj.UserName);
                var aspnetuser = new AspNetUser();


                aspnetuser.Id = Guid.NewGuid().ToString();
                aspnetuser.Email = Obj.UserName;
                aspnetuser.CreatedDate = DateTime.Now;
                aspnetuser.UserName = Obj.UserName;
                aspnetuser.PasswordHash = _passwordcreatehasher.HashPassword(null, Obj.Password);
                aspnetuser.PhoneNumber = client.PhoneNumber;


                _context.AspNetUsers.Add(aspnetuser);
                _context.SaveChanges();

                var user = new User();
                user.AspNetUserId = aspnetuser.Id;
                user.FirstName = client.FirstName;
                user.LastName = client.LastName;
                user.Mobile = client.PhoneNumber;
                user.Street = client.Street;
                user.City = client.City;
                user.State = client.State;
                user.ZipCode = client.ZipCode;
                user.Email = Obj.UserName;
                user.CreatedBy = client.FirstName;
                user.IntYear = client.IntYear;
                user.IntDate = client.IntDate;
                user.StrMonth = client.StrMonth;
                user.RegionId = client.RegionId;

                _context.Users.Add(user);
                _context.SaveChanges();

                var aspnetuserrole = new AspNetUserRole
                {
                    UserId = aspnetuser.Id,
                    RoleId = 2
                };

                _context.AspNetUserRoles.Add(aspnetuserrole);
                _context.SaveChanges();

                return RedirectToAction(nameof(Privacy));
            }
            return View(Obj);
        }



        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public IActionResult ResetPassword(ForgetPasswordVM model)
        {
            // Token Generation 

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("email", model.email) }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expires in 1 hour
                Issuer = _config["Jwt:Issuer"],
                //Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key)

                , SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);



            // Sending Mail Information

            string to = model.email;
            string subject = "Forget Your Password... Do not worry You can change your password.";
            var resetlink = Url.Action("ChangePassword", "Home", new { token = jwtToken }, Request.Scheme);

            var body = new StringBuilder();
            body.AppendFormat("Hello");
            body.AppendLine(@"Your KAUH Account about to activate click 
                             the link below to complete the actination process");
            body.AppendLine("<a href=\"" + resetlink + "\">Change Your Password</a>");

            string Body = body.ToString();


            if (ModelState.IsValid)
            {
                var user = _context.Users.Where(x => x.Email == to);
                _emailService.SendEmail(to, subject, Body);
                model.EmailSent = true;
                return RedirectToAction("PatientLoginn");
            }
            return View();
        }



        [HttpGet]
        public IActionResult ChangePassword(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

                // 5. Verify the JWT token and allow the user to reset the password if the token is valid
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)

,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var email = jwtToken.Claims.First(x => x.Type == "email").Value;

                // Pass the email to your password reset view
                ViewBag.Email = email;
                return View();

            }
            catch 
            {
                return Content("Invalid token");
            }
        }

        [HttpPost]
        public IActionResult ChangePassword(ForgetPasswordVM model)
        {
            if (ModelState.IsValid && model != null && _context.AspNetUsers.Any(u => u.Email == model.email))
            {
                if (model.Password == model.ConfirmPassword)
                {
                    var user = _context.AspNetUsers.FirstOrDefault(u => u.Email == model.email);
                    user.PasswordHash = _passwordHasher.HashPassword(null, model.Password);
                    _context.AspNetUsers.Update(user);
                    _context.SaveChanges();
                    return View("PatientLoginn");
                }
            }
            return Content("Model is not valid");
        }

        [AllowAnonymous]
        public JsonResult CheckSession()
        {
            var request = HttpContext.Request;
            var token = request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { sessionExists = false });
            }
            else
            {
                return Json(new { sessionExists = true });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }


}