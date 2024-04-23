using BAL.Interface;
using BAL.Repository;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Identity;
using System.Collections;
using OfficeOpenXml;
using String = System.String;
using Microsoft.CodeAnalysis;

namespace HalloDoc.Controllers
{

    public class AdminDashboardController : Controller
    {
        private readonly IProviders _provider;
        private readonly IAdminDashboard _admin;
        private readonly IAdminActions _adminActions;
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IuploadFile _uploadfile;
        private readonly IPatientRequest _request;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher<AdminProfileVM> _passwordHasher;
        private readonly IPasswordHasher<PhysicianProfileVM> _passHasher;
        private readonly IPasswordHasher<AdminCreateAccVM> _passAdminHasher;
        private readonly IProviderSite _providersite;
        private readonly IUploadProvider _uploadProvider;
        private readonly IAccessMenu _accessMenu;
        private readonly IScheduling _schedule;
        private readonly IProviderDashboard _pDashboard;

        public AdminDashboardController( IAdminDashboard admin, IAdminActions action, IHostingEnvironment env, IuploadFile uploadfile, IPatientRequest request, IEmailService emailService, IPasswordHasher<AdminProfileVM> password,
                    IProviders providers, IUploadProvider upload, IPasswordHasher<PhysicianProfileVM> hasher, IAccessMenu menu, IPasswordHasher<AdminCreateAccVM> hasherr, IProviderSite pSite, IScheduling schedule, IProviderDashboard pDashboard)
        {
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
            _providersite = pSite;
            _schedule = schedule;
            _pDashboard = pDashboard;
        }

        #region Dashboard

        [CustomAuthorize(new string[] { "Administrator"}, "6")]
        public IActionResult MainPage()
        {
            var email = HttpContext.Session.GetString("Email");
            TempData["username"] = _admin.username(email);
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
            result = result.Where(s => (String.IsNullOrEmpty(SearchString) || s.PatientName.ToLower().Contains(SearchString.ToLower())) && (System.String.IsNullOrEmpty(selectButton) || s.requestId == int.Parse(selectButton)) && ((SelectedStateId == "0" || SelectedStateId == null) || s.regionId == int.Parse(SelectedStateId)));



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

        #endregion

        #region Export
        // Export
        [CustomAuthorize(new string[] { "Administrator", "Provider" }, "6")]
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

        #endregion Export

        #region Encounter
        // Encounter Form Actions
        [CustomAuthorize(new string[] { "Administrator", "Provider" }, "6")]
        [HttpGet("AdminDashboard/EncounterForm/{requestid?}", Name = "EncounterByAdmin")]
        [HttpGet("ProviderDashboard/EncounterForm/{requestid?}", Name = "EncounterByProvider")]
        public IActionResult EncounterForm(int requestid)
        {
            var physicianid = HttpContext.Session.GetInt32("PhysicianId");
            ViewBag.IsPhysician = physicianid == null ? false : true;

            AdminMainPageVM MainModel = new AdminMainPageVM
            {
                PageName = PageName.Encounterform
            };
            List<EncounterFormVM> model = _adminActions.getEncounterformdata(requestid);
            MainModel.encountermodel = model[0];
            return View("MainPage", MainModel);
        }

        [HttpPost("ProviderDashboard/EncounterForm/{requestid?}", Name = "EncounterPostProvider")]
        [HttpPost("AdminDashboard/EncounterForm/{requestid?}", Name = "EncounterPostAdmin")]
        public IActionResult EncounterForm(EncounterFormVM model)
        {
            var physicianid = HttpContext.Session.GetInt32("PhysicianId");
            _adminActions.addencounterdata(model);
            return physicianid != null ? RedirectToAction("Dashboard", "ProviderDashboard") : RedirectToAction("MainPage");

        }
        public IActionResult finalizeForm(int requestid)
        {
            bool result = _adminActions.finalize(requestid);
            if (result == true)
            {
                return RedirectToAction("Dashboard", "ProviderDashboard");
            }
            return Ok("Can't finalize the form");
        }

        [HttpGet("ProviderDashboard/GeneratePdf/{requestid?}", Name = "DownloadPdfByProvider")]
        [HttpGet("AdminDashboard/GeneratePdf/{requestid?}", Name = "DownloadPdfByADMIN")]
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

        #endregion Encounter

        #region View Case 

        [CustomAuthorize(new string[] { "Administrator", "Provider" }, "25")]
        [HttpGet("AdminDashBoard/ViewCaseAdmin/{reqcliId?}", Name = "AdminViewCase")]
        [HttpGet("ProviderDashboard/ViewCase/{reqcliId?}", Name = "ProviderCase")]
        public IActionResult ViewCase(string reqcliId)
        {
            var physicianid = HttpContext.Session.GetInt32("PhysicianId");
            var requestclientId = reqcliId;
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

            ViewBag.IsPhysician = physicianid != null ? true : false;

            return View("MainPage", MainModel);
        }

        #endregion View Case    

        #region Close Case
        // Close Case Actions

        [CustomAuthorize(new string[] { "Administrator" }, "6")]
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
                TempData["Message"] = "Data updated";
                TempData["MessageType"] = "success";
                return RedirectToAction("CloseCase", new { reqid = model.requestid });
            }
            TempData["Message"] = "Unable to Update The Data";
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

