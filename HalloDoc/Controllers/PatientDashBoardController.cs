using BAL.Interface;
using BAL.Repository;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace HalloDoc.Controllers
{
    [CustomAuthorize(new string[] { "Patient" })]
    public class PatientDashBoardController : Controller
    {
        [Obsolete]
        private readonly IHostingEnvironment _environment;
        private readonly IuploadFile _uploadfile;
        private readonly IPatientRequest _request;
        private readonly IRequests _otherreq;
        private readonly ILogin _login;

        [Obsolete]
        public PatientDashBoardController( IHostingEnvironment environment , IuploadFile file, IPatientRequest request, IRequests otherreq, ILogin login)
        {
            _environment = environment;
            _uploadfile = file;
            _request = request;
            _otherreq = otherreq;
            _login = login;
        }

        

        #region Aggrement

        public IActionResult ReviewAgreement(string token)
        {
            string[] tokenParts = token.Split(':');
            string requestid = tokenParts[1];
            string timepart = tokenParts[2];
            ViewBag.Requestid = requestid;
            var Name = _request.reqclientByRequestId(int.Parse(requestid));
            string name = Name.FirstName +" "+ Name.LastName;
            ViewBag.Name = name;

            bool isTrueUser = _request.isReqClientExist(int.Parse(requestid));

            if (isTrueUser)
            {
                return View();
            }
            else
            {
                return Ok("User Invalid");
            }
        }

        public IActionResult ReviewAgreementPost(int requestid)
        {
            var request = _request.requestByRequestId(requestid);
            if (request != null && request.Status == 2)
            {
                _otherreq.AggrementReview(request, requestid);
                return RedirectToAction("PatientDashboard");
            }
            else
            {
                return Ok("User Not Exist");
            }
        }

        public IActionResult CancleReviewAgreement(int requestid, string PatientNote)
        {
            var request = _request.requestByRequestId(requestid);
            if (request != null && request.Status == 2)
            {
                _otherreq.cancelAgreement(requestid, PatientNote, request);

                return RedirectToAction("PatientDashboard");
            }
            return Ok("can't cancel request");
        }

        #endregion

        #region PatientDashboard
        public IActionResult PatientDashboard()
        {
            var email = HttpContext.Session.GetString("Email");

            var xyz = _login.userByEmail(email);
            if (xyz != null)
            {
                ViewBag.username = xyz.FirstName + xyz.LastName;


                PatientDashboardVM patientDashboardVM = _otherreq.getPatientDashboardData(email);


                return View(patientDashboardVM);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region editProfile

        [HttpPost]
        public IActionResult editprofile(ProfilePatient profilepatient)
        {
            var user = _login.userByEmail(profilepatient.Email);

            if (user != null && ModelState.IsValid)
            {
                _otherreq.EditPatientProfile(profilepatient);
                TempData["Message"] = "Profile Edited Successfully";
                TempData["MessageType"] = "success";
                return RedirectToAction("PatientDashboard");
            }
            else
            {
                return NotFound();
            }

        }

        #endregion

        #region ViewDocuments

        public IActionResult ViewDocumentPatdash()
        {
            return View();
        }

        [HttpPost]

        public IActionResult uploadFile(int requestid)
        {
            var file = Request.Form.Files["Document"];
            
            if (file == null) 
            {
                return NotFound();
            }
            else
            {
                var uniquefilesavetoken = Guid.NewGuid().ToString();

                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                fileName = $"{fileName}_{uniquefilesavetoken}{extension}";
                string path = Path.Combine(this._environment.WebRootPath, "uploads");
                _uploadfile.uploadfile(file,fileName, path);

                _request.Addrequestwisefile(fileName, requestid);
                return RedirectToAction("ViewDocumentsPatdash", new { requestid = requestid });
            }
        }


        [HttpGet]
        public IActionResult ViewDocumentsPatdash(int requestid)
        {
            var email = HttpContext.Session.GetString("Email");
            var user = _login.userByEmail(email);
            var reqfile =  _request.requestWiseFilesById(requestid);


            if (user != null)
            {
                ViewBag.username = user.FirstName + " " +user.LastName;

                var result = new ViewdocumentVM
                {
                    RequestWiseFile = reqfile,
                    requestid = requestid,
                    
                    confirmationNumber = _request.requestByRequestId(requestid).ConfirmationNumber.ToUpper()
                                  
                };

                return View(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public IActionResult downloadfile(string filename)
        {
            string Filename = Path.GetFileName(filename);
            string folderpath = Path.Combine(_environment.WebRootPath, "uploads");
            var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot\\uploads", Filename);

            if (System.IO.File.Exists(filePath))
            {
                var filestream = new FileStream(filePath, FileMode.Open);
                var contentType = "application/octet-stream";
                return File(filestream, contentType, Filename);
            }
            else
            {
                return NotFound();
            }
        }

        #endregion

    }
}
