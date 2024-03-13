using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Org.BouncyCastle.Asn1.Mozilla;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;


namespace BAL.Repository
{
    public class AdminActionRepo : IAdminActions
    {

        private readonly ApplicationDbContext _context;

        public AdminActionRepo(ApplicationDbContext context)
        {
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
        public IQueryable<ViewCaseVM> getViewCaseData(int reqclientId)
        {
            var result = (from req in _context.Requests
                          join reqclient in _context.RequestClients
                           on req.RequestId equals reqclient.RequestId
                          select new ViewCaseVM()
                          {
                              FirstName = reqclient.FirstName.ToLower(),
                              LastName = reqclient.LastName,
                              DateOfBirth = (new DateTime((int)reqclient.IntYear, int.Parse(reqclient.StrMonth), (int)reqclient.IntDate)),
                              Phone = reqclient.PhoneNumber,
                              Email = reqclient.Email,
                              Notes = reqclient.Notes,
                              address = reqclient.City + " , " + reqclient.State + " , " + reqclient.ZipCode.ToString(),
                              region = reqclient.State,
                              ConfirmationNumber = req.ConfirmationNumber,
                              RequestClientId = reqclient.RequestClientId,

                          });
            result = result.Where(s => s.RequestClientId == reqclientId);
            return result;
        }

        public void changeStatusOnCancleCase(int requesid, string reason, string Notes)
        {
            var request = _context.Requests.FirstOrDefault(h => h.RequestId == requesid);
            if (request != null)
            {
                request.Status = 3;
                request.CaseTag = reason;


                RequestStatusLog requeststatuslog = new RequestStatusLog();

                requeststatuslog.RequestId = requesid;
                requeststatuslog.Notes = Notes;
                requeststatuslog.CreatedDate = DateTime.Now;
                requeststatuslog.Status = 3;

                _context.Add(requeststatuslog);
                _context.SaveChanges();

                _context.Update(request);
                _context.SaveChanges();
            }
        }
        public List<Physician> GetPhysicianByRegion(string RegionId)
        {
            var result = (from physician in _context.Physicians
                          join region in _context.PhysicianRegions on
                            physician.PhysicianId equals region.PhysicianId into phy
                          select physician).Where(s => s.RegionId == int.Parse(RegionId)).ToList();

            return result;

        }
        public ViewNotesVM viewnotes(int id)
        {
            ViewNotesVM model = new ViewNotesVM();
            var transferedNotes = (from reqlog in _context.RequestStatusLogs
                                   where reqlog.RequestId == id
                                   select reqlog.Notes).ToList();
            model.TransferNotes = transferedNotes;
            model.AdminNotes = _context.RequestNotes.FirstOrDefault(s => s.RequestId == id).AdminNotes;
            model.PhysicianNotes = _context.RequestNotes.FirstOrDefault(s => s.RequestId == id).PhysicianNotes;
            model.RequestId = id;
            return model;
        }
        public void BlockCase(int requestId, string reason)
        {
            var request = _context.Requests.Where(h => h.RequestId == requestId).First();
            var requestClient = _context.RequestClients.Where(r => r.RequestId == requestId).First();

            if(request != null && requestClient != null)
            {
                BlockRequest blockmodel = new BlockRequest();
                blockmodel.RequestId = requestId.ToString();
                blockmodel.Reason = reason;
                blockmodel.CreatedDate = request.CreatedDate;
                blockmodel.ModifiedDate = DateTime.Now;
                blockmodel.Email = requestClient.Email;
                blockmodel.PhoneNumber = requestClient.PhoneNumber;

                RequestStatusLog requeststatuslog = new RequestStatusLog();

                requeststatuslog.RequestId = requestId;
                requeststatuslog.Notes = reason;
                requeststatuslog.CreatedDate = DateTime.Now;
                requeststatuslog.Status = 11;

                request.Status = 11;
                
                _context.BlockRequests.Add(blockmodel); 
                _context.RequestStatusLogs.Add(requeststatuslog);
                _context.Requests.Update(request);
                _context.SaveChanges();
            }
        }

        public List<HealthProfessional> getVenbypro(string ProfessionId)
        {
            var result = _context.HealthProfessionals.Where(u => u.Profession == int.Parse(ProfessionId)).ToList();
            //    var result = (from vendors in _context.HealthProfessionals
            //                 join prof in _context.HealthProfessionalTypes on
            //                 vendors.Profession equals prof.HealthProfessionalId into professionalTypes
            //                 select vendors).Where(u => u.Profession == int.Parse(ProfessionId)).ToList();
            return result;
        }

        public bool transferNotes(int requestid, int phyid, string transnotes)
        {
            var request = _context.Requests.Where(u => u.RequestId == requestid).FirstOrDefault();
            if (request != null)
            {
                request.ModifiedDate = DateTime.Now;
                request.PhysicianId = phyid;
                _context.Requests.Update(request);
                _context.SaveChanges();

                RequestStatusLog log = new RequestStatusLog();
                log.Status = request.Status;
                log.RequestId = requestid;
                log.PhysicianId = phyid;
                log.Notes = transnotes;
                log.CreatedDate = DateTime.Now;
                _context.RequestStatusLogs.Add(log);
                _context.SaveChanges();

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ClearCase(int requestId)
        {
            var request = _context.Requests.Where(u => u.RequestId == requestId).FirstOrDefault();

            if (request != null)
            {
                request.Status = 10;
                request.ModifiedDate = DateTime.Now;

                _context.Requests.Update(request);
                _context.SaveChanges();

                RequestStatusLog log = new RequestStatusLog
                {
                    RequestId = requestId,
                    CreatedDate = DateTime.Now,
                    Status = 10,

                };
                _context.RequestStatusLogs.Add(log);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public CloseCaseVM closecasegetdata(CloseCaseVM model, int reqid)
        {
            var filedetails = _context.RequestWiseFiles.Where(u => u.RequestId == reqid).ToList();
            var client = _context.RequestClients.Where(u => u.RequestId == reqid).FirstOrDefault();
            var request = _context.Requests.FirstOrDefault(u => u.RequestId == reqid);
            if (request != null )
            {
                model.Files = filedetails;
                model.FirstName = client.FirstName;
                model.LastName = client.LastName;
                model.Email = client.Email;
                model.Phonenum = client.PhoneNumber;
                model.DateOfBirth = new DateOnly((int)client.IntYear, int.Parse(client.StrMonth), (int)client.IntDate);
                model.ConfirmationNum = request.ConfirmationNumber.ToUpper();
                model.requestid = reqid;
            }
            return model;
        }
    }
}