        #endregion Close Case

        #region View Notes

        [CustomAuthorize(new string[] { "Administrator", "Provider" }, "26")]
        [HttpGet("AdminDashboard/ViewNotesAdmin/{reqid?}", Name = "AdminViewNotes")]
        [HttpGet("ProviderDashboard/ViewNotes/{reqid?}", Name = "ProvideViewNotes")]
        public IActionResult ViewNotesAdminn(int reqid)
        {
            var physicianid = HttpContext.Session.GetInt32("PhysicianId");
            ViewBag.IsPhysician = physicianid == null ? false : true;

            AdminMainPageVM MainModel = new AdminMainPageVM()
            {
                PageName = PageName.ViewNotes
            };
            MainModel.NotesVM = _adminActions.viewnotes(reqid);
            return View("MainPage", MainModel);
        }

        [HttpPost]
        public IActionResult addAdminnote(ViewNotesVM model, int reqid)
        {
            RequestNote notes = _adminActions.reqnotebyreqid(reqid);
            var physicianId = HttpContext.Session.GetInt32("PhysicianId");
            var email = HttpContext.Session.GetString("Email");
            if (notes.CreatedBy == null)
            {
                notes.CreatedBy = physicianId == null ? _admin.getAdminByemail(email).AspNetUserId : _pDashboard.getPhysicianbyEmail((int)physicianId).AspNetUserId;
            }
            model.RequestId = reqid;
            if (notes != null)
            {
                _adminActions.addrequnotes(model, notes);
                return RedirectToAction("ViewNotesAdminn", new { reqid = reqid });
            }
            return RedirectToAction("ViewNotesAdminn", new { reqid = reqid });
        }
        public IActionResult TransferNotes(int reeqid, string Notes)
        {
            int phyid = int.Parse(Request.Form["physicianId"]);

            if (_adminActions.transferNotes(reeqid, phyid, Notes) == true)
            {
                return RedirectToAction("MainPage");
            }
            else
            {
                return Ok("Cannot Add TransferNotes");
            }
        }
        #endregion View Notes

        #region Send Order
        [CustomAuthorize(new string[] { "Administrator", "Provider" }, "21")]
        [HttpGet("ProviderDashboard/SendOrder/{requestid?}", Name = "SendOrderByProvider")]
        [HttpGet("AdminDashboard/SendOrder/{requestid?}", Name = "AdminSendOrder")]
        public IActionResult SendOrder(int requestid)
        {
            var physicianid = HttpContext.Session.GetInt32("PhysicianId");
            ViewBag.IsPhysisian = physicianid == null ? false : true;

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

        [CustomAuthorize(new string[] { "Administrator", "Provider" }, "6")]
        [HttpPost("ProviderDashboard/SendOrder/{requestid?}", Name = "SendOrderByProviderpost")]
        [HttpPost("AdminDashboard/SendOrder/{requestid?}", Name = "AdminSendOrderpost")]
        public IActionResult SendOrder(SendOrderVM model)
        {
            var physicianid = HttpContext.Session.GetInt32("PhysicianId");

            AdminMainPageVM MainModel = new AdminMainPageVM()
            {
                PageName = PageName.SendOrder
            };
            if (physicianid == null)
            {
                var email = HttpContext.Session.GetString("Email");
                var admin = _admin.username(email);
                model.createdby = admin;
            }
            else
            {
                model.createdby = _pDashboard.getPhysicianbyEmail((int)physicianid).FirstName;
            }
            model.CreatedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                bool issend = _adminActions.sendOrder(model);
                if (issend == true)
                {
                    TempData["Message"] = "Successfully sent Your order!...";
                    TempData["MessageType"] = "success";

                    return physicianid == null ? RedirectToAction("MainPage") : RedirectToAction("Dashboard", "ProviderDashboard");

                }
            }
            TempData["Message"] = "Unable to send order";
            TempData["MessageType"] = "warning";
            return RedirectToAction("MainPage");
        }

        #endregion Send Order

        #region Cancel Case
        [CustomAuthorize(new string[] { "Administrator", "Provider" }, "6")]
        [HttpPost]
        public IActionResult CancelCase(int requestid)
        {
            string? Reason = Request.Form["Reason"];
            string? Notes = Request.Form["Notes"];

            bool iscancel = _adminActions.changeStatusOnCancleCase(requestid, Reason, Notes);
            if (iscancel)
            {
                TempData["Message"] = "success";
                TempData["MessageType"] = "success";
            }
            return RedirectToAction("MainPage");
        }

        #endregion Cancel Case

        #region Assign Case
        [CustomAuthorize(new string[] { "Administrator" }, "6")]
        [HttpPost]
        public IActionResult AssignCase(IFormCollection form)
        {

            string reeqid = form["requestid"];
            string physicianId = form["physicianId"];
            string Notes = form["Notes"];
            _adminActions.ChangeOnAssign(int.Parse(reeqid), int.Parse(physicianId), Notes);
            return RedirectToAction("MainPage");
        }
        #endregion Assign Case

