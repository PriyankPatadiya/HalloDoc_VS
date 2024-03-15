using BAL.Interface;
using BAL.Repository;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.tool.xml;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Rotativa.AspNetCore;
using Org.BouncyCastle.Asn1.Mozilla;

namespace HalloDoc.Controllers
{
    [CustomAuthorize("Administrator")] 
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAdminDashboard _admin;
        private readonly IAdminActions _adminActions;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IuploadFile _uploadfile;
        private readonly IPatientRequest _request;
        private readonly IEmailService _emailService;
        public AdminDashboardController(ApplicationDbContext context, IAdminDashboard admin, IAdminActions action, IHostingEnvironment env, IuploadFile uploadfile, IPatientRequest request, IEmailService emailService)
        {
            _context = context;
            _admin = admin;
            _adminActions = action;
            _hostingEnvironment = env;
            _uploadfile = uploadfile;
            _request = request;
            _emailService = emailService;
        }

        public IActionResult MainPage()
        {
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
            model.Region = _context.Regions.ToList();
            return model;
        }

        // Filter action 

        public IActionResult SearchByName(string SearchString, string selectButton, string StatusButton, string SelectedStateId, string partialviewpath)
        {
            var result = _admin.GetRequestsQuery(StatusButton);
            result = result.Where(s => (String.IsNullOrEmpty(SearchString) || s.PatientName.Contains(SearchString)) && (String.IsNullOrEmpty(selectButton) || s.requestId == int.Parse(selectButton)) && ((SelectedStateId == "0" || SelectedStateId == null) || s.regionId == int.Parse(SelectedStateId)));
            if (result != null)
            {
                if (!String.IsNullOrEmpty(StatusButton))
                {
                    ViewBag.Status = int.Parse(StatusButton);
                }
                return PartialView(partialviewpath, result.ToList());
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
            return RedirectToAction("EncounterForm", new { requestid = model.requestid});
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
            result.regiontable = _context.Regions.ToList();
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
            var client = _context.RequestClients.Where(u => u.RequestId == model.requestid).FirstOrDefault();
            if(client != null)
            {
                client.FirstName = model.FirstName;
                client.LastName = model.LastName;
                client.PhoneNumber = model.Phonenum;
                client.IntDate = model.DateOfBirth.Day;
                client.IntYear = model.DateOfBirth.Year;
                client.StrMonth = model.DateOfBirth.Month.ToString();
                _context.RequestClients.Update(client);
                _context.SaveChanges();
                return RedirectToAction("CloseCase", new { reqid = model.requestid });
            }
            return Ok("can't save details");
        }

        public IActionResult ClosecasePost(int reqid)
        {
            var request =_context.Requests.Where(i => i.RequestId == reqid).FirstOrDefault();
            if(request != null)
            {
                request.Status = 9;
                request.ModifiedDate = DateTime.Now;
                _context.Requests.Update(request);
                _context.SaveChanges();

                RequestStatusLog log = new RequestStatusLog
                {
                    Status = request.Status,
                    RequestId = reqid,
                    CreatedDate = DateTime.Now,
                };
                _context.RequestStatusLogs.Add(log);
                _context.SaveChanges();

                return RedirectToAction("MainPage");
            }
            return Ok("can't close the request");
        }

        // View Notes Actions

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
            SendOrderVM.Professions = _context.HealthProfessionalTypes.ToList();
            SendOrderVM.requestid = requestid;
            MainModel.SendOrderVM = SendOrderVM;
            return View("MainPage", MainModel);
        }

        [HttpPost]
        public IActionResult SendOrder(SendOrderVM model)
        {
            OrderDetail order = new OrderDetail
            {
                RequestId = model.requestid,
                VendorId = model.vendorid,
                FaxNumber = model.Fax,
                Email = model.Email,
                BusinessContact = model.BusinessContact,
                Prescription = model.prescription,
                NoOfRefill = model.Noofretail,
                CreatedDate = DateTime.Now,
            };
            _context.OrderDetails.Add(order);
            _context.SaveChanges();
            return RedirectToAction("MainPage");
        }


        // Assign Case Actions

