using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
namespace BAL.Repository
{
    public class ProviderDashboardRepo : IProviderDashboard
    {
        private readonly ApplicationDbContext _context;
        public ProviderDashboardRepo(ApplicationDbContext context) { 
            _context = context;
        }

        public Physician getPhysicianbyEmail(int  physicianId)
        {
            return _context.Physicians.FirstOrDefault(u => u.PhysicianId == physicianId);
        }
        public void AcceptRequest(int reqid)
        {
            var result = _context.Requests.FirstOrDefault(u => u.RequestId == reqid);
            result.Status = 2;
            result.ModifiedDate = DateTime.Now;
            _context.Requests.Update(result);
            _context.SaveChanges();

            RequestStatusLog requestStatusLog = new RequestStatusLog();
            requestStatusLog.Status = result.Status;
            requestStatusLog.RequestId = reqid;
            requestStatusLog.CreatedDate = DateTime.Now;

            _context.RequestStatusLogs.Add(requestStatusLog);
            _context.SaveChanges();
        }
        public void transferRequest(int reeqid, string Notes) {
            var request = _context.Requests.FirstOrDefault(u => u.RequestId == reeqid);
            request.Status = 1;
            request.PhysicianId = null;
            _context.Requests.Update(request);
            _context.SaveChanges();

            RequestStatusLog log = new RequestStatusLog();
            log.Status = request.Status;
            log.RequestId = reeqid;
            log.Notes = Notes;
            log.CreatedDate = DateTime.Now;
            _context.RequestStatusLogs.Add(log); _context.SaveChanges();
        }

        public void concludeRequest(int reqId, int physicianId, string Notes)
        {
            var request = _context.Requests.FirstOrDefault(u => u.RequestId == reqId);
            request.Status = 8;
            request.ModifiedDate = DateTime.Now;
            _context.Requests.Update(request);
            _context.SaveChanges();

            RequestStatusLog log = new RequestStatusLog()
            {
                RequestId = reqId,
                Status = request.Status,
                PhysicianId = physicianId,
                CreatedDate = DateTime.Now,
                Notes = Notes,

            };
            _context.RequestStatusLogs.Add(log);
            _context.SaveChanges();

            RequestNote note = _context.RequestNotes.FirstOrDefault(u => u.RequestId == reqId);
            note.PhysicianNotes = Notes;
            _context.RequestNotes.Update(note); _context.SaveChanges();
        }

        public List<PhysicianLocation> locationList()
        {
            return _context.PhysicianLocations.ToList();
            
        }
    }
}
