using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace HalloDoc.Controllers
{
    public class PatientDashBoardController : Controller
    {
        private readonly ApplicationDbContext _context;
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
                string path = Path.Combine(this._environment.WebRootPath, "uploads");
                _uploadfile.uploadfile(file, path);

                _request.Addrequestwisefile(file.FileName, requestid);
                return RedirectToAction("ViewDocumentsPatdash", new { requestid = requestid });
            }
        }



        [HttpGet("{requestid}")]
        public async Task<IActionResult> ViewDocumentsPatdash(int requestid)
        {
            var email = HttpContext.Session.GetString("Email");
            var xyz = _context.AspNetUsers.FirstOrDefault(u => u.Email == email);
            var reqfile =await _context.RequestWiseFiles.Where(x=>x.RequestId==requestid).ToListAsync();


            if (xyz != null )
            {
                ViewBag.username = xyz.UserName;

                var result = new ViewdocumentVM
                              {
                                  RequestWiseFile = reqfile,
                                  requestid = requestid
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

        //[HttpPost]

        //public IActionResult downloadallfiles(int requestid)
        //{
        //    var zipName = $"TestFiles-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        //required: using System.IO.Compression;  
        //        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
        //        {



        //            _context.RequestWiseFiles.Where(u => u.RequestId == requestid).ToList().ForEach(file =>
        //            {
        //                var entry = zip.CreateEntry(file.FileName);
        //                using (var fileStream = new MemoryStream(file.))
        //                using (var entryStream = entry.Open())
        //                {
        //                    fileStream.CopyTo(entryStream);
        //                }
        //            });
        //        }
        //        return File(ms.ToArray(), "application/zip", zipName);
        //    }
        //}

        public IActionResult PatientDashboard()
        {
         
            var email = HttpContext.Session.GetString("Email");
           
            var xyz = _context.AspNetUsers.FirstOrDefault(u => u.Email == email);
            if(xyz != null)
            {
                ViewBag.username = xyz.UserName;
                

                var result = (from req in _context.Requests join requestfile in _context.RequestWiseFiles on req.RequestId equals requestfile.RequestId
                              into reqs
                              where req.Email == email
                              from requestfile
                              in reqs.DefaultIfEmpty()
                              select new PatientDashboardVM
                              {
                                  currentStatus = req.Status,
                                  CreatedDate = req.CreatedDate,
                                  Document = requestfile.FileName == null ? null : requestfile.FileName,
                                  requestid = req.RequestId,
                                  count = _context.RequestWiseFiles.Count(u => u.RequestId == req.RequestId),

                              }).ToList();
            
            return View(result);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
