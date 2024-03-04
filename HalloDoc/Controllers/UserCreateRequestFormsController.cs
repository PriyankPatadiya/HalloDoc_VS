using BAL.Interface;
using DAL.DataContext;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace HalloDoc.Controllers
{
    public class UserCreateRequestFormsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IPatientRequest _request;
        private readonly IHostingEnvironment _environment;
        private readonly IuploadFile _uploadfile;

        public UserCreateRequestFormsController(ApplicationDbContext context, IPatientRequest req, IHostingEnvironment environment)
        {
            _request = req;
            _context = context;
            _environment = environment;
        }
        public IActionResult RequestForMe()
        {
            var email = HttpContext.Session.GetString("Email");
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            var req = new PatientReqVM();
            req.FirstName = user.FirstName;
            req.LastName = user.LastName;
            req.Email = email;
            req.PhoneNumber = user.Mobile;
            
            return View(req);
        }


        public IActionResult RequestForSomeoneElse()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RequestForMe(PatientReqVM model)
        {
            var email = HttpContext.Session.GetString("Email");
            var user = _context.AspNetUsers.Any(u => u.Email == email);

            

            _request.AddPatientForm(model);
            //if (model.Document != null && model.Document.Length > 0)
            //{
            //    string path = Path.Combine(this._environment.WebRootPath, "Uploads");
            //    string fileName = Path.GetFileName(model.Document.FileName);

            //    model.uploadfile(model.Document, path);

            //    var request = model.GetUserByEmail(model.Email);
            //    _patreq.Addrequestwisefile(fileName, request.RequestId);


            //    return View("Friend_FamilyRequestForm");
            //}

            return View();
            
        }
    }
}
