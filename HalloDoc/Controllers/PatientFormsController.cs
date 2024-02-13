using BAL.Interface;
using DAL.DataContext;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Npgsql.Internal.TypeHandlers.LTreeHandlers;
using System.Net;
using System.Net.Mail;
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace HalloDoc.Controllers
{
    public class patientFormsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IPatientRequest _patreq;
        private readonly IRequests _otherreq;
        private IHostingEnvironment _environment;

        public patientFormsController(ApplicationDbContext context, IPatientRequest patreq, IRequests otherreq, IHostingEnvironment environment)
        {
            _context = context; 
            _patreq = patreq;
            _otherreq = otherreq;
            _environment = environment;
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

        [HttpPost]
        public IActionResult PatientRequestForm(PatientReqVM pInfo) 
        {
            if(ModelState.IsValid) 
            {
                if(pInfo.PasswordHash != pInfo.ConfirmPasswordHash)
                {
                    ModelState.AddModelError("ConfirmPassword", "Password and ConfirmPassword is not same");
                    return View("PatientRequestForm");
                }
                
                if (pInfo != null)
                {
                    
                   _patreq.AddPatientForm(pInfo);

                    string wwwpath = this._environment.WebRootPath;
                    string contentpath = this._environment.ContentRootPath;
                    string path = Path.Combine(this._environment.WebRootPath, "Uploads");


                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    List<string> uploadedFiles = new List<string>();

                    string fileName = Path.GetFileName(pInfo.Document.FileName);
                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        pInfo.Document.CopyTo(stream);
                        uploadedFiles.Add(fileName);
                        ViewBag.Message += string.Format("<b>{0}</b> uploaded.<br />", fileName);
                    }
                    return View("Friend_FamilyRequestForm");
                }
            }
            return View("PatientRequestForm");
        }
        


        [HttpPost]
        public IActionResult Friend_FamilyRequest(OthersReqVM model)
        {
            if (ModelState.IsValid)
            {
                if(model != null)
                {
                    _otherreq.AddFamilyFriendForm(model);
                    return View();
                }
            }
            return View("Friend_FamilyRequestForm");
        }

        [HttpPost]

        public IActionResult ConciergeRequestForm(OthersReqVM model)
        {
            if (ModelState.IsValid)
            {
                if(model !=null)
                {
                    _otherreq.AddConciergeForm(model);
                    return View();
                }
            }
            return View();
        }

        [HttpPost]

        public IActionResult BusinessRequestForm(OthersReqVM model)
        {
            if (ModelState.IsValid) { 
                if(model != null)
                {
                    _otherreq.AddBusinessForm(model);
                    return View();
                }
            }
            return View();
        }

        [HttpPost]
        public bool CheckMail(string email)
        {
            var user = _context.AspNetUsers.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]

        public IActionResult SendMail(OthersReqVM model)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "localhost";
            client.Port = 81;
            NetworkCredential nc = new NetworkCredential();
            nc.UserName = "UserName";
            nc.Password = "1234";
            client.Credentials = nc;
            client.EnableSsl = true;

            MailAddress from = new MailAddress("priyankppatadiya@gmail.com", "Priyank");
            MailAddress to = new MailAddress(model.Email, model.FirstName);

            MailMessage message = new MailMessage();
            message.From = from;
            message.Subject = "Register Your Account On HalloDoc";
            message.Body = @"Hello" + model.FirstName + "\n You are not registered in HalloDoc, Please register Your account... \n" +
                "<a href='https://localhost:44336/Home/PatientCreateAcc'></a>";

            client.Send(message);
            return View(model);
        }

    }
}
