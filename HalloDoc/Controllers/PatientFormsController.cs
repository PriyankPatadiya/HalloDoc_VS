using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace HalloDoc.Controllers
{
    public class patientFormsController : Controller
    {

        private readonly ApplicationDbContext _context; 

        public patientFormsController(ApplicationDbContext context)
        {
            _context = context; 
        }
        public IActionResult PatientRequestForm()
        {
            return View();
        }


        public IActionResult Friend_FamilyRequestForm()
        {
            return View();
        }
        public IActionResult BusinessRequestForm()
        {
            return View();
        }
        public IActionResult ConciergeRequestForm()
        {
            return View();
        }

        public IActionResult PatientReq(PatientReqVM pInfo)
        {
            if(ModelState.IsValid) 
            {
                if (pInfo != null)
                {
                    var aspnetuser = new AspNetUser();
                   
                    aspnetuser.Id = Guid.NewGuid().ToString();
                    aspnetuser.Email = pInfo.Email;
                    aspnetuser.CreatedDate = DateTime.Now;
                    aspnetuser.UserName = pInfo.Email;

                    _context.AspNetUsers.Add(aspnetuser);
                    _context.SaveChanges();
                   
                    var user = new User();
                    user.AspNetUserId = aspnetuser.Id;
                    user.FirstName = pInfo.FirstName;
                    user.LastName  = pInfo.LastName;
                    user.Mobile = pInfo.PhoneNumber;
                    user.Street = pInfo.Street;
                    user.City = pInfo.City; 
                    user.State = pInfo.State;   
                    user.ZipCode = pInfo.ZipCode;
                    user.Email = pInfo.Email;
                    user.CreatedBy = pInfo.FirstName;

                    _context.Users.Add(user);
                    _context.SaveChanges();

                    var request = new Request();
                    
                    request.UserId = user.UserId;
                    request.FirstName = pInfo.FirstName;
                    request.LastName = pInfo.LastName;  
                    request.CreatedDate = DateTime.Now;
                    request.PhoneNumber = pInfo.PhoneNumber;    
                    request.Email = pInfo.Email;
                    
                    _context.Requests.Add(request);
                    _context.SaveChanges();
                }

                
                
            }
            return View("Friend_FamilyRequestForm");
        }

        [HttpPost]
        public JsonResult CheckEmail(string email)
        {
            AspNetUser entities = new AspNetUser();
            bool isValid = !entities.Users.ToList().Exists(p => p.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase));
            return Json(isValid);
        }
    }
}
