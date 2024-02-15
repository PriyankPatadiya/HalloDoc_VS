using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HalloDoc.Controllers
{
    public class PatientDashBoardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientDashBoardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult ViewDocumentPatdash()
        {
            return View();
        }

        [HttpGet("{requestid}")]
        public IActionResult ViewDocumentsPatdash(int requestid)
        {
            var email = HttpContext.Session.GetString("Email");
            var xyz = _context.AspNetUsers.FirstOrDefault(u => u.Email == email);
            var reqfile = _context.RequestWiseFiles.FirstOrDefault(u => u.RequestId == requestid);

            if (xyz != null )
            {
                ViewBag.username = xyz.UserName;

                var result = (from req in _context.Requests where req.RequestId == requestid
                              select
                              new ViewdocumentVM
                           {
                                  uploader = req.FirstName + " " + req.LastName,
                                  title = reqfile.FileName,
                                  UploadDate = req.CreatedDate,
                           }).ToList();

                return View(result);
            }
            else
            {
                return NotFound();
            }
        }

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
                                  requestid = req.RequestId

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
