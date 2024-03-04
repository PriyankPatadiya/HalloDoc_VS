using BAL.Interface;
using DAL.DataContext;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace HalloDoc.Controllers
{
    public class patientFormsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IPatientRequest _patreq;
        private readonly IRequests _otherreq;
        private IHostingEnvironment _environment;
        private readonly IuploadFile _uploadfile;

        public patientFormsController(ApplicationDbContext context, IPatientRequest patreq, IRequests otherreq, IHostingEnvironment environment, IuploadFile uploadFile)
        {
            _context = context; 
            _patreq = patreq;
            _otherreq = otherreq;
            _environment = environment;
            _uploadfile = uploadFile;
        }
        public IActionResult PatientRequestForm()
        {
            PatientReqVM model = new PatientReqVM();
            model.Region = _context.Regions.ToList();

            return View(model);
        }


        public IActionResult Friend_FamilyRequestForm()
        {
            OthersReqVM model = new OthersReqVM();
            model.Region = _context.Regions.ToList();
            return View(model);
        }
        public IActionResult BusinessRequestForm()
        {
            OthersReqVM model = new OthersReqVM();
            model.Region = _context.Regions.ToList();
            return View(model);
        }
        public IActionResult ConciergeRequestForm()
        {
            OthersReqVM model = new OthersReqVM();
            model.Region = _context.Regions.ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PatientRequestForm(PatientReqVM pInfo) 
            {
            var user = _context.AspNetUsers.FirstOrDefault(a  => a.Email == pInfo.Email);
            pInfo.State =  await _patreq.GetStateAccordingToRegionId(pInfo.SelectedStateId);
                pInfo.CreatedDate = DateTime.Now.Date;
                pInfo.confirmationnumber = _patreq.GenerateConfirmationNumber(pInfo);
            if (ModelState.IsValid)
            {
                if (pInfo.PasswordHash != pInfo.ConfirmPasswordHash && user == null)
                {
                    ModelState.AddModelError("ConfirmPassword", "Password and ConfirmPassword is not same");
                    return View("PatientRequestForm");
                }
                var uniquefilesavetoken = new Guid().ToString();

                string fileName = Path.GetFileName(pInfo.Document.FileName);
                fileName = $"{fileName}_{uniquefilesavetoken}";

                _patreq.AddPatientForm(pInfo);

                if (pInfo.Document != null && pInfo.Document.Length > 0 )
                {
                    string path = Path.Combine(this._environment.WebRootPath, "Uploads");
                    
                    
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }


                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        pInfo.Document.CopyTo(stream);
                    }

                    var request = _patreq.GetUserByEmail(pInfo.Email);
                    _patreq.Addrequestwisefile(fileName, request.RequestId);


                    return RedirectToAction("SubmitRequest", "Home");
                }
            }
            return View("PatientRequestForm");
        }




        


        [HttpPost]
        public async Task<IActionResult> Friend_FamilyRequest(OthersReqVM model)
        {
            model.State = await _patreq.GetStateAccordingToRegionId(model.SelectedStateId);
                model.CreatedDate = DateTime.Now.Date;
                model.confirmationnumber = _otherreq.GenerateConfirmationNumber(model);
            if (ModelState.IsValid)
            {
                _otherreq.AddFamilyFriendForm(model);
                if(model.Document != null && model.Document.Length > 0)
                {
                    string path = Path.Combine(this._environment.WebRootPath, "Uploads");
                    string fileName = Path.GetFileName(model.Document.FileName);
                    _uploadfile.uploadfile(model.Document, path);

                    var request = _patreq.GetUserByEmail(model.Email);

                    if (request != null)
                    {
                        _patreq.Addrequestwisefile(fileName, request.RequestId);
                    }
                    return RedirectToAction("SubmitRequest","Home");
                }
            }
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> ConciergeRequestForm(OthersReqVM model)
        {
                model.State =   await _patreq.GetStateAccordingToRegionId(model.SelectedStateId);
                model.CreatedDate = DateTime.Now.Date;
                model.confirmationnumber = _otherreq.GenerateConfirmationNumber(model);
            if (ModelState.IsValid)
            {
                _otherreq.AddConciergeForm(model);

                    return RedirectToAction("SubmitRequest", "Home");

            }
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> BusinessRequestForm(OthersReqVM model)
        {
                model.State = await _patreq.GetStateAccordingToRegionId(model.SelectedStateId);
                model.CreatedDate = DateTime.Now.Date;
                model.confirmationnumber = _otherreq.GenerateConfirmationNumber(model);
            if (ModelState.IsValid) {
                _otherreq.AddBusinessForm(model);
                return RedirectToAction("SubmitRequest", "Home");
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