        [HttpPost]
        public IActionResult CancelCase(int reeqid, string Reason)
        {
            string Notes = Request.Form["Notes"];
            _adminActions.changeStatusOnCancleCase(reeqid, Reason, Notes);
            return Ok();
        }

        public IActionResult filterPhyByRegion(string RegionId)
        {
            var physician = _adminActions.GetPhysicianByRegion(RegionId);
            return Json(physician);
        }

        [HttpPost]
        public IActionResult AssignCase(int reeqid, string physicianId, string Notes)
        {
            _adminActions.ChangeOnAssign(reeqid, int.Parse(physicianId), Notes);
            return View("MainPage");
        }

        // Block Request Actions

        public IActionResult BlockCase(int reeqid, string reason)
        {
            _adminActions.BlockCase(reeqid, reason);
            return View("MainPage");
        }

        // View Document Actions

        public IActionResult ViewDocuments(int reeqid)
        {
            AdminMainPageVM mainmodel = new AdminMainPageVM
            {
                PageName = PageName.viewdocument
            };
            ViewdocumentVM model = new ViewdocumentVM();
            var requestfiles = _context.RequestWiseFiles.Where(x => x.RequestId == reeqid).ToList();
            model.RequestWiseFile = requestfiles;
            model.requestid = reeqid;
            //model.confirmationNumber = _context.Requests.Where(s => s.RequestId == reeqid).FirstOrDefault().ConfirmationNumber.ToUpper();
            mainmodel.DocumentVM = model;

            return View("MainPage", mainmodel);
        }

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
                return RedirectToAction("ViewDocuments", new { reeqid = requestid });
            }
        }

        
        public IActionResult deleteFile(int reqid, string filename) { 
            var requestwisefile = _context.RequestWiseFiles.Where(u => u.RequestId == reqid && u.FileName == filename).FirstOrDefault();
            requestwisefile.IsDeleted[0] = true;

            _context.RequestWiseFiles.Update(requestwisefile);
            _context.SaveChanges();

            ViewBag.Isdelete = true;
            return RedirectToAction("ViewDocuments", new { reeqid = reqid });
        }

        [HttpPost]
        public IActionResult deleteAllFiles( List<string> selectedFiles)
        {
            foreach(var file in selectedFiles)
            {
                var reqwisefile = _context.RequestWiseFiles.Where(u => u.FileName == file).FirstOrDefault();
                reqwisefile.IsDeleted[0] = true;
                _context.RequestWiseFiles.Update(reqwisefile);
            }
            _context.SaveChanges();
            return Ok();

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
                return RedirectToAction("ViewDocuments", new { reeqid = reqid });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public JsonResult CheckSession()
        {
            var request = HttpContext.Request;
            var token = request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { sessionExists = false });
            }
            else
            {
                return Json(new { sessionExists = true });
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
            var data = _context.HealthProfessionals.Where(u => u.VendorId == int.Parse(businessId));
            return Json(data);
        }

        [HttpPost]
        public IActionResult addAdminnote(ViewNotesVM model, int reqid)
        {
            RequestNote notes = _context.RequestNotes.FirstOrDefault(u => u.RequestId == reqid);
            if(notes != null)
            {
                notes.AdminNotes = model.AdminNotes;
                _context.RequestNotes.Update(notes);
                _context.SaveChanges();
                return RedirectToAction("ViewNotesAdmin");
            }
            return RedirectToAction("ViewNotesAdmin");
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
            var link = Url.Action("ReviewAgreement","PatientDashBoard", new { token = token } , protocol: HttpContext.Request.Scheme);

            string to = _context.RequestClients.Where(r => r.RequestId == requestid).First().Email;
            to = "priyank.patadiya@etatvasoft.com";
            string subject = "Agreement";
            var body = new StringBuilder();
            body.AppendFormat("Hello");
            body.AppendLine(@"Click below link to review the agreement");
            body.AppendLine("<a href=\"" + link + "\">Click here to continue the process</a>");

            string Body = body.ToString();

            if(to != null)
            {
                _emailService.SendEmail(to, subject, Body);
                return Ok("Agreement sent");
            }

            return Ok("Failed to send agreement");
        }

        public IActionResult AdminProfile()
        {
            return View();
        }
    }
}
