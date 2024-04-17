﻿using BAL.Interface;
using BAL.Repository;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace HalloDoc.Controllers
{
    [CustomAuthorize(new string[] { "Patient" })]
    public class UserCreateRequestFormsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IPatientRequest _request;
        [Obsolete]
        private readonly IHostingEnvironment _environment;
        private readonly IuploadFile _uploadfile;
        private readonly IPatientRequest _patreq;
        private readonly IRequests _others;

        [Obsolete]
        public UserCreateRequestFormsController(ApplicationDbContext context, IPatientRequest req, IHostingEnvironment environment, IPatientRequest parreq, IRequests others)
        {
            _request = req;
            _context = context;
            _environment = environment;
            _patreq = parreq;
            _others = others;
        }
        public IActionResult RequestForMe()
        {
            var email = HttpContext.Session.GetString("Email");
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            var req = new PatientReqVM();
            req.Region = _context.Regions.ToList();
            req.FirstName = user.FirstName;
            req.LastName = user.LastName;
            req.Email = email;
            req.PhoneNumber = user.Mobile;

            ViewBag.username = user.FirstName + " " + user.LastName;    
            
            return View(req);
        }


        public IActionResult RequestForSomeoneElse()
        {
            var email = HttpContext.Session.GetString("Email");
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            var req = new PatientReqVM();
            req.Region = _context.Regions.ToList();
            ViewBag.username = user.FirstName + " " + user.LastName;
            return View(req);
        }

        [HttpPost]
        public async Task<IActionResult> RequestForMe(PatientReqVM model)
        {
            var email = HttpContext.Session.GetString("Email");
            var user = _context.AspNetUsers.Any(u => u.Email == email);
                model.State = await _patreq.GetStateAccordingToRegionId(model.SelectedStateId);
                model.CreatedDate = DateTime.Now.Date;
            if(user && ModelState.IsValid)
            {
                model.confirmationnumber = _patreq.GenerateConfirmationNumber(model);

                
                _patreq.AddPatientForm(model);

                if (model.Document != null && model.Document.Length > 0)
                {
                    var uniquefilesavetoken = new Guid().ToString();
                    string fileName = Path.GetFileNameWithoutExtension(model.Document.FileName);
                    string extension = Path.GetExtension(model.Document.FileName);
                    fileName = $"{fileName}_{uniquefilesavetoken}{extension}";

                    string path = Path.Combine(this._environment.WebRootPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        model.Document.CopyTo(stream);
                    }
                    var request = _patreq.GetUserByEmail(model.Email);
                    _patreq.Addrequestwisefile(fileName, request.RequestId);

                    return RedirectToAction("PatientDashboard", "PatientDashBoard");
                }
            }
            var req = new PatientReqVM();
            req.Region = _context.Regions.ToList();
            return View(req);
            
        }

        [HttpPost]
        public async Task<IActionResult> RequestForSomeoneElse(PatientReqVM model)
        {
            var email = HttpContext.Session.GetString("Email");
            if (ModelState.IsValid)
            {
                model.State = await _patreq.GetStateAccordingToRegionId(model.SelectedStateId);
                model.CreatedDate = DateTime.Now.Date;
                model.confirmationnumber = _patreq.GenerateConfirmationNumber(model);

                

                _patreq.AddRequestForElse(model);

                //if (model.Document != null && model.Document.Length > 0)
                //{
                //    var uniquefilesavetoken = new Guid().ToString();
                //    string fileName = Path.GetFileNameWithoutExtension(model.Document.FileName);
                //    string extension = Path.GetExtension(model.Document.FileName);
                //    fileName = $"{fileName}_{uniquefilesavetoken}{extension}";
                //    string path = Path.Combine(this._environment.WebRootPath, "Uploads");
                //    if (!Directory.Exists(path))
                //    {
                //        Directory.CreateDirectory(path);
                //    }
                //    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                //    {
                //        model.Document.CopyTo(stream);
                //    }
                //    var request = _patreq.GetUserByEmail(model.Email);
                //    _patreq.Addrequestwisefile(fileName, request.RequestId);

                //    return RedirectToAction("PatientDashboard", "PatientDashBoard");
                //}
            }
            var req = new PatientReqVM();
            req.Region = _context.Regions.ToList();
            return View(req);
        }
    }
}
