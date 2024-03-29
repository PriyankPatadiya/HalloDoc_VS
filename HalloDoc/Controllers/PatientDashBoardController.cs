﻿using BAL.Interface;
using BAL.Repository;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Net;
using System.Net.NetworkInformation;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace HalloDoc.Controllers
{
    [CustomAuthorize("Patient")]
    public class PatientDashBoardController : Controller
    {
        private readonly ApplicationDbContext _context;
        [Obsolete]
        private readonly IHostingEnvironment _environment;
        private readonly IuploadFile _uploadfile;
        private readonly IPatientRequest _request;
        
        

        public PatientDashBoardController(ApplicationDbContext context, IHostingEnvironment environment , IuploadFile file, IPatientRequest request)
        {
            _context = context;
            _environment = environment;
            _uploadfile = file;
            _request = request;
        }

        public IActionResult ViewDocumentPatdash()
        {
            return View();
        }

        public IActionResult ReviewAgreement(string token)
        {
            string[] tokenParts = token.Split(':');
            string requestid = tokenParts[1];
            string timepart = tokenParts[2];
            ViewBag.Requestid = requestid;
            var Name = _context.RequestClients.Where(u => u.RequestId == int.Parse(requestid)).FirstOrDefault();
            string name = Name.FirstName +" "+ Name.LastName;
            ViewBag.Name = name;

            bool isTrueUser = _context.RequestClients.Any(u => u.RequestId == int.Parse(requestid));

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
            var request = _context.Requests.Where(u => u.RequestId == requestid).FirstOrDefault();
            if (request != null && request.Status == 2)
            {
                request.Status = 4;
                request.ModifiedDate = DateTime.Now;
                _context.Requests.Update(request);
                _context.SaveChanges();

                RequestStatusLog model = new RequestStatusLog();
                model.Status = request.Status;
                model.RequestId = requestid;
                model.CreatedDate = DateTime.Now;

                _context.RequestStatusLogs.Add(model);
                _context.SaveChanges();
                return RedirectToAction("PatientDashboard");
            }
            else
            {
                return Ok("User Not Exist");
            }
        }

        public IActionResult CancleReviewAgreement(int requestid, string PatientNote)
        {
            var request = _context.Requests.Where(u => u.RequestId == requestid).FirstOrDefault();
            if (request != null && request.Status == 2)
            {
                request.Status = 7;
                request.ModifiedDate = DateTime.Now;
                _context.Requests.Update(request);
                _context.SaveChanges();

                RequestStatusLog model = new RequestStatusLog();
                model.Status = request.Status;
                model.RequestId = requestid;
                model.Notes = PatientNote;
                model.CreatedDate = DateTime.Now;
                _context.RequestStatusLogs.Add(model);
                _context.SaveChanges();

                return RedirectToAction("PatientDashboard");
            }
            return Ok("can't cancel request");
        }

        public IActionResult PatientDashboard()
        {
            var email = HttpContext.Session.GetString("Email");

            var xyz = _context.Users.FirstOrDefault(u => u.Email == email);
            if (xyz != null)
            {
                ViewBag.username = xyz.FirstName + xyz.LastName;


                var Dashboard = (from req in _context.Requests
                                 join reqclient in _context.RequestClients on req.RequestId equals reqclient.RequestId
                              join requestfile in _context.RequestWiseFiles on req.RequestId equals requestfile.RequestId
                              into reqs
                              where reqclient.Email == email
                              from requestfile
                              in reqs.DefaultIfEmpty()
                              select new PatDashTableVM
                              {
                                 
                                  currentStatus = req.Status,
                                  CreatedDate = req.CreatedDate,
                                  Document = requestfile.FileName == null ? null : requestfile.FileName,
                                  requestid = req.RequestId,
                                  count = _context.RequestWiseFiles.Count(u => u.RequestId == req.RequestId),

                              }).ToList();

                var profiledata = (from user in _context.Users
                                   where user.Email == email
                                   select new ProfilePatient
                                   {
                                       FirstName = user.FirstName,
                                       LastName = user.LastName,
                                       BirthDate = new DateTime((int)user.IntYear, int.Parse(user.StrMonth), (int)user.IntDate),
                                       PhoneNumber = user.Mobile,
                                       Email = user.Email,
                                       Street = user.Street,
                                       City = user.City,
                                       State = user.State,
                                       ZipCode = user.ZipCode

                                   }).FirstOrDefault();

                PatientDashboardVM patientDashboardVM = new PatientDashboardVM();

                patientDashboardVM.DashTable = Dashboard;
                patientDashboardVM.ProfilePatient = (ProfilePatient)profiledata;
                
                return View(patientDashboardVM);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult editprofile(ProfilePatient profilepatient)
        {
            var user = _context.Users.Where(u => u.Email == profilepatient.Email).FirstOrDefault();

            if (user != null && ModelState.IsValid)
            {
                user.FirstName = profilepatient.FirstName; 
                user.LastName = profilepatient.LastName;
                user.Email = profilepatient.Email;
                user.Street = profilepatient.Street;
                user.Mobile = profilepatient.PhoneNumber;
                user.City = profilepatient.City;    
                user.State = profilepatient.State;
                user.ZipCode    = profilepatient.ZipCode;   
                user.IntDate = profilepatient.BirthDate.Day;
                user.IntYear = profilepatient.BirthDate.Year;
                user.StrMonth = profilepatient.BirthDate.Month.ToString();
                
                _context.Update(user);
                _context.SaveChanges();

                TempData["Message"] = "Profile Edited Successfully";
                TempData["MessageType"] = "success";
                return RedirectToAction("PatientDashboard");
            }
            else
            {
                return NotFound();
            }

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
        public async Task<IActionResult> ViewDocumentsPatdash(int requestid)
        {
            var email = HttpContext.Session.GetString("Email");
            var xyz = _context.Users.FirstOrDefault(u => u.Email == email);
            var reqfile =await _context.RequestWiseFiles.Where(x=>x.RequestId==requestid).ToListAsync();


            if (xyz != null )
            {
                ViewBag.username = xyz.FirstName + " " +xyz.LastName;

                var result = new ViewdocumentVM
                {
                    RequestWiseFile = reqfile,
                    requestid = requestid,
                    
                    confirmationNumber = _context.Requests.Where(s => s.RequestId == requestid).FirstOrDefault().ConfirmationNumber.ToUpper()
                                  
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



    }
}