        #region Block Case
        // Block Request Actions
        [CustomAuthorize(new string[] { "Administrator" }, "6")]
        public IActionResult BlockCase(int reeqid, string reason)
        {
            _adminActions.BlockCase(reeqid, reason);
            TempData["Message"] = "Blocked Successfully";
            TempData["MessageType"] = "success";
            return RedirectToAction("MainPage");
        }
        #endregion Block Case

        #region View Document
        // View Document Actions
        [CustomAuthorize(new string[] { "Administrator", "Provider" }, "6")]
        [HttpGet("ProviderDashboard/ViewDocuments/{reeqid?}", Name = "ProviderDocument")]
        public IActionResult ViewDocuments(int reeqid)
        {
            var physicianid = HttpContext.Session.GetInt32("PhysicianId");
            ViewBag.IsPhysician = physicianid == null ? false : true;

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
        [HttpGet("ProviderDashboard/downloadfile")]
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

        [HttpPost("ProviderDashboard/uploadFile/{requestid?}", Name = "uploadByProvider")]
        [HttpPost("AdminDashboard/uploadFile/{requestid?}", Name = "uploadByAdmin")]
        public IActionResult uploadFile(int requestid)
        {
            var physicianid = HttpContext.Session.GetInt32("PhysicianId");
            ViewBag.IsPhysician = physicianid != null ? true : false;
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
                return physicianid == null ? RedirectToAction("ViewDocuments", new { reeqid = requestid }) : RedirectToAction("ConcludeCare", "ProviderDashboard", new { requestId = requestid });
            }
        }

        [HttpPost("ProviderDashboard/deleteFile/{reqid?}", Name = "deleteByProvider")]
        [HttpPost("AdminDashboard/deleteFile/{reqid?}", Name = "deleteByAdmin")]
        public IActionResult deleteFile(int reqid, string filename)
        {
            var requestwisefile = _admin.filebyReqidandName(reqid, filename);
            var physicianid = HttpContext.Session.GetInt32("PhysicianId");
            ViewBag.IsPhysician = physicianid != null ? true : false;
            if (requestwisefile != null)
            {
                _admin.deleteSingleFile(requestwisefile);
                ViewBag.Isdelete = true;
                TempData["Message"] = "file deleted successfully....";
                TempData["MessageType"] = "success";
                return physicianid == null ? RedirectToAction("ViewDocuments", new { reeqid = reqid }) : RedirectToAction("ConcludeCare", "ProviderDashboard", new { requestId = reqid });
            }
            TempData["Message"] = "Unable To delete file";
            TempData["MessageType"] = "warning";
            return physicianid == null ? RedirectToAction("ViewDocuments", new { reeqid = reqid }) : RedirectToAction("ConcludeCare", "ProviderDashboard", new { requestId = reqid });
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
        #endregion View Document

        #region Clear Case

        [CustomAuthorize(new string[] { "Administrator" }, "6")]
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
        #endregion Clear Case

        #region Send Aggrement

        [CustomAuthorize(new string[] { "Administrator", "Provider" }, "6")]
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

                return TempData["isFromPhysician"] != null ? RedirectToAction("Dashboard", "ProviderDashboard") : RedirectToAction("MainPage");


            }
            TempData["Message"] = "can't send Aggrement";
            TempData["MessageType"] = "warning";

            return TempData["isFromPhysician"] != null ? RedirectToAction("Dashboard", "ProviderDashboard") : RedirectToAction("MainPage");
        }

        #endregion Send Aggrement

        #region Admin Profile

        [CustomAuthorize(new string[] { "Administrator" }, "5")]
        public IActionResult AdminProfile(string mail)
        {
            string email = HttpContext.Session.GetString("Email");
            ViewBag.username = _admin.username(email);
            var adminn = mail != null ? _admin.getProfileData(mail) : _admin.getProfileData(email);

            return View(adminn);
        }

        public IActionResult changeAccInfo(AdminProfileVM model, int AdminId, List<string> regions)
        {
            var isAdminExistById = _admin.isAdminExistById(AdminId);
            var isAdminExist = _admin.isAdminExist(model.Email);
            string prevMail = _admin.getMailByAdminId(AdminId);
            bool isMailChanged = _admin.isMailChanged(prevMail, model.Email);
            if (isAdminExistById)
            {
                if(isMailChanged && isAdminExist)
                {
                    TempData["Message"] = "Added Email is Already Exist";
                    TempData["MessageType"] = "warning";
                    return RedirectToAction("AdminProfile", new {mail = model.Email});
                }
                else
                {
                    _admin.changeAccountInfo(model, prevMail, regions);
                    HttpContext.Session.SetString("Email", model.Email);
                    TempData["Message"] = "Edited Successfully";
                    TempData["MessageType"] = "success";
                    return isMailChanged == false ? RedirectToAction("AdminProfile", new {mail = model.Email}) : RedirectToAction("PatientLoginn", "Home");
                }
            }
            TempData["Message"] = "Not able to change information";
            TempData["MessageType"] = "success";
            return RedirectToAction("AdminProfile", new {mail = prevMail});
        }

