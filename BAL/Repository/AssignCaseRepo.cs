using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class AssignCaseRepo : IAssignCase
    {
        private readonly ApplicationDbContext _context;
        public AssignCaseRepo(ApplicationDbContext context) { 
            _context = context;
        }
        public void ChangeOnAssign(int reeqid, int phyid, string notes)
            {
            var Request = _context.Requests.Where(s => s.RequestId == reeqid).FirstOrDefault();
            if (Request != null)
            {
                Request.Status = 2;
                Request.ModifiedDate = DateTime.Now;
                Request.PhysicianId = phyid;
                _context.Update(Request);
                _context.SaveChanges();

                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.Status = Request.Status;
                requestStatusLog.Notes = notes;
                requestStatusLog.RequestId = reeqid;
                requestStatusLog.CreatedDate = DateTime.Now;

                _context.RequestStatusLogs.Add(requestStatusLog);
                _context.SaveChanges();
            }
        }
    }
}
