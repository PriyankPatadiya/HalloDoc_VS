using BAL.Interface;
using DAL.DataContext;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.Controllers
{
    public class patientFormsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IPatientRequest _patreq;
        private readonly IRequests _otherreq;

        public patientFormsController(ApplicationDbContext context, IPatientRequest patreq, IRequests otherreq)
        {
            _context = context; 
            _patreq = patreq;
            _otherreq = otherreq;
        }
        public IActionResult PatientRequestForm()
        {
            return View();
        }


        public IActionResult Friend_FamilyRequestForm()
        {
            return View();
        }
        public IActionResult BusinessRequestForm()
        {
            return View();
        }
        public IActionResult ConciergeRequestForm()
        {
            return View();
        }

        public IActionResult PatientReq(PatientReqVM pInfo)
        {
            if(ModelState.IsValid) 
            {
                if(pInfo.PasswordHash != pInfo.ConfirmPasswordHash)
                {
                    ModelState.AddModelError("ConfirmPassword", "Password and ConfirmPassword is not same");
                    return View("PatientRequestForm");
                }
                
                if (pInfo != null)
                {
                    
                   _patreq.AddPatientForm(pInfo);
                    return View("Friend_FamilyRequestForm");
                }

                
                
            }
            return View("PatientRequestForm");
        }

        [HttpPost]
        public IActionResult Friend_FamilyRequest(OthersReqVM model)
        {
            if (ModelState.IsValid)
            {
                if(model != null)
                {
                    _otherreq.AddFamilyFriendForm(model);
                    return View();
                }
            }
            return View("Friend_FamilyRequestForm");
        }

        [HttpPost]

        public IActionResult ConciergeRequestForm(OthersReqVM model)
        {
            if (ModelState.IsValid)
            {
                if(model !=null)
                {
                    _otherreq.AddConciergeForm(model);
                    return View();
                }
            }
            return View();
        }

        [HttpPost]

        public IActionResult BusinessRequestForm(OthersReqVM model)
        {
            if (ModelState.IsValid) { 
                if(model != null)
                {
                    _otherreq.AddBusinessForm(model);
                    return View();
                }
            }
            return View();
        }

        [HttpPost]
        public bool CheckMail(string email)
        {
            var user = _context.AspNetUsers.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        


    }
}
