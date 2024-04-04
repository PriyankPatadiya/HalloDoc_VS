using BAL.Interface;
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
using OfficeOpenXml;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.String;
using System.Drawing;
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
        private readonly IPasswordHasher<AdminCreateAccVM> _passAdminHasher;
        private readonly IUploadProvider _uploadProvider;
        private readonly IAccessMenu _accessMenu;

        public AdminDashboardController(ApplicationDbContext context, IAdminDashboard admin, IAdminActions action, IHostingEnvironment env, IuploadFile uploadfile, IPatientRequest request, IEmailService emailService, IPasswordHasher<AdminProfileVM> password,
                    IProviders providers, IUploadProvider upload, IPasswordHasher<PhysicianProfileVM> hasher, IAccessMenu menu, IPasswordHasher<AdminCreateAccVM> hasherr)
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
            _accessMenu = menu;
            _passAdminHasher = hasherr;
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
            result = result.Where(s => (String.IsNullOrEmpty(SearchString) || s.PatientName.Contains(SearchString)) && (System.String.IsNullOrEmpty(selectButton) || s.requestId == int.Parse(selectButton)) && ((SelectedStateId == "0" || SelectedStateId == null) || s.regionId == int.Parse(SelectedStateId)));



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
            if (paginatedData.Count != 0)
            {
                if (!String.IsNullOrEmpty(StatusButton))
                {
                    ViewBag.Status = int.Parse(StatusButton);
                    ViewBag.CurrentPage = currentpage;
                    ViewBag.TotalPages = totalPages;
                }
                return PartialView(partialviewpath, paginatedData);
            }
            else
            {
                ViewBag.EmptyMessage = "There is No data to show you....";
                return PartialView(partialviewpath);
            }
        }

        // Export

        public IActionResult exportfile(string StatusButton, int pagesize, int currentpage)
        {
            var result = _admin.GetRequestsQuery(StatusButton);
            result = result.Skip((currentpage - 1) * pagesize).Take(pagesize);

            using (var excel = new ExcelPackage())
            {
                var worksheet = excel.Workbook.Worksheets.Add("sheet1");
                worksheet.Cells[1, 1].Value = "PatientName";
                worksheet.Cells[1, 2].Value = "BirthDate";
                worksheet.Cells[1, 3].Value = "RequestorName";
                worksheet.Cells[1, 4].Value = "RequestDate";
                worksheet.Cells[1, 5].Value = "phone";
                worksheet.Cells[1, 6].Value = "address";
                worksheet.Cells[1, 7].Value = "Email";

                var row = 2;
                foreach (var item in result)
                {
                    worksheet.Cells[row, 1].Value = item.physicianname;
                    worksheet.Cells[row, 2].Value = item.BirthDate;
                    worksheet.Cells[row, 3].Value = item.RequestorName;
                    worksheet.Cells[row, 4].Value = item.RequestDate;
                    worksheet.Cells[row, 5].Value = item.phone;
                    worksheet.Cells[row, 6].Value = item.address;
                    worksheet.Cells[row, 7].Value = item.Email;
                    row++;
                }

                var excelBytes = excel.GetAsByteArray();
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export.xlsx");
            }
        }

        public IActionResult exportAll(string StatusButton)
        {
            var result = _admin.GetRequestsQuery(StatusButton);
            using (var excel = new ExcelPackage())
            {
                var worksheet = excel.Workbook.Worksheets.Add("sheet1");
                worksheet.Cells[1, 1].Value = "PatientName";
                worksheet.Cells[1, 2].Value = "BirthDate";
                worksheet.Cells[1, 3].Value = "RequestorName";
                worksheet.Cells[1, 4].Value = "RequestDate";
                worksheet.Cells[1, 5].Value = "phone";
                worksheet.Cells[1, 6].Value = "address";
                worksheet.Cells[1, 7].Value = "Email";

                var row = 2;
                foreach (var item in result)
                {
                    worksheet.Cells[row, 1].Value = item.physicianname;
                    worksheet.Cells[row, 2].Value = item.BirthDate;
                    worksheet.Cells[row, 3].Value = item.RequestorName;
                    worksheet.Cells[row, 4].Value = item.RequestDate;
                    worksheet.Cells[row, 5].Value = item.phone;
                    worksheet.Cells[row, 6].Value = item.address;
                    worksheet.Cells[row, 7].Value = item.Email;
                    row++;
                }

                var excelBytes = excel.GetAsByteArray();
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "exportAll.xlsx");
            }
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
        public IActionResult CancelCase(int requestid)
        {
            string Reason = Request.Form["Reason"];
            string Notes = Request.Form["Notes"];
            bool iscancel = _adminActions.changeStatusOnCancleCase(requestid, Reason, Notes);
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
            var email = HttpContext.Session.GetString("Email");
            ViewBag.username = _admin.username(email);
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
            var email = HttpContext.Session.GetString("Email");
            ViewBag.username = _admin.username(email);

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

        public IActionResult CreateProviderAcc(int id)
        {
            var email = HttpContext.Session.GetString("Email");
            ViewBag.username = _admin.username(email);
            PhysicianProfileVM model = new PhysicianProfileVM();
            if (id == 1)
            {
                model.isFromUserAccess = true;
            }
            else
            {
                model.isFromUserAccess = false;
            }
            model.Regions = _admin.regions();
            return View("ProviderMenu/CreateProviderAccount", model);
        }

        [HttpPost]
        public IActionResult CreateProviderAcc(PhysicianProfileVM model, string[] Region)
        {
            if (ModelState.IsValid)
            {
                string hashedpass = _passHasher.HashPassword(null, model.Password);
                _provider.createproviderAcc(model, Region, hashedpass);
                return RedirectToAction("Provider");
            }
            model.Regions = _admin.regions();
            return View("ProviderMenu/CreateProviderAccount", model);
        }

        public IActionResult changeNotification(bool isChecked, string id)
        {
            bool ans = _provider.changeNotification(isChecked, id);
            if (ans)
            {
                TempData["Message"] = "NotificationChanged";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = "can't change notification!";
                TempData["MessageType"] = "error";
            }
            return RedirectToAction("Provider");
        }


        // Access 

        public IActionResult userAccess()
        {
            var email = HttpContext.Session.GetString("Email");
            ViewBag.username = _admin.username(email);
            return View("AccessMenu/UserAccess");
        }

        public IActionResult filterUserAccessTable(string AccTypeId)
        {
            var result = _accessMenu.getUserAccessData(int.Parse(AccTypeId));
            return PartialView("AccessMenu/_UserAccessPartial", result);
        }

        public IActionResult CreateAdminAcc()
        {
            var email = HttpContext.Session.GetString("Email");
            ViewBag.username = _admin.username(email);
            AdminCreateAccVM model = new AdminCreateAccVM();
            model.Regions = _admin.regions();
            return View("ProviderMenu/CreateAdminAccount", model);
        }

        [HttpPost]
        public IActionResult CreateAdminAcc(AdminCreateAccVM model, int[] adminRegion)
        {
            if (ModelState.IsValid)
            {
                string hashedPassword = _passAdminHasher.HashPassword(null, model.Password);
                _provider.createAdminAcc(model, hashedPassword, adminRegion);
                return RedirectToAction("userAccess");
            }
            model.Regions = _admin.regions();
            return View("ProviderMenu/CreateAdminAccount", model);
        }


        // Role Access

        public IActionResult roleAccess()
        {
            var email = HttpContext.Session.GetString("Email");
            ViewBag.username = _admin.username(email);
            var roles = _context.Roles.ToList();
            var list = roles.Where(item => item.IsDeleted != null && (item.IsDeleted.Length == 0 || !item.IsDeleted[0]));
            return View("AccessMenu/RoleAccess", list.ToList());
        }


        public IActionResult CreateAccess()
        {
            var email = HttpContext.Session.GetString("Email");
            ViewBag.username = _admin.username(email);
            return View("AccessMenu/CreateAccess");
        }

        public IActionResult GetRoles(int role)
        {
            var menu = _context.Menus.Where(item => role == 0 || item.AccountType == role).ToList();
            return PartialView("AccessMenu/_CreateAccessRole", menu);
        }


        [HttpPost]
        public IActionResult CreateAccess(int[] rolemenu, string rolename, int accounttype)
        {
            Role role = new Role();
            role.Name = rolename;
            role.AccountType = (short)accounttype;
            role.CreatedBy = "admin";
            role.CreatedDate = DateTime.Now;
            role.IsDeleted = new BitArray(new[] { false });
            _context.Roles.Add(role);
            _context.SaveChanges();

            foreach (var menu in rolemenu)
            {
                RoleMenu rolemenu1 = new RoleMenu();
                rolemenu1.MenuId = menu;
                rolemenu1.RoleId = role.RoleId;
                _context.RoleMenus.Add(rolemenu1);
                _context.SaveChanges();
            }
            TempData["Message"] = "Created Successfully!";
            TempData["MessageType"] = "success";
            return RedirectToAction("roleAccess");
        }
        public IActionResult EditAccess(int roleid)
        {
            var role = _context.Roles.Where(u => u.RoleId == roleid).First();
            EditRoleAccessVM model = new EditRoleAccessVM();
            model.roleid = roleid;
            model.Name = role.Name;
            model.accountType = role.AccountType;
            model.Menus = _context.Menus.Where(u => u.AccountType == role.AccountType).ToList();
            model.selectedmenus = _context.RoleMenus.Where(u => u.RoleId == roleid).ToList();
            return View("AccessMenu/EditRole", model);
        }

        [HttpPost]
        public IActionResult EditAccess(int[] rolemenu, string rolename, int roleid)
        {
            if (roleid != null)
            {
                var role = _context.Roles.Where(u => u.RoleId == roleid).First();
                role.Name = rolename;
                role.ModifiedDate = DateTime.Now;
                role.ModifiedBy = rolename;
                _context.Roles.Update(role);
                _context.SaveChanges();

                var prevRoleMenu = _context.RoleMenus.Where(u => u.RoleId == roleid).ToList();
                _context.RoleMenus.RemoveRange(prevRoleMenu);
                _context.SaveChanges();
                foreach (var menu in rolemenu)
                {
                    RoleMenu rolemenu1 = new RoleMenu();
                    rolemenu1.MenuId = menu;
                    rolemenu1.RoleId = role.RoleId;
                    _context.RoleMenus.Add(rolemenu1);
                    _context.SaveChanges();
                }
            }
            TempData["Message"] = "Edited Successfully!";
            TempData["MessageType"] = "success";
            return RedirectToAction("roleAccess");
        }

        public IActionResult DeleteRole(int roleid)
        {
            if (roleid != null)
            {
                var role = _context.Roles.Where(u => u.RoleId == roleid).First();
                var prevRoleMenu = _context.RoleMenus.Where(u => u.RoleId == roleid).ToList();
                _context.RoleMenus.RemoveRange(prevRoleMenu);
                _context.Roles.Remove(role);
                _context.SaveChanges();
                TempData["Message"] = "Removed Successfully!";
                TempData["MessageType"] = "success";

            }
            else
            {
                TempData["Message"] = "Can't Remove!";
                TempData["MessageType"] = "error";
            }

            return RedirectToAction("roleAccess");
        }

        // location 

        public IActionResult ProviderLocation()
        {
            return View();
        }
        public IActionResult getPhysicianMapDetail()
        {
            List<PhysicianLocation> physicianLocations = _context.PhysicianLocations.ToList();
            return Json(physicianLocations);
        }

        // Scheduling

        public IActionResult Scheduling()
        {
            var email = HttpContext.Session.GetString("Email");
            ViewBag.username = _admin.username(email);
            SchedulingVM model = new SchedulingVM();
            model.Region = _admin.regions();
            return View("Scheduling/Scheduling", model);
        }


        public IActionResult GetPhysicianShift(string region)
        {
            var physicians = (from physician in _context.Physicians
                              where int.Parse(region) == 0 || physician.RegionId == int.Parse(region)
                              select physician)
                             .ToList();
            return Json(physicians);
        }


        #region Scheduling

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateShift(SchedulingVM model)
        {
            if (ModelState.IsValid)
            {
                var email = HttpContext.Session.GetString("Email");
                var admin = _context.Admins.First(u => u.Email == email);

                bool isShiftExist = _context.ShiftDetails.Any(sD =>
                                    sD.Shift.PhysicianId == model.Physicianid &&
                                    sD.ShiftDate.Date == model.Startdate.Date && // Shifts must start on the same day
                                    ((sD.StartTime < model.Endtime && sD.EndTime > model.Starttime) || // Check for overlap
                                    (sD.StartTime < model.Starttime && sD.EndTime > model.Starttime) ||
                                    (sD.StartTime < model.Endtime && sD.EndTime > model.Endtime) ||
                                    (sD.StartTime > model.Starttime && sD.EndTime < model.Endtime)));


                if (!isShiftExist)
                {
                    Shift shift = new Shift();
                    shift.PhysicianId = model.Physicianid;
                    shift.StartDate = model.Startdate;
                    shift.IsRepeat = new BitArray(new[] { model.Isrepeat });
                    shift.RepeatUpto = model.Repeatupto;
                    shift.CreatedDate = DateTime.Now;
                    shift.CreatedBy = admin.AspNetUserId;
                    _context.Shifts.Add(shift);
                    _context.SaveChanges();

                    ShiftDetail sd = new ShiftDetail();
                    sd.ShiftId = shift.ShiftId;
                    sd.ShiftDate = new DateTime(model.Startdate.Year, model.Startdate.Month, model.Startdate.Day);
                    sd.StartTime = model.Starttime;
                    sd.EndTime = model.Endtime;
                    sd.RegionId = model.Regionid;
                    sd.Status = model.Status;
                    sd.IsDeleted = new BitArray(new[] { false });
                    _context.ShiftDetails.Add(sd);
                    _context.SaveChanges();

                    ShiftDetailRegion sr = new ShiftDetailRegion();
                    sr.ShiftDetailId = sd.ShiftDetailId;
                    sr.RegionId = (int)model.Regionid;
                    sr.IsDeleted = new BitArray(new[] { false });
                    _context.ShiftDetailRegions.Add(sr);
                    _context.SaveChanges();

                    if (model.Isrepeat)
                    {
                        var stringArray = model.checkWeekday.Split(",");
                        foreach (var weekday in stringArray)
                        {

                            DateTime startDateForWeekday = model.Startdate.AddDays((7 + int.Parse(weekday) - (int)model.Startdate.DayOfWeek) % 7);

                            if (startDateForWeekday < model.Startdate.Date.Add(DateTime.Now.TimeOfDay))
                            {
                                startDateForWeekday = startDateForWeekday.AddDays(7);
                            }


                            for (int i = 0; i < shift.RepeatUpto; i++)
                            {

                                ShiftDetail shiftDetail = new ShiftDetail
                                {
                                    ShiftId = shift.ShiftId,
                                    ShiftDate = startDateForWeekday.AddDays(i * 7),
                                    RegionId = (int)model.Regionid,
                                    StartTime = model.Starttime,
                                    EndTime = model.Endtime,
                                    Status = 0,
                                    IsDeleted = new BitArray(new[] { false })
                                };

                                _context.Add(shiftDetail);
                                _context.SaveChanges();
                            }
                        }
                    }

                    return RedirectToAction("Scheduling");

                }
                else
                {
                    return BadRequest("Physician is Already in shift for entered Time");
                }
            }
            return RedirectToAction("Scheduling");
        }

        public IActionResult getEvents(int regionId)
        {
            var events = (from s in _context.Shifts
                          join pd in _context.Physicians on s.PhysicianId equals pd.PhysicianId
                          join sd in _context.ShiftDetails on s.ShiftId equals sd.ShiftId into shiftGroup
                          from sd in shiftGroup.DefaultIfEmpty()

                          select new SchedulingVM
                          {
                              title = string.Concat(sd.StartTime, " ", sd.EndTime, " ", pd.FirstName, " ", pd.LastName),
                              Shiftid = sd.ShiftDetailId,
                              Startdate = sd.ShiftDate.Date.Add(sd.StartTime.ToTimeSpan()),
                              Enddate = sd.ShiftDate.Date.Add(sd.EndTime.ToTimeSpan()),
                              Status = sd.Status,
                              Physicianid = pd.PhysicianId,
                              PhysicianName = pd.FirstName + ' ' + pd.LastName,
                              Shiftdate = sd.ShiftDate,
                              ShiftDetailId = sd.ShiftDetailId,
                              Regionid = sd.RegionId,
                              ShiftDeleted = sd.IsDeleted[0]

                          }).Where(item => regionId == 0 || item.Regionid == regionId).ToList();
            events = events.Where(item => !item.ShiftDeleted).ToList();

            return Json(events);
        }

        [HttpPost]
        public IActionResult saveShiftChanges(int shiftDetailId, DateTime startDate, TimeOnly startTime, TimeOnly endTime, int region)
        {
            // Find the shift detail by its ID
            ShiftDetail shiftdetail = _context.ShiftDetails.Find(shiftDetailId);

            // If shift detail is not found, return a 404 Not Found response
            if (shiftdetail == null)
            {
                return BadRequest();
            }
            try
            {
                // Update the shift detail properties
                shiftdetail.ShiftDate = startDate;
                shiftdetail.StartTime = startTime;
                shiftdetail.EndTime = endTime;

                // Update the database
                _context.ShiftDetails.Update(shiftdetail);
                _context.SaveChanges();
                var events = _adminActions.getEvents(region);
                return Ok(new { message = "Shift detail updated successfully.", events = events });
            }
            catch (Exception ex)
            {
                // Return a 400 Bad Request response with the error message
                return BadRequest("Error updating shift detail: " + ex.Message);
            }
        }
        public IActionResult DeleteShift(int shiftDetailId, int region)
        {
            ShiftDetail shiftdetail = _context.ShiftDetails.Find(shiftDetailId);
            if (shiftdetail == null)
            {
                return NotFound("Shift detail not found.");
            }
            shiftdetail.IsDeleted = new BitArray(new[] { true });
            _context.ShiftDetails.Update(shiftdetail);
            _context.SaveChanges();
            var events = _adminActions.getEvents(region);

            return Ok(new { message = "Shift detail Deleted successfully.", events = events });

        }

        public IActionResult changeShiftStatus(int shiftDetailId, int region)
        {
            ShiftDetail shiftDetail = _context.ShiftDetails.Find(shiftDetailId);
            if (shiftDetail == null)
            {
                return NotFound("Shift detail not found.");
            }
            if (shiftDetail.Status == 0)
            {
                shiftDetail.Status = 1;
            }
            else
            {
                shiftDetail.Status = 0;
            }
            _context.ShiftDetails.Update(shiftDetail); _context.SaveChanges();
            var events = _adminActions.getEvents(region);

            return Ok(new { message = "Shift Returned successfully.", events = events });
        }
        public IActionResult requestedShifts()
        {
            ViewBag.Regions = _admin.regions();
            return View("Scheduling/shiftForReview");
        }

        [HttpGet]
        public IActionResult changeShiftReviewTable(int region)
        {
            List<ShiftReviewVM> sdList = (from sd in _context.ShiftDetails
                                          where ((sd.RegionId == region || region == 0) &&
                                          sd.Status != 0 && sd.IsDeleted != new BitArray(new[] { true }))
                                          select new ShiftReviewVM()
                                          {
                                              shiftDetailId = sd.ShiftDetailId,
                                              physicianName = _context.Shifts.First(u => u.ShiftId == sd.ShiftId).Physician.FirstName,
                                              Regioname = _context.Regions.First(u => u.RegionId == sd.RegionId).Name,
                                              day = sd.ShiftDate.ToString("MMMM dd, yyyy"),
                                              startTime = sd.StartTime,
                                              endTime = sd.EndTime,
                                          }).ToList();

            if (sdList.Count == 0)
            {
                if (region != 0)
                {
                    ViewBag.EmptyMessage = "All shifts are approved for this region!";
                }
                else
                {
                    ViewBag.EmptyMessage = "All shifts are approved!";
                }
            }
            return PartialView("Scheduling/_shiftsForReviewPartial", sdList);
        }

        [HttpPost]
        public IActionResult ApproveShifts(string[] shiftIds)
        {
            if (shiftIds != null)
            {
                foreach (var item in shiftIds)
                {
                    var shiftdetail = _context.ShiftDetails.First(u => u.ShiftDetailId == int.Parse(item));
                    shiftdetail.Status = 0;
                    _context.ShiftDetails.Update(shiftdetail);
                }
                _context.SaveChanges();
            }

            return RedirectToAction("requestedShifts");
        }

        public IActionResult deleteShifts(string[] shiftIds)
        {
            if (shiftIds != null)
            {
                foreach (var item in shiftIds)
                {
                    var shiftdetails = _context.ShiftDetails.First(u => u.ShiftDetailId == int.Parse(item));
                    shiftdetails.IsDeleted = new BitArray(new[] { true });
                    _context.ShiftDetails.Update(shiftdetails);
                }
                _context.SaveChanges();
            }
            return RedirectToAction("requestedShifts");
        }

        public IActionResult ProviderOnCall()
        {
            ViewBag.Regions = _admin.regions();
            return View("Scheduling/ProvidersOnCall");
        }

        public IActionResult getMdsOnCall(string regionId)
        {
           var currentTime = DateTime.Now.Hour;
            var onDutyQuery = from shiftDetail in _context.ShiftDetails
                              join physician in _context.Physicians on shiftDetail.Shift.PhysicianId equals physician.PhysicianId
                              join physicianRegion in _context.PhysicianRegions on physician.PhysicianId equals physicianRegion.PhysicianId
                              where (regionId == "0" || physicianRegion.RegionId == int.Parse(regionId)) &&
                                    shiftDetail.ShiftDate.Date == DateTime.Now.Date &&
                                    currentTime >= shiftDetail.StartTime.Hour &&
                                    currentTime <= shiftDetail.EndTime.Hour &&
                                    shiftDetail.IsDeleted == new BitArray(new[] { false }) && physician.IsDeleted == new BitArray ( new[] { false })
                              select physician;

            var onDuty = onDutyQuery.Distinct().ToList();

			var offDutyQuery = from physician in _context.Physicians
							   join physicianRegion in _context.PhysicianRegions on physician.PhysicianId equals physicianRegion.PhysicianId
							   where (regionId == "0" || physicianRegion.RegionId == int.Parse(regionId)) &&
									 !_context.ShiftDetails.Any(item => item.Shift.PhysicianId == physician.PhysicianId &&
                                                                        item.ShiftDate.Date == DateTime.Now.Date &&
																	   currentTime >= item.StartTime.Hour &&
																	   currentTime <= item.EndTime.Hour &&
																	   item.IsDeleted == new BitArray ( new [] {false}))
							   select physician;
			var offDuty = offDutyQuery.Distinct().ToList();

            var providers =  new ProviderOncallOfdutyVM 
            {
                OnDuty = onDuty,
                OffDuty = offDuty
            };
            return PartialView("Scheduling/_providerOnCallPartial", providers);
        }
        #endregion Scheduling

        #region Partners
        public IActionResult Partners()
        {
            ViewBag.Regions = _admin.regions();
            return View("Partners/VendorsDetails");
        }

        public IActionResult GetVendorsDetails(string search, string regionId)
        {
            List<PartnersVM> partners = (from professions in _context.HealthProfessionals
                                         join type in _context.HealthProfessionalTypes
                                         on professions.Profession equals type.HealthProfessionalId
                                         select new PartnersVM()
                                         {
                                             profession = type.ProfessionName,
                                             vendorid = professions.VendorId,
                                             CreatedDate = professions.CreatedDate,
                                             BusinessName = professions.VendorName,
                                             Email = professions.Email,
                                             Fax = professions.FaxNumber,
                                             phoneNumber = professions.PhoneNumber,
                                             BusinessContact = professions.BusinessContact,
                                             regionId = professions.RegionId

                                         }).Where(s => (String.IsNullOrEmpty(search) || s.BusinessName.Contains(search)) && (regionId == "0" || s.regionId == int.Parse(regionId))).ToList();

            if (partners.Count == 0)
            {
                ViewBag.EmptyMessage = "No Data Available";
            }

            return PartialView("Partners/_VendorsDetailsPartial", partners);
        }
        #endregion Partners
    }
}


