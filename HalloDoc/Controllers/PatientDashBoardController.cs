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
        public IActionResult PatientDashboard(string Email)
        {
            var xyz = _context.AspNetUsers.FirstOrDefault(u => u.Email == Email);
            if (xyz == null)
            {
               return NotFound();
            }
            else
            {
                ViewBag.username = xyz.UserName;
            }
            var email = HttpContext.Session.GetString("Email");
            var result = (from req in _context.Requests join requestfile in _context.RequestWiseFiles on req.RequestId equals requestfile.RequestId
                          into reqs
                          where req.Email == email
                          from requestfile
                          in reqs.DefaultIfEmpty()
                          select new PatientDashboardVM 
                          {
                              currentStatus = req.Status,
                              CreatedDate = req.CreatedDate,
                              Document = requestfile.FileName == null ? null : requestfile.FileName

                          }).ToList();
            
            return View(result);
        }
    }
}