        public IActionResult changeBillingInfo(int AdminId, AdminProfileVM model)
        {
            string email = _admin.getMailByAdminId(AdminId);
            if (email != "")
            {
                _admin.changeBillingInfo(model, email);
                TempData["Message"] = "Edited Successfully";
                TempData["MessageType"] = "success";
                return RedirectToAction("AdminProfile", new {mail = email});
            }
            TempData["Message"] = "Not able to change information";
            TempData["MessageType"] = "success";
            return RedirectToAction("AdminProfile", new {mail = email});
        }

        public IActionResult changePass(int AdminId)
        {
            string Password = Request.Form["Password"];
            string userEmail = HttpContext.Session.GetString("Email");
            string email = _admin.getMailByAdminId(AdminId);
            Password = _passwordHasher.HashPassword(null, Password);
            if (userEmail != "")
            {
                _admin.changePassword(email, Password);
                TempData["Message"] = "Password Changed....";
                TempData["MessageType"] = "success";
                return userEmail == email ? RedirectToAction("PatientLoginn", "Home") : RedirectToAction("AdminProfile", new {mail = email});
            }
            TempData["Message"] = "Not Able to change password";
            TempData["MessageType"] = "warning";
            return RedirectToAction("AdminProfile" , new {mail = email});
        }

        #endregion Admin Profile

        #region Send Link
        // Send Link

