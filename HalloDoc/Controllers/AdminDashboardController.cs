﻿using BAL.Interface;
using BAL.Repository;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Identity;
using System.Collections;
using Microsoft.IdentityModel.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.Controllers
{
    [CustomAuthorize("Administrator")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProviders _provider;
        private readonly IAdminDashboard _admin;
        private readonly IAdminActions _adminActions;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IuploadFile _uploadfile;
        private readonly IPatientRequest _request;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher<AdminProfileVM> _passwordHasher;
        private readonly IPasswordHasher<PhysicianProfileVM> _passHasher;
        private readonly IUploadProvider _uploadProvider;

        public AdminDashboardController(ApplicationDbContext context, IAdminDashboard admin, IAdminActions action, IHostingEnvironment env, IuploadFile uploadfile, IPatientRequest request, IEmailService emailService, IPasswordHasher<AdminProfileVM> password,
                    IProviders providers, IUploadProvider upload, IPasswordHasher<PhysicianProfileVM> hasher)
        {
            _context = context;
            _admin = admin;
            _adminActions = action;
            _hostingEnvironment = env;
            _uploadfile = uploadfile;
            _request = request;
            _emailService = emailService;
            _passwordHasher = password;
            _provider = providers;
            _uploadProvider = upload;
            _passHasher = hasher;
        }

        public IActionResult MainPage()
        {
            var email = HttpContext.Session.GetString("Email");
            ViewBag.username = _admin.username(email);
            AdminMainPageVM MainModel = new AdminMainPageVM()
            {
                PageName = PageName.Dashboard
            };

            AdminDashboardVM vm = new AdminDashboardVM();

            MainModel.DashboardVM = AdminDashCallFromMain(vm, "1");
            if (!String.IsNullOrEmpty("1"))
            {
                ViewBag.Status = int.Parse("1");
            }
            return View(MainModel);
        }
        public AdminDashboardVM AdminDashCallFromMain(AdminDashboardVM model, string StatusButton)
        {
            var result = _admin.GetRequestsQuery(StatusButton);
            model.DashboardTableVM = result.ToList();
            model.NewCount = _admin.CountRequests("1");
            model.PendingCount = _admin.CountRequests("2");
            model.ActiveCount = _admin.CountRequests("3");
            model.ConcludeCount = _admin.CountRequests("4");
            model.ToCloseCount = _admin.CountRequests("5");
            model.UnpaidCount = _admin.CountRequests("6");
            model.Region = _admin.regions();
            return model;
        }

        // Filter action 

        public IActionResult SearchByName(string SearchString, string selectButton, string StatusButton, string SelectedStateId, string partialviewpath, int pagesize, int currentpage)
        {
            var result = _admin.GetRequestsQuery(StatusButton);
            result = result.Where(s => (String.IsNullOrEmpty(SearchString) || s.PatientName.Contains(SearchString)) && (String.IsNullOrEmpty(selectButton) || s.requestId == int.Parse(selectButton)) && ((SelectedStateId == "0" || SelectedStateId == null) || s.regionId == int.Parse(SelectedStateId)));
            


            int totalItems = result.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            if (SearchString != null || selectButton != null || SelectedStateId != "0")
            {
                if (totalPages <= 1)
                {
                    currentpage = 1;
                }
            }
            var paginatedData = result.ToList().Skip((currentpage - 1) * pagesize).Take(pagesize).ToList();
            if (result != null)
            {
                if (!String.IsNullOrEmpty(StatusButton))
                {
                    ViewBag.Status = int.Parse(StatusButton);
                    ViewBag.CurrentPage = currentpage;
                    ViewBag.TotalPages = totalPages;
                }
                return PartialView(partialviewpath, paginatedData);
            }
            return View("MainPage");
        }

        // Encounter Form Actions

        public IActionResult EncounterForm(int requestid)
        {
            AdminMainPageVM MainModel = new AdminMainPageVM
            {
                PageName = PageName.Encounterform
            };
            List<EncounterFormVM> model = _adminActions.getEncounterformdata(requestid);
            MainModel.encountermodel = model[0];
            return View("MainPage", MainModel);
        }

        [HttpPost]
        public IActionResult EncounterForm(EncounterFormVM model)
        {
            _adminActions.addencounterdata(model);
            return RedirectToAction("EncounterForm", new { requestid = model.requestid });
        }
        public IActionResult finalizeForm(int requestid)
        {
            bool result = _adminActions.finalize(requestid);
            if (result == true)
            {
                return RedirectToAction("MainPage");
            }
            return Ok("Can't finalize the form");
        }
        public IActionResult GeneratePdf(int requestid)
        {

            List<EncounterFormVM> viewEncounterForm = _adminActions.getEncounterformdata(requestid);

            if (viewEncounterForm == null)
            {
                return NotFound();
            }

            //return View("EncounterFormDetails", encounterFormView);
            return new ViewAsPdf("EncounterFormDetails", viewEncounterForm[0])
            {
                FileName = "Encounter_Form.pdf"
            };

        }

        // View Case Actions

        [HttpGet]
        public IActionResult ViewCaseAdmin()
        {
            var requestclientId = HttpContext.Request.Query["reqcliId"];
            AdminMainPageVM MainModel = new AdminMainPageVM()
            {
                PageName = PageName.ViewCaseForm
            };
            ViewCaseVM result = _adminActions.getViewCaseData(int.Parse(requestclientId)).FirstOrDefault();
            int requestid = _admin.getRequestIdbyRequestClientId(int.Parse(requestclientId));
            int status = _admin.getStatusByRequetId(requestid);
            result.Status = status;
            result.requestid = requestid;
            result.regiontable = _admin.regions();
            MainModel.Casemodel = result;
            return View("MainPage", MainModel);
        }

        // Close Case Actions

        public IActionResult CloseCase(int reqid)
        {
            AdminMainPageVM mainModel = new AdminMainPageVM();
            mainModel.PageName = PageName.CloseCase;
            CloseCaseVM model = new CloseCaseVM();
            model = _adminActions.closecasegetdata(model, reqid);
            mainModel.closecase = model;
            return View("MainPage", mainModel);
        }
        [HttpPost]
        public IActionResult CloseCase(CloseCaseVM model)
        {
            bool isclosed = _adminActions.closecase(model);
            if (isclosed == true)
            {
                TempData["Message"] = "Request Closed";
                TempData["MessageType"] = "success";
                return RedirectToAction("CloseCase", new { reqid = model.requestid });
            }
            TempData["Message"] = "Unable to close request";
            TempData["MessageType"] = "warning";
            return RedirectToAction("CloseCase", new { reqid = model.requestid });
        }

        public IActionResult ClosecasePost(int reqid)
        {
            var request = _admin.reqbyreqid(reqid);
            if (request != null)
            {
                _adminActions.closeRequest(request, reqid);
                TempData["Message"] = "closed Request";
                TempData["MessageType"] = "success";
                return RedirectToAction("MainPage");
            }
            TempData["Message"] = ".";
            TempData["MessageType"] = "warning";
            return RedirectToAction("MainPage");
        }

        // View Notes Actions

        public IActionResult ViewNotesAdminn(int reqid)
        {
            AdminMainPageVM MainModel = new AdminMainPageVM()
            {
                PageName = PageName.ViewNotes
            };
            var result = _adminActions.viewnotes(reqid);
            MainModel.NotesVM = result;
            return View("MainPage", MainModel);
        }
        public IActionResult ViewNotesAdmin()
        {
            var requestId = HttpContext.Request.Query["reqid"];
            AdminMainPageVM MainModel = new AdminMainPageVM()
            {
                PageName = PageName.ViewNotes
            };
            var result = _adminActions.viewnotes(int.Parse(requestId));
            MainModel.NotesVM = result;
            return View("MainPage", MainModel);
        }
        // Send Order Actions

        public IActionResult SendOrder(int requestid)
        {
            AdminMainPageVM MainModel = new AdminMainPageVM()
            {
                PageName = PageName.SendOrder
            };
            SendOrderVM SendOrderVM = new SendOrderVM();
            SendOrderVM.Professions = _adminActions.healthProfessionalTypes();
            SendOrderVM.requestid = requestid;
            MainModel.SendOrderVM = SendOrderVM;
            return View("MainPage", MainModel);
        }

        [HttpPost]
        public IActionResult SendOrder(SendOrderVM model)
        {
            if (ModelState.IsValid)
            {
                bool issend = _adminActions.sendOrder(model);
                if (issend == true)
                {
                    TempData["Message"] = "Successfully sent Your order!...";
                    TempData["MessageType"] = "success";
                    return RedirectToAction("MainPage");
                }
            }
            TempData["Message"] = "Unable to send order";
            TempData["MessageType"] = "warning";
            return RedirectToAction("MainPage");
        }


        // Assign Case Actions
        [HttpPost]
        public IActionResult CancelCase(int reeqid)
        {
            string Reason = Request.Form["Reason"];
            string Notes = Request.Form["Notes"];
            bool iscancel = _adminActions.changeStatusOnCancleCase(reeqid, Reason, Notes);
            if (iscancel)
            {
                TempData["Message"] = "success";
                TempData["MessageType"] = "success";
            }
            return RedirectToAction("MainPage");
        }

        public IActionResult filterPhyByRegion(string RegionId)
        {
            var physician = _adminActions.GetPhysicianByRegion(RegionId);
            return Json(physician);
        }

        [HttpPost]
        public IActionResult AssignCase(IFormCollection form)
        {

            string reeqid = form["requestid"];
            string physicianId = form["physicianId"];
            string Notes = form["Notes"];
            _adminActions.ChangeOnAssign(int.Parse(reeqid), int.Parse(physicianId), Notes);
            return RedirectToAction("MainPage");
        }

        // Block Request Actions

        public IActionResult BlockCase(int reeqid, string reason)
        {
            _adminActions.BlockCase(reeqid, reason);
            TempData["Message"] = "Blocked Successfully";
            TempData["MessageType"] = "success";
            return RedirectToAction("MainPage");
        }

        // View Document Actions

        public IActionResult ViewDocuments(int reeqid)
        {
            AdminMainPageVM mainmodel = new AdminMainPageVM
            {
                PageName = PageName.viewdocument
            };
            ViewdocumentVM model = new ViewdocumentVM();
            var requestfiles = _admin.filesbyrequestid(reeqid);
            model.RequestWiseFile = requestfiles;
            model.requestid = reeqid;
            //model.confirmationNumber = _context.Requests.Where(s => s.RequestId == reeqid).FirstOrDefault().ConfirmationNumber.ToUpper();
            mainmodel.DocumentVM = model;

            return View("MainPage", mainmodel);
        }

        [Obsolete]
        public IActionResult downloadfile(string filename)
        {
            string Filename = Path.GetFileName(filename);
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot\\uploads", Filename);

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
                string path = Path.Combine(this._hostingEnvironment.WebRootPath, "uploads");
                _uploadfile.uploadfile(file, fileName, path);

                _request.Addrequestwisefile(fileName, requestid);
                ViewBag.IsUpload = true;
                TempData["Message"] = "file uploaded successfully....";
                TempData["MessageType"] = "success";
                return RedirectToAction("ViewDocuments", new { reeqid = requestid });
            }
        }


        public IActionResult deleteFile(int reqid, string filename)
        {
            var requestwisefile = _admin.filebyReqidandName(reqid, filename);
            if (requestwisefile != null)
            {
                _admin.deleteSingleFile(requestwisefile);
                ViewBag.Isdelete = true;
                TempData["Message"] = "file deleted successfully....";
                TempData["MessageType"] = "success";
                return RedirectToAction("ViewDocuments", new { reeqid = reqid });
            }
            TempData["Message"] = "Unable To delete file";
            TempData["MessageType"] = "warning";
            return RedirectToAction("ViewDocuments", new { reeqid = reqid });
        }

        [HttpPost]
        public IActionResult deleteAllFiles(List<string> selectedFiles, string reqid)
        {
            bool isdeleteall = _admin.deleteAllFiles(selectedFiles);
            if (isdeleteall)
            {
                TempData["Message"] = "files deleted successfully....";
                TempData["MessageType"] = "success";
                return RedirectToAction("ViewDocuments", new { reeqid = reqid });
            }
            TempData["Message"] = "Can't find file or unable to delete the file";
            TempData["MessageType"] = "warning";
            return RedirectToAction("ViewDocuments", new { reeqid = reqid });
        }

        public IActionResult SendEmailWithAttachments(List<string> selectedFilesPath, string email, string reqid)
        {
            try
            {
                string to = email;
                string subject = "Sending your documents";
                string body = "body";
                _emailService.SendMailWithAttachments(to, subject, body, selectedFilesPath);

                ViewBag.PatientMailalert = true;
                TempData["Message"] = "Files successfully send by mail";
                TempData["MessageType"] = "success";
                return RedirectToAction("ViewDocuments", new { reeqid = reqid });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // View Notes

        public IActionResult filterVenByPro(string ProfessionId)
        {
            var list = _adminActions.getVenbypro(ProfessionId);
            return Json(list);
        }
        public IActionResult getvendordata(string businessId)
        {
            var data = _admin.getprofessionalsbyvendorid(businessId);
            return Json(data);
        }

        [HttpPost]
        public IActionResult addAdminnote(ViewNotesVM model, int reqid)
        {
            RequestNote notes = _adminActions.reqnotebyreqid(reqid);
            if (notes != null)
            {
                _adminActions.addrequnotes(model, notes);
                return RedirectToAction("ViewNotesAdminn", new { reqid = reqid });
            }
            return RedirectToAction("ViewNotesAdminn", new { reqid = reqid });
        }
        public IActionResult TransferNotes(int reeqid)
        {
            int phyid = int.Parse(Request.Form["physicianId"]);
            string transNote = Request.Form["Notes"];

            if (_adminActions.transferNotes(reeqid, phyid, transNote) == true)
            {
                return RedirectToAction("MainPage");
            }
            else
            {
                return Ok("Cannot Add TransferNotes");
            }
        }

        public IActionResult ClearCaseee(int reqid)
        {
            bool result = _adminActions.ClearCase(reqid);

            if (result == true)
            {
                return Ok("Successfully Cleared the Case");
            }
            else
            {
                return Ok("Can't Clear the case");
            }
        }

        [HttpPost]
        public IActionResult SendAgreement(int requestid)
        {

            string token = Guid.NewGuid().ToString() + ":" + requestid.ToString() + ":" + DateTime.Now.ToString();
            var link = Url.Action("ReviewAgreement", "PatientDashBoard", new { token = token }, protocol: HttpContext.Request.Scheme);

            string to = _adminActions.clientsbyreqid(requestid).First().Email;
            to = "priyank.patadiya@etatvasoft.com";
            string subject = "Agreement";
            var body = new StringBuilder();
            body.AppendFormat("Hello");
            body.AppendLine(@"Click below link to review the agreement");
            body.AppendLine("<a href=\"" + link + "\">Click here to continue the process</a>");

            string Body = body.ToString();

            if (to != null)
            {
                TempData["Message"] = "Aggrement Sent";
                TempData["MessageType"] = "success";
                _emailService.SendEmail(to, subject, Body);
                return RedirectToAction("MainPage");
            }
            TempData["Message"] = "can't send Aggrement";
            TempData["MessageType"] = "warning";
            return RedirectToAction("MainPage");
        }

        // Admin Profile

        public IActionResult AdminProfile()
        {
            string email = HttpContext.Session.GetString("Email");
            ViewBag.username = _admin.username(email);
            var admin = _admin.getProfileData(email);

            return View(admin);
        }

        public IActionResult changeAccInfo(AdminProfileVM model, List<string> regions)
        {
            string email = HttpContext.Session.GetString("Email");

            if (email != "")
            {
                _admin.changeAccountInfo(model, email, regions);
                HttpContext.Session.SetString("Email", email);
                TempData["Message"] = "Edited Successfully";
                TempData["MessageType"] = "success";
                return RedirectToAction("AdminProfile");
            }
            TempData["Message"] = "Not able to change information";
            TempData["MessageType"] = "success";
            return RedirectToAction("AdminProfile");
        }

        public IActionResult changeBillingInfo(AdminProfileVM model)
        {
            string email = HttpContext.Session.GetString("Email");
            if (email != "")
            {
                _admin.changeBillingInfo(model, email);
                TempData["Message"] = "Edited Successfully";
                TempData["MessageType"] = "success";
                return RedirectToAction("AdminProfile");
            }
            TempData["Message"] = "Not able to change information";
            TempData["MessageType"] = "success";
            return RedirectToAction("AdminProfile");
        }

        public IActionResult changePass([FromForm] string Password)
        {
            string email = HttpContext.Session.GetString("Email");
            Password = _passwordHasher.HashPassword(null, Password);
            if (email != "")
            {
                _admin.changePassword(email, Password);
                TempData["Message"] = "Password Changed....";
                TempData["MessageType"] = "success";
                return RedirectToAction("PatientLoginn", "Home");
            }
            TempData["Message"] = "Not Able to change password";
            TempData["MessageType"] = "warning";
            return RedirectToAction("AdminProfile");
        }


        // Send Link

        public IActionResult sendLinkofSubmitreq(string PatientFirstname, string PatientLastname, string PatientEmail)
        {
            var link = Url.ActionLink("SubmitRequest", "Home", protocol: HttpContext.Request.Scheme);

            string to = PatientEmail;
            string subject = "Submit A Request To Connect With Our Physicians";
            var body = new StringBuilder();
            body.AppendFormat("Hello");
            body.AppendLine(@"Please Submit Your Request here");
            body.AppendLine("<a href=\"" + link + "\">Click here</a>");

            string Body = body.ToString();

            if (to != null)
            {
                TempData["Message"] = "email Sent Successfully";
                TempData["MessageType"] = "success";
                _emailService.SendEmail(to, subject, Body);
                return RedirectToAction("MainPage");
            }
            TempData["Message"] = "Can't Send Email";
            TempData["MessageType"] = "warning";
            return RedirectToAction("MainPage");
        }

        // Create Request

        public IActionResult CreateRequestAdmin()
        {
            PatientReqVM model = new PatientReqVM();
            model.Region = _admin.regions();
            return PartialView("CreateRequestAdmin", model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequestAdmin(PatientReqVM model)
        {
            var email = HttpContext.Session.GetString("Email");
            if (ModelState.IsValid)
            {
                model.State = await _request.GetStateAccordingToRegionId(model.SelectedStateId);
                _request.AddAdminCreateRequest(model, email);
                TempData["Message"] = "Request Added!";
                TempData["MessageType"] = "success";
                return RedirectToAction("MainPage");
            }

            model.Region = _admin.regions();
            return PartialView("CreateRequestAdmin", model);

        }

        // Provider Menu

        public IActionResult Provider()
        {
            ProviderMenuVM model = new ProviderMenuVM();
            model.regions = _admin.regions();
            return View("ProviderMenu/Provider", model);
        }

        public IActionResult filterProviderTable(string stateid)
        {
            var result = _provider.getfilteredPhysicians(int.Parse(stateid)).ToList();
            return PartialView("ProviderMenu/_ProviderPartialTable", result);
        }


        public IActionResult ProviderProfile(int id)
        {
            Physician? physician = _provider.getPhysicianById(id);

            PhysicianProfileVM physicanProfile = new PhysicianProfileVM();
            physicanProfile.FirstName = physician.FirstName;
            physicanProfile.LastName = physician.LastName ?? "";
            physicanProfile.Email = physician.Email;
            physicanProfile.Address1 = physician.Address1 ?? "";
            physicanProfile.Address2 = physician.Address2 ?? "";
            physicanProfile.City = physician.City ?? "";
            physicanProfile.ZipCode = physician.Zip ?? "";
            physicanProfile.MobileNo = physician.Mobile ?? "";
            physicanProfile.Regions = _admin.regions();
            physicanProfile.MedicalLicense = physician.MedicalLicense;
            physicanProfile.NPINumber = physician.Npinumber;
            physicanProfile.SynchronizationEmail = physician.SyncEmailAddress;
            physicanProfile.physicianid = physician.PhysicianId;
            physicanProfile.WorkingRegions = _provider.getList(physician.PhysicianId);
            physicanProfile.State = physician.RegionId;
            physicanProfile.SignatureFilename = physician.Signature;
            physicanProfile.BusinessWebsite = physician.BusinessWebsite;
            physicanProfile.BusinessName = physician.BusinessName;
            physicanProfile.PhotoFileName = physician.Photo;
            physicanProfile.IsAgreement = physician.IsAgreementDoc;
            physicanProfile.IsBackground = physician.IsBackgroundDoc;
            physicanProfile.IsHippa = physician.IsAgreementDoc;
            physicanProfile.NonDiscoluser = physician.IsNonDisclosureDoc;
            physicanProfile.License = physician.IsLicenseDoc;
            return View("ProviderMenu/ProviderProfile", physicanProfile);
        }

        public IActionResult ResetPhysicianPassword(string Password, int physicianid)
        {

            Physician? physician = _provider.getPhysicianById(physicianid);
            AspNetUser? account = _provider.getAccByEmail(physician.Email);


            if (account != null && Password != null)
            {
                string passwordhash = _passwordHasher.HashPassword(null, Password);
                _provider.UpdatePassword(account, passwordhash);
                TempData["Message"] = "Password Changed Successfully!";
                TempData["MessageType"] = "success";

            }
            else
            {
                TempData["Message"] = "Can't Change Password!";
                TempData["MessageType"] = "success";
            }

            return RedirectToAction("ProviderProfile", new { id = physicianid });
        }

        public IActionResult PhysicianInformation(int id, string MobileNo, string[] Region, string SynchronizationEmail, string NPINumber, string MedicalLicense)
        {
            Physician? physician = _provider.getPhysicianById(id);
            AspNetUser? account = _provider.getAccByEmail(physician.Email);
            string[] regions = Region;
            if (physician != null)
            {
                _provider.updatePhysicianInfo(physician, MobileNo, regions, SynchronizationEmail, NPINumber, MedicalLicense);
            }
            else
            {

            }
            return RedirectToAction("ProviderProfile", new { id = id });

        }


        public IActionResult MailingBillingInformationProvider(int physicianid, string MobileNo, string Address1, string Address2, string City, int State, string Zipcode)
        {

            Physician? physician = _provider.getPhysicianById(physicianid);
            if (physician != null)
            {
                physician.Address1 = Address1;
                physician.Address2 = Address2;
                physician.City = City;
                physician.Mobile = MobileNo;
                physician.RegionId = State;
                physician.Zip = Zipcode;
                _provider.updateBilling(physician);

            }
            else
            {

            }
            return RedirectToAction("ProviderProfile", new { id = physicianid });
        }

        public IActionResult Physicianprofile(int id, string businessName, string businessWebsite, IFormFile signatureFile, IFormFile photoFile)
        {
            try
            {
                _admin.UpdateProviderProfile(id, businessName, businessWebsite, signatureFile, photoFile);

            }
            catch (InvalidOperationException ex)
            {

                Console.WriteLine(ex.Message);
            }

            return RedirectToAction("ProviderProfile", new { id = id });
        }

        [HttpPost]
        public IActionResult SaveSignatureImage(IFormFile signatureImage, int id)
        {

            if (signatureImage != null && signatureImage.Length > 0)
            {
                string fileName = _uploadProvider.UploadSignature(signatureImage, id);
                var physician = _provider.getPhysicianById(id);
                physician.Signature = fileName;
                _provider.updateBilling(physician);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult UploadDocs(string fileName, IFormFile File, int physicianid)
        {
            Physician? physician = _provider.getPhysicianById(physicianid);
            if (physician != null)
            {

                if (fileName == "ICA")
                {
                    var docfile = _uploadProvider.UploadDocFile(File, physicianid, fileName);
                    physician.IsAgreementDoc = new BitArray(new[] { true });
                }
                if (fileName == "Background")
                {
                    var docfile = _uploadProvider.UploadDocFile(File, physicianid, fileName);
                    physician.IsBackgroundDoc = new BitArray(new[] { true });
                }
                if (fileName == "Hippa")
                {
                    var docfile = _uploadProvider.UploadDocFile(File, physicianid, fileName);
                    physician.IsTrainingDoc = new BitArray(new[] { true });
                }
                if (fileName == "NonDiscoluser")
                {
                    var docfile = _uploadProvider.UploadDocFile(File, physicianid, fileName);
                    physician.IsNonDisclosureDoc = new BitArray(new[] { true });
                }
                if (fileName == "License")
                {
                    var docfile = _uploadProvider.UploadDocFile(File, physicianid, fileName);
                    physician.IsLicenseDoc = new BitArray(new[] { true });
                }
                _provider.updateBilling(physician);
                return Ok();
            }
            else
            {
                return BadRequest("No Doc File received.");
            }
        }

        public IActionResult CreateProviderAcc()
        {
            PhysicianProfileVM model = new PhysicianProfileVM();
            model.Regions = _admin.regions();
            return View("ProviderMenu/CreateProviderAccount", model);
        }

        [HttpPost]
        public IActionResult CreateProviderAcc(PhysicianProfileVM model, string[] Region)
        {
            if (ModelState.IsValid)
            {
                AspNetUser user = new AspNetUser();
                user.Id = Guid.NewGuid().ToString();
                user.UserName = model.FirstName + model.LastName;
                user.PasswordHash = _passHasher.HashPassword(null, model.Password);
                user.Email = model.Username;
                user.PhoneNumber = model.MobileNo;
                user.CreatedDate = DateTime.Now;
                _context.AspNetUsers.Add(user);
                _context.SaveChanges();

                var aspnetuserrole = new AspNetUserRole
                {
                    UserId = user.Id,
                    RoleId = 3
                };
                _context.AspNetUserRoles.Add(aspnetuserrole);
                _context.SaveChanges();

                Physician physician = new Physician();
                physician.AspNetUserId = user.Id;
                physician.FirstName = model.FirstName;
                physician.LastName = model.LastName;
                physician.Email = model.Username;
                physician.Mobile = model.MobileNo;
                physician.MedicalLicense = model.MedicalLicense;
                physician.IsCredentialDoc = new BitArray(new[] { false });
                physician.IsLicenseDoc = new BitArray(new[] { false });
                physician.Address1 = model.Address1;
                physician.Address2 = model.Address2;
                physician.City = model.City;
                physician.RegionId = model.State;
                physician.Zip = model.ZipCode;
                physician.CreatedDate = DateTime.Now;
                physician.Status = 1;
                physician.BusinessName = model.BusinessName;
                physician.BusinessWebsite = model.BusinessWebsite;
                physician.RoleId = 2;
                physician.Npinumber = model.NPINumber;
                physician.Signature = model.SignatureFilename;
                _context.Physicians.Add(physician);
                _context.SaveChanges();
                
                if (model.File != null)
                {
                    _uploadProvider.UploadPhoto(model.File, physician.PhysicianId);
                    physician.Photo = model.File.FileName;
                }

                if (model.ICAFile != null)
                {
                    _uploadProvider.UploadDocFile(model.File, physician.PhysicianId, "ICA");
                    physician.IsAgreementDoc = new BitArray(new[] { true });
                }
                else
                {
                    physician.IsAgreementDoc = new BitArray(new[] { false });
                }
                if (model.BackFile != null)
                {
                    _uploadProvider.UploadDocFile(model.BackFile, physician.PhysicianId, "BackDoc");
                    physician.IsBackgroundDoc = new BitArray(new[] { true });
                }
                else
                {
                    physician.IsBackgroundDoc = new BitArray(new[] { false });
                }
                if (model.HippaFile != null)
                {
                    _uploadProvider.UploadDocFile(model.HippaFile, physician.PhysicianId, "TrainingDoc");
                    physician.IsTrainingDoc = new BitArray(new[] { true });
                }
                else
                {
                    physician.IsTrainingDoc = new BitArray(new[] { false });
                }
                if (model.NonFile != null)
                {
                    _uploadProvider.UploadDocFile(model.NonFile, physician.PhysicianId, "NonDisclosureDoc");
                    physician.IsNonDisclosureDoc = new BitArray(new[] { true });
                }
                else
                {
                    physician.IsNonDisclosureDoc = new BitArray(new[] { false });
                }
                _context.Physicians.Update(physician);
                _context.SaveChanges();

                foreach (var item in Region)
                {
                    PhysicianRegion physicianRegion = new PhysicianRegion();
                    physicianRegion.PhysicianId = physician.PhysicianId;
                    physicianRegion.RegionId = int.Parse(item);
                    _context.Add(physicianRegion);
                    _context.SaveChanges();
                }

                PhysicianNotification notification = new PhysicianNotification
                {
                    PhysicianId = physician.PhysicianId,
                    IsNotificationStopped = new BitArray(new[] { false })
                };
                _context.PhysicianNotifications.Add( notification );
                _context.SaveChanges();


            }
            return RedirectToAction("Provider");
        }

        public IActionResult changeNotification(bool isChecked, string id)
        {
            var phynoty = _context.PhysicianNotifications.Where(u => u.PhysicianId == int.Parse(id)).FirstOrDefault();
            if(phynoty != null)
            {
                if(isChecked == true)
                {
                    phynoty.IsNotificationStopped = new BitArray(new[] { true });
                }
                else
                {
                    phynoty.IsNotificationStopped = new BitArray(new[] { false });
                }

                _context.PhysicianNotifications.Update(phynoty);
                _context.SaveChanges();
                return RedirectToAction("Provider");
            }
            return RedirectToAction("Provider");
        }
    }
}
