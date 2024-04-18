using BAL.Interface;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace HalloDoc.Controllers
{
    public class patientFormsController : Controller
    {

        private readonly IPatientRequest _patreq;
        private readonly IRequests _otherreq;
        [Obsolete]
        private IHostingEnvironment _environment;
        private readonly IuploadFile _uploadfile;
        private readonly IEmailService _email;
        private readonly IAdminDashboard _admin;
        private readonly ILogin _login;

        [Obsolete]
        public patientFormsController( IPatientRequest patreq, IRequests otherreq, IHostingEnvironment environment, IuploadFile uploadFile, IEmailService email, IAdminDashboard admin, ILogin login)
        {
            _patreq = patreq;
            _otherreq = otherreq;
            _environment = environment;
            _uploadfile = uploadFile;
            _email = email;
            _admin = admin;
            _login = login;
        }

        #region Views

        public IActionResult PatientRequestForm()
        {
            PatientReqVM model = new PatientReqVM();
            model.Region = _admin.regions();

            return View(model);
        }
        public IActionResult Friend_FamilyRequestForm()
        {
            OthersReqVM model = new OthersReqVM();
            model.Region = _admin.regions();
            return View(model);
        }
        public IActionResult BusinessRequestForm()
        {
            OthersReqVM model = new OthersReqVM();
            model.Region = _admin.regions();
            return View(model);
        }
        public IActionResult ConciergeRequestForm()
        {
            OthersReqVM model = new OthersReqVM();
            model.Region = _admin.regions();
            return View(model);
        }

        #endregion

        #region AddPatientRequest
        [HttpPost]
        public async Task<IActionResult> PatientRequestForm(PatientReqVM pInfo)
        {
            var user = _login.AspuserbyEmail(pInfo.Email);
            if (ModelState.IsValid)
            {
                if (pInfo.PasswordHash != pInfo.ConfirmPasswordHash)
                {
                    ModelState.AddModelError("ConfirmPassword", "Password and ConfirmPassword is not same");
                    return View("PatientRequestForm");
                }
                pInfo.State = await _patreq.GetStateAccordingToRegionId(pInfo.SelectedStateId);
                pInfo.CreatedDate = DateTime.Now.Date;
                pInfo.confirmationnumber = _patreq.GenerateConfirmationNumber(pInfo);
                _patreq.AddPatientForm(pInfo);
                if (pInfo.Document != null && pInfo.Document.Length > 0)
                {
                    var uniquefilesavetoken = Guid.NewGuid().ToString();
                    string fileName = Path.GetFileNameWithoutExtension(pInfo.Document.FileName);
                    string extension = Path.GetExtension(pInfo.Document.FileName);
                    fileName = $"{fileName}_{uniquefilesavetoken}{extension}";
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
                    TempData["Message"] = "Request Added";
                    TempData["MessageType"] = "success";
                    return RedirectToAction("SubmitRequest", "Home");
                }
            }
            pInfo.Region = _admin.regions();
            return View("PatientRequestForm", pInfo);
        }
        #endregion

        #region AddF-FRequest
        [HttpPost]
        public async Task<IActionResult> Friend_FamilyRequest(OthersReqVM model)
        {
            var user = _login.AspuserbyEmail(model.Email);
            
            if (ModelState.IsValid)
            {
                if (user == null)
                {
                    var link = Url.ActionLink("PatientCreateAcc", "Home", new { email = model.Email }, protocol: HttpContext.Request.Scheme);
                    string to = model.Email;
                    string subject = "Create Your Account On HalloDoc";
                    var body = new StringBuilder();
                    body.AppendLine("Please Create Your Request On HalloDoc To show status of your request here");
                    body.AppendLine("<a href=\"" + link + "\">Click here to create Acc</a>");
                    string Body = body.ToString();
                    _email.SendEmail(to, subject, Body);
                }
                model.State = await _patreq.GetStateAccordingToRegionId(model.SelectedStateId);
                model.CreatedDate = DateTime.Now.Date;
                model.confirmationnumber = _otherreq.GenerateConfirmationNumber(model);
                _otherreq.AddFamilyFriendForm(model);
                if (model.Document != null && model.Document.Length > 0)
                {
                    string path = Path.Combine(this._environment.WebRootPath, "Uploads");
                    var uniquefilesavetoken = Guid.NewGuid().ToString();
                    string fileName = Path.GetFileNameWithoutExtension(model.Document.FileName);
                    string extension = Path.GetExtension(model.Document.FileName);
                    fileName = $"{fileName}_{uniquefilesavetoken}{extension}";
                    _uploadfile.uploadfile(model.Document,fileName, path);
                    var request = _patreq.GetUserByEmail(model.Email);
                    _patreq.Addrequestwisefile(fileName, request.RequestId);
                }
                TempData["Message"] = "Request Added";
                TempData["MessageType"] = "success";
                return RedirectToAction("SubmitRequest", "Home");
            }
            model.Region = _admin.regions();
            return View("Friend_FamilyRequestForm", model);
        }
        #endregion

        #region AddConciergeRequest
        [HttpPost]
        public async Task<IActionResult> ConciergeRequestForm(OthersReqVM model)
        {
            var user = _login.AspuserbyEmail(model.Email);
            
            if (ModelState.IsValid)
            {
                if (user == null)
                {
                    var link = Url.ActionLink("PatientCreateAcc", "Home", new { email = model.Email }, protocol: HttpContext.Request.Scheme);
                    string to = model.Email;
                    string subject = "Create Your Account On HalloDoc";
                    var body = new StringBuilder();
                    body.AppendLine("Please Create Your Request On HalloDoc To show status of your request here");
                    body.AppendLine("<a href=\"" + link + "\">Click here to create Acc</a>");
                    string Body = body.ToString();
                    _email.SendEmail(to, subject, Body);
                }
                model.State = await _patreq.GetStateAccordingToRegionId(model.SelectedStateId);
                model.CreatedDate = DateTime.Now.Date;
                model.confirmationnumber = _otherreq.GenerateConfirmationNumber(model);
                _otherreq.AddConciergeForm(model);
                TempData["Message"] = "Request Added";
                TempData["MessageType"] = "success";
                return RedirectToAction("SubmitRequest", "Home");
            }
            model.Region = _admin.regions();
            return View("ConciergeRequestForm", model);
        }
        #endregion

        #region AddBusinessRequest

        [HttpPost]

        public async Task<IActionResult> BusinessRequestForm(OthersReqVM model)
        {
            var user = _login.AspuserbyEmail(model.Email);
           
            if (ModelState.IsValid) {
                if (user == null)
                {
                    var link = Url.ActionLink("PatientCreateAcc", "Home", new { email = model.Email }, protocol: HttpContext.Request.Scheme);
                    string to = model.Email;
                    string subject = "Create Your Account On HalloDoc";
                    var body = new StringBuilder();
                    body.AppendLine("Please Create Your Request On HalloDoc To show status of your request here");
                    body.AppendLine("<a href=\"" + link + "\">Click here to create Acc</a>");
                    string Body = body.ToString();
                    _email.SendEmail(to, subject, Body);
                }
                model.State = await _patreq.GetStateAccordingToRegionId(model.SelectedStateId);
                model.CreatedDate = DateTime.Now.Date;
                model.confirmationnumber = _otherreq.GenerateConfirmationNumber(model);
                _otherreq.AddBusinessForm(model);
                TempData["Message"] = "Request Added";
                TempData["MessageType"] = "success";
                return RedirectToAction("SubmitRequest", "Home");
            }
            model.Region = _admin.regions();
            return View("BusinessRequestForm", model);
        }

        #endregion

        #region Mails

        [HttpPost]
        public bool CheckMail(string email)
        {
            var user = _login.isAspNetUser(email);
            return user;
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

        #endregion
    }
}