        [CustomAuthorize(new string[] { "Administrator", "Provider" }, "6")]
        [HttpPost("ProviderDashboard/sendLinkofSubmitreq")]
        public IActionResult sendLinkofSubmitreq(string PatientFirstname, string PatientLastname, string PatientEmail)
        {
            var physicianId = HttpContext.Session.GetInt32("PhysicianId");
            bool isPhysician = physicianId != null ? true : false;

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
                return isPhysician ? RedirectToAction("Dashboard","ProviderDashboard") : RedirectToAction("MainPage");
            }
            TempData["Message"] = "Can't Send Email";
            TempData["MessageType"] = "warning";
            return isPhysician? RedirectToAction("Dashboard","ProviderDashboard") : RedirectToAction("MainPage");
        }

        #endregion Send Link

        #region Create Request
        // Create Request

        [CustomAuthorize(new string[] { "Administrator", "Provider" }, "27")]
        [HttpGet("ProviderDashboard/CreateRequestAdmin", Name = "createRequestProvider")]
        [HttpGet("AdminDashboard/CreateRequestAdmin", Name = "createRequestAdmin")]
        public IActionResult CreateRequestAdmin()
        {
            var physicianId = HttpContext.Session.GetInt32("PhysicianId");
            ViewBag.IsPhysician = physicianId != null ? true : false;
            PatientReqVM model = new PatientReqVM();
            model.Region = _admin.regions();
            return PartialView("CreateRequestAdmin", model);
        }

        [HttpPost("ProviderDashboard/CreateRequestAdmin")]
        [HttpPost("AdminDashboard/CreateRequestAdmin")]
        public async Task<IActionResult> CreateRequestAdmin(PatientReqVM model)
        {
            var physicianId = HttpContext.Session.GetInt32("PhysicianId");
            var email = HttpContext.Session.GetString("Email");
            if (ModelState.IsValid)
            {
                bool isPhysician = physicianId != null ? true : false;
                model.State = await _request.GetStateAccordingToRegionId(model.SelectedStateId);
                _request.AddAdminCreateRequest(model, email, isPhysician,(int)( isPhysician ? physicianId : 0) );
                TempData["Message"] = "Request Added!";
                TempData["MessageType"] = "success";
                
                return isPhysician ? RedirectToAction("Dashboard","ProviderDashboard") : RedirectToAction("MainPage");
            }
            ViewBag.IsPhysician = physicianId != null ? true : false;
            model.Region = _admin.regions();
            return PartialView("CreateRequestAdmin", model);

        }

        #endregion Create Admin Request

        #region Provider Menu
        // Provider Menu

        [CustomAuthorize(new string[] { "Administrator" }, "4")]
        public IActionResult Provider()
        {
            ProviderMenuVM model = new ProviderMenuVM();
            model.regions = _admin.regions();
            return View("ProviderMenu/Provider", model);
        }

        public IActionResult filterProviderTable(string stateid, int currentPage, int pageSize)
        {

            var result = _provider.getfilteredPhysicians(int.Parse(stateid)).ToList();
            int totalPages = (int)Math.Ceiling((double)result.Count() / pageSize);
            var paginatedData = result.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            if (paginatedData.Count != 0)
            {
                ViewBag.CurrentPage = currentPage;
                ViewBag.TotalPages = totalPages;
            }
            ViewBag.IsNullData = paginatedData.Count() != 0 ? false : true;
            return PartialView("ProviderMenu/_ProviderPartialTable", paginatedData);
        }

        [HttpGet("ProviderDashboard/ProviderProfile/{id?}", Name = "ProfileProvider")]
        [HttpGet("AdminDashboard/ProviderProfile/{id?}", Name = "ProfileProviderAdmin")]
        public IActionResult ProviderProfile(int id)
        {
            var physicianId = HttpContext.Session.GetInt32("PhysicianId");
            Physician? physician;
            if (physicianId != null)
            {
                ViewBag.IsPhysician = true;
                physician = _pDashboard.getPhysicianbyEmail((int)physicianId);
            }

            else
            {
                ViewBag.IsPhysician = false;
                physician = _provider.getPhysicianById(id);
            }
            PhysicianProfileVM physicanProfile = _providersite.getProviderProfileData(physician);
            physicanProfile.Regions = _admin.regions();

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
                TempData["Message"] = "Information Updated";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = "Unable to Update Data";
                TempData["MessageType"] = "warning";
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
                TempData["Message"] = "Data Updated";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = "Unable to Update Data";
                TempData["MessageType"] = "warning";
            }
            return RedirectToAction("ProviderProfile", new { id = physicianid });
        }

        public IActionResult Physicianprofile(int id, string businessName, string businessWebsite, IFormFile signatureFile, IFormFile photoFile)
        {
            try
            {
                _admin.UpdateProviderProfile(id, businessName, businessWebsite, signatureFile, photoFile);
                TempData["Message"] = "Provider Profile Updated";
                TempData["MessageType"] = "success";
            }
            catch (Exception ex)
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
                TempData["Message"] = "Signature Saved";
                TempData["MessageType"] = "success";
                return RedirectToAction("ProviderProfile", new { id = id });
            }
            else
            {
                TempData["Message"] = "Can't save signature";
                TempData["MessageType"] = "warning";
                return RedirectToAction("ProviderProfile", new { id = id });
            }
        }

        [HttpPost]
        public IActionResult UploadDocs(string fileName, IFormFile File, int physicianid)
        {
            Physician? physician = _provider.getPhysicianById(physicianid);
            if (physician != null)
            {

                switch (fileName)
                {
                    case "ICA":
                        _uploadProvider.UploadDocFile(File, physicianid, fileName);
                        physician.IsAgreementDoc = new BitArray(new[] { true });
                        break;
                    case "Background":
                        _uploadProvider.UploadDocFile(File, physicianid, fileName);
                        physician.IsBackgroundDoc = new BitArray(new[] { true });
                        break;
                    case "Hippa":
                        _uploadProvider.UploadDocFile(File, physicianid, fileName);
                        physician.IsTrainingDoc = new BitArray(new[] { true });
                        break;
                    case "NonDiscoluser":
                        _uploadProvider.UploadDocFile(File, physicianid, fileName);
                        physician.IsNonDisclosureDoc = new BitArray(new[] { true });
                        break;
                    case "License":
                        _uploadProvider.UploadDocFile(File, physicianid, fileName);
                        physician.IsLicenseDoc = new BitArray(new[] { true });
                        break;
                    default:
                        // Handle unexpected file names here
                        break;
                }
                _provider.updateBilling(physician);
                TempData["Message"] = "Documents Uploaded";
                TempData["MessageType"] = "success";
                return RedirectToAction("ProviderProfile", new { id = physicianid });

            }
            else
            {
                TempData["Message"] = "Unable to upload Doc";
                TempData["MessageType"] = "warning";
                return RedirectToAction("ProviderProfile", new { id = physicianid });
            }
        }

        public IActionResult CreateProviderAcc(int id)
        {
            PhysicianProfileVM model = new PhysicianProfileVM();
            model.isFromUserAccess = id == 1 ? true : false;

            model.Regions = _admin.regions();
            return View("ProviderMenu/CreateProviderAccount", model);
        }

        [HttpPost]
        public IActionResult CreateProviderAcc(PhysicianProfileVM model, string[] Region, string latitude, string longitude)
        {
            if (ModelState.IsValid)
            {
                string hashedpass = _passHasher.HashPassword(null, model.Password);
                _provider.createproviderAcc(model, Region, hashedpass);
                TempData["Message"] = "Account Created";
                TempData["MessageType"] = "success";
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
        #endregion Provider Menu

        #region Access
        // Access 
        [CustomAuthorize(new string[] { "Administrator" }, "7")]
        public IActionResult userAccess()
        {
            return View("AccessMenu/UserAccess");
        }

        public IActionResult filterUserAccessTable(string AccTypeId)
        {
            var result = _accessMenu.getUserAccessData(int.Parse(AccTypeId));
            return PartialView("AccessMenu/_UserAccessPartial", result);
        }


        [CustomAuthorize(new string[] { "Administrator" }, "4")]
        public IActionResult CreateAdminAcc()
        {
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
            return RedirectToAction("userAccess");
        }
        public IActionResult CreateProviderAccessAcc()
        {
            PhysicianProfileVM model = new PhysicianProfileVM();
            model.isFromUserAccess = true;

            model.Regions = _admin.regions();
            return View("ProviderMenu/CreateProviderAccount", model);
        }

        public IActionResult CreateAdminAccessAcc()
        {
            AdminCreateAccVM model = new AdminCreateAccVM();
            model.Regions = _admin.regions();
            return View("ProviderMenu/CreateAdminAccount", model);
        }

        [CustomAuthorize(new string[] { "Administrator" }, "7")]
        // Role Access

        public IActionResult roleAccess()
        {
            var roles = _accessMenu.getRole();
            var list = roles.Where(item => item.IsDeleted != null && (item.IsDeleted.Length == 0 || !item.IsDeleted[0]));
            return View("AccessMenu/RoleAccess", list.ToList());
        }

        public IActionResult CreateAccess()
        {
            return View("AccessMenu/CreateAccess");
        }

        public IActionResult GetRoles(int role)
        {
            var menu = _accessMenu.getMenubyRole(role);
            return PartialView("AccessMenu/_CreateAccessRole", menu);
        }

        [HttpPost]
        public IActionResult CreateAccess(int[] rolemenu, string rolename, int accounttype)
        {
            _accessMenu.createAccess(rolemenu, rolename, accounttype);
            TempData["Message"] = "Created Successfully!";
            TempData["MessageType"] = "success";
            return RedirectToAction("roleAccess");
        }
        public IActionResult EditAccess(int roleid)
        {
            var role = _accessMenu.getRoleByRoleId(roleid);
            EditRoleAccessVM model = _accessMenu.editRoleAccess(roleid, role);
            return View("AccessMenu/EditRole", model);
        }

        [HttpPost]
        public IActionResult EditAccess(int[] rolemenu, string rolename, int roleid)
        {
            _accessMenu.editAccess(rolemenu, rolename, roleid);
            TempData["Message"] = "Edited Successfully!";
            TempData["MessageType"] = "success";
            return RedirectToAction("roleAccess");
        }

        public IActionResult DeleteRole(int roleid)
        {
            _accessMenu.DeleteRole(roleid);
            TempData["Message"] = "Removed Successfully!";
            TempData["MessageType"] = "success";


            return RedirectToAction("roleAccess");
        }

        
        #endregion Access

        #region Location
        // location 

        [CustomAuthorize(new string[] { "Administrator" }, "16")]
        public IActionResult ProviderLocation()
        {
            return View();
        }
        public IActionResult getPhysicianMapDetail()
        {
            List<PhysicianLocation> physicianLocations = _pDashboard.locationList();
                return Json(physicianLocations);
        }
        #endregion Location

        #region Scheduling

        [CustomAuthorize(new[] { "Administrator" }, "2")]
        public IActionResult Scheduling()
        {
            SchedulingVM model = new SchedulingVM();
            model.Region = _admin.regions();
            return View("Scheduling/Scheduling", model);
        }


        public IActionResult GetPhysicianShift(string region)
        {
            var physicians = _schedule.getShiftPhysicians(region);
            if (physicians.Count() == 0)
            {
                return Json(new { Message = "No physicians found for the given region." });
            }
            return Json(physicians);
        }



        [CustomAuthorize(new[] { "Administrator", "Provider" }, "6")]
        [HttpPost("ProviderDashboard/CreateShift")]
        [HttpPost("AdminDashboard/CreateShift")]

        public IActionResult CreateShift(SchedulingVM model)
        {
            var physicianId = HttpContext.Session.GetInt32("PhysicianId");
            var email = HttpContext.Session.GetString("Email");
            if (ModelState.IsValid)
            {
                var creater = physicianId == null ? _admin.getAdminByemail(email).AspNetUserId : _provider.getPhysicianById((int)physicianId).AspNetUserId;

                // Check if existz
                bool isShiftExist = _schedule.IsShiftExist(model, physicianId);


                if (!isShiftExist)
                {
                    Shift shift = new Shift();

                    // Add shift
                    _schedule.addShift(model, physicianId, creater, shift);

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
                                bool isRepeatedShiftExist = _schedule.IsRepeatedShiftExist(model, physicianId, startDateForWeekday, i);

                                if (isRepeatedShiftExist)
                                {
                                    TempData["Message"] = "Shift Already exist on " + startDateForWeekday.ToString("dddd");
                                    TempData["MessageType"] = "error";
                                    return physicianId == null ? RedirectToAction("Scheduling") : RedirectToAction("Scheduling", "ProviderDashboard");
                                }
                                else
                                {

                                    TempData["Message"] = "Shifts Added";
                                    TempData["MessageType"] = "success";
                                    _schedule.AddRepeatedShift(model, (int)physicianId, startDateForWeekday, shift, i);
                                }

                            }
                        }
                    }
                    return physicianId == null ? RedirectToAction("Scheduling") : RedirectToAction("Scheduling", "ProviderDashboard");

                }
                else
                {
                    TempData["Message"] = "shift is already exist";
                    TempData["MessageType"] = "error";
                    return physicianId == null ? RedirectToAction("Scheduling") : RedirectToAction("Scheduling", "ProviderDashboard");
                }
            }
            TempData["Message"] = "Value incorrect";
            TempData["MessageType"] = "error";
            return physicianId == null ? RedirectToAction("Scheduling") : RedirectToAction("Scheduling", "ProviderDashboard");
        }

        public IActionResult getEvents(int regionId)
        {
            var events = _schedule.getEvents(regionId);
            events = events.Where(item => !item.ShiftDeleted).ToList();

            return Json(events);
        }

        [HttpPost]
        public IActionResult saveShiftChanges(int shiftDetailId, DateTime startDate, TimeOnly startTime, TimeOnly endTime, int region)
        {
            // Find the shift detail by its ID
            ShiftDetail shiftdetail = _schedule.sdBysDId(shiftDetailId);

            // If shift detail is not found, return a 404 Not Found response
            if (shiftdetail == null)
            {
                return BadRequest();
            }
            try
            {
                // Update the shift detail properties
                _schedule.saveShift(shiftdetail, startDate, startTime, endTime);
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
            ShiftDetail shiftdetail = _schedule.sdBysDId(shiftDetailId);
            if (shiftdetail == null)
            {
                return NotFound("Shift detail not found.");
            }
            _schedule.deleteShift(shiftdetail);
            var events = _adminActions.getEvents(region);

            return Ok(new { message = "Shift detail Deleted successfully.", events = events });

        }

        public IActionResult changeShiftStatus(int shiftDetailId, int region)
        {
            ShiftDetail shiftDetail = _schedule.sdBysDId(shiftDetailId);
            if (shiftDetail == null)
            {
                return NotFound("Shift detail not found.");
            }
            shiftDetail.Status = (short)(shiftDetail.Status == 0 ? 1 : 0);
            _schedule.returnShift(shiftDetail);
            var events = _adminActions.getEvents(region);

            return Ok(new { message = "Shift Returned successfully.", events = events });
        }
        public IActionResult requestedShifts()
        {
            ViewBag.Regions = _admin.regions();
            return View("Scheduling/shiftForReview");
        }

        [HttpGet]
        public IActionResult changeShiftReviewTable(int region, int currentPage, int pageSize)
        {
            var sdList = _schedule.listBYRegion(region);
            int totalPages = (int)Math.Ceiling((double)sdList.Count() / pageSize);
            var paginatedData = sdList.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            if (paginatedData.Count != 0)
            {
                ViewBag.CurrentPage = currentPage;
                ViewBag.TotalPages = totalPages;
            }

            if (paginatedData.Count == 0)
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
            return PartialView("Scheduling/_shiftsForReviewPartial", paginatedData);
        }

        [HttpPost]
        public IActionResult ApproveShifts(string[] shiftIds)
        {
            if (shiftIds != null)
            {
                _schedule.ApproveShifts(shiftIds);
            }

            return RedirectToAction("requestedShifts");
        }

        public IActionResult deleteShifts(string[] shiftIds)
        {
            if (shiftIds != null)
            {
                _schedule.DeleteShifts(shiftIds);
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
            

            var onDuty = _schedule.OnDuty(regionId);

            var offDuty = _schedule.OffDuty(regionId);

            var providers = new ProviderOncallOfdutyVM
            {
                OnDuty = onDuty,
                OffDuty = offDuty
            };
            return PartialView("Scheduling/_providerOnCallPartial", providers);
        }
        #endregion Scheduling

        #region Partners

        [CustomAuthorize(new[] { "Administrator" }, "10")]
        public IActionResult Partners()
        {
            ViewBag.Regions = _admin.regions();
            ViewBag.Professions = _provider.getProfessionals();
            return View("Partners/VendorsDetails");
        }

        public IActionResult GetVendorsDetails(string search, string regionId)
        {
            List<PartnersVM> partners = _providersite.getVendors(search, regionId);

            if (partners.Count == 0)
            {
                ViewBag.EmptyMessage = "No Data Available";
            }

            return PartialView("Partners/_VendorsDetailsPartial", partners);
        }

        [HttpPost]
        public IActionResult addBusiness(AddBusinessVM vendor)
        {
            if (ModelState.IsValid)
            {
                _providersite.addBusiness(vendor);
                TempData["Message"] = "Business Added";
                TempData["MessageType"] = "success";
            }

            return RedirectToAction("Partners");
        }

        public IActionResult EditBusiness(int Vendorid)
        {
            AddBusinessVM model = _providersite.getBusinessDetails(Vendorid);
            ViewBag.Regions = _admin.regions();
            ViewBag.Professions = _provider.getProfessionals();
            
            return PartialView("Partners/EditBusiness", model);
        }

        [HttpPost]
        public IActionResult EditBusiness(AddBusinessVM vendor)
        {
            var Vendor = _provider.getProfessionByVendorId(vendor.vendorId);
            if (ModelState.IsValid && Vendor != null)
            {
                TempData["Message"] = "Edited!";
                TempData["MessageType"] = "success";
                _providersite.updateBusinessDetails(vendor, Vendor);
            }
            return RedirectToAction("Partners");
        }

        public IActionResult DeleteBusiness(int Vendorid)
        {
            var Vendor = _provider.getProfessionByVendorId(Vendorid);
            if (Vendor != null)
            {
                _providersite.deleteBusiness(Vendor);
                TempData["Message"] = "Deleted";
                TempData["MessageType"] = "success";
            }
            return RedirectToAction("Partners");
        }
        #endregion Partners

        #region PatientHistory

        [CustomAuthorize(new[] { "Administrator" }, "3")]
        public IActionResult PatientHistory()
        {
            return View("RecordsMenu/PatientRecords");
        }

        public IActionResult getPatientRecords(string firstName, string lastName, string email, string phoneNumber)
        {
            var user = _providersite.getFileteredPatient(firstName, lastName, email, phoneNumber);

            ViewBag.IsEmpty = user.Count() > 0 ? false : true;
            return PartialView("RecordsMenu/_patientRecordsPartial", user);
        }

        public IActionResult explorePatientRequests(int userId)
        {
            var requests = _providersite.getExploreRequests(userId);
            return View("RecordsMenu/explorePatientRecord", requests);
        }
        #endregion PatientHistory

        #region Search Records

        [CustomAuthorize(new[] { "Administrator" }, "6")]
        public IActionResult SearchRecords()
        {
            return View("RecordsMenu/SearchRecords");
        }
        public IActionResult getSearchRecordsData(int[] status, string patientName,
        string providerName, string phoneNum, string email, string requestType, DateTime fromDate, DateTime toDate, int currentPage, int pageSize)
        {
            var searchedRecord = _providersite.getSearchRecordsWithFilter(status, patientName, providerName, phoneNum, email, requestType, fromDate, toDate).ToList();

            int totalPages = (int)Math.Ceiling((double)searchedRecord.Count() / pageSize);
            var paginatedData = searchedRecord.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            if (paginatedData.Count != 0)
            {
                ViewBag.CurrentPage = currentPage;
                ViewBag.TotalPages = totalPages;
                ViewBag.IsEmpty = false;
            }
            else
            {
                ViewBag.IsEmpty = true;
            }
            return PartialView("RecordsMenu/_searchRecordsPartial", paginatedData);

        }
        public IActionResult exportSearchRecordfile(int[] status, string patientName,
        string providerName, string phoneNum, string email, string requestType)
        {
            var searchedRecord = _providersite.getSearchRecordsWithFilter(status, patientName, providerName, phoneNum, email, requestType, new DateTime() , new DateTime()).ToList();

            
            using (var excel = new ExcelPackage())
            {
                var worksheet = excel.Workbook.Worksheets.Add("sheet1");
                worksheet.Cells[1, 1].Value = "PatientName";
                worksheet.Cells[1, 3].Value = "RequestorName";
                worksheet.Cells[1, 4].Value = "RequestDate";
                worksheet.Cells[1, 5].Value = "phone";
                worksheet.Cells[1, 6].Value = "address";
                worksheet.Cells[1, 7].Value = "Email";
                worksheet.Cells[1, 8].Value = "Zip";
                worksheet.Cells[1, 9].Value = "Physician";

                var row = 2;
                foreach (var item in searchedRecord)
                {
                    worksheet.Cells[row, 1].Value = item.PatientName;
                    worksheet.Cells[row, 3].Value = item.Requestor;
                    worksheet.Cells[row, 4].Value = item.DateOfService;
                    worksheet.Cells[row, 5].Value = item.PhoneNumber;
                    worksheet.Cells[row, 6].Value = item.Address;
                    worksheet.Cells[row, 7].Value = item.Email;
                    worksheet.Cells[row, 8].Value = item.Zip;
                    worksheet.Cells[row, 9].Value = item.PhysicianName;
                    row++;
                }

                var excelBytes = excel.GetAsByteArray();
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "export.xlsx");

            }
        }

        public IActionResult deleteRequest(int requestId)
        {
            _admin.deleteRequest(requestId);
            return RedirectToAction("SearchRecords");
        }
        #endregion Search Records

        #region Blocked History

        [CustomAuthorize(new string[] { "Administrator" }, "15")]
        public IActionResult BlockedHistory()
        {
            return View("RecordsMenu/BlockedRequests");
        }

        public IActionResult getBlockedRequests(DateTime date, string Name, string email, string phoneNumber)
        {
            List<BlockedHistoryVM> blocked = _providersite.getBlockedRequests().ToList();

            var blockedrequests = blocked.Where(i => (String.IsNullOrEmpty(Name) || i.PatientName.Contains(Name)) &&
                                                      (String.IsNullOrEmpty(email) || i.Email.Contains(email)) &&
                                                      (String.IsNullOrEmpty(phoneNumber) || i.PhoneNumber.Contains(phoneNumber))
                                                      ).ToList();
            ViewBag.IsEmpty = blockedrequests.Count() > 0 ? false : true;
            return PartialView("RecordsMenu/_blockedHistoryPartial", blockedrequests);
        }

        public IActionResult unBlockRequest(int RequestId)
        {
            _admin.unblockRequest(RequestId);
            return RedirectToAction("BlockedHistory");
        }
        #endregion Blocked History

        #region Email-log

        public IActionResult EmailLog()
        {
            return View("RecordsMenu/EmailLogs");
        }

        //public IActionResult getEmailRecords()
        //{
        //    var Emails = _admin.getEmailRecords();

        //    return PartialView("RecordsMenu/_emailLogsPartial", Emails);
        //}
        #endregion

        #region Other
        [CustomAuthorize(new string[] { "Administrator", "Provider" }, "6")]
        public IActionResult filterPhyByRegion(string RegionId)
        {
            var physician = _adminActions.GetPhysicianByRegion(RegionId);
            return Json(physician);
        }
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
        #endregion Other
    }
}