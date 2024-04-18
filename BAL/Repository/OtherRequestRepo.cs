using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using System.Collections;

namespace BAL.Repository
{
    public class OtherRequestRepo : IRequests
    {
        private readonly ApplicationDbContext _context;
        public OtherRequestRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        //Family-Friend Form

        public void AddFamilyFriendForm(OthersReqVM model)
        {
            var request = new Request();
            var requestClient = new RequestClient();

            request.RequestTypeId = 2;
            request.FirstName = model.YourFirstName;
            request.LastName = model.YourLastName;
            request.Email = model.YourEmail;
            request.PhoneNumber = model.PhoneNumber;
            request.RelationName = model.Relation;
            request.CreatedDate = DateTime.Now;
            request.ConfirmationNumber = model.confirmationnumber;
            request.IsDeleted = new BitArray(new bool[] { true });
            _context.Requests.Add(request);
            _context.SaveChanges();

            requestClient.RequestId = request.RequestId;
            requestClient.FirstName = model.FirstName;
            requestClient.LastName = model.LastName;
            requestClient.Email = model.Email;
            requestClient.PhoneNumber = model.PhoneNumber;
            requestClient.StrMonth = model.BirthDate.Month.ToString();
            requestClient.IntYear = model.BirthDate.Year;
            requestClient.IntDate = model.BirthDate.Day;
            requestClient.Street = model.Street;
            requestClient.City = model.City;
            requestClient.State = model.State;
            requestClient.ZipCode = model.Zipcode;
            requestClient.RegionId = model.SelectedStateId;
            _context.RequestClients.Add(requestClient);
            _context.SaveChanges();

            var reqnotes = new RequestNote();
            reqnotes.RequestId = request.RequestId;
            reqnotes.AdminNotes = "-";
            reqnotes.CreatedDate = DateTime.Now;
            reqnotes.CreatedBy = request.FirstName + request.LastName;
            reqnotes.PhysicianNotes = "-";
            _context.RequestNotes.Add(reqnotes);
            _context.SaveChanges();
        }

        // Adding Concierge Request

        public void AddConciergeForm(OthersReqVM model)
        {
            var request = new Request();
            var requestClient = new RequestClient();
            var concierge = new Concierge();

            request.RequestTypeId = 3;
            request.FirstName = model.YourFirstName;
            request.LastName = model.YourLastName;
            request.Email = model.YourEmail;
            request.PhoneNumber = model.YourPhoneNumber;
            request.CreatedDate = DateTime.Now;
            request.ConfirmationNumber = model.confirmationnumber;
            request.IsDeleted = new BitArray(new bool[] { true });
            _context.Requests.Add(request);
            _context.SaveChanges();

            requestClient.RequestId = request.RequestId;
            requestClient.FirstName = model.FirstName;
            requestClient.LastName = model.LastName;
            requestClient.Email = model.Email;
            requestClient.PhoneNumber = model.PhoneNumber;
            requestClient.StrMonth = model.BirthDate.Month.ToString();
            requestClient.IntYear = model.BirthDate.Year;
            requestClient.IntDate = model.BirthDate.Day;
            requestClient.Street = model.Street;
            requestClient.City = model.City;
            requestClient.State = model.State;
            requestClient.ZipCode = model.Zipcode;
            requestClient.RegionId = model.SelectedStateId;
            _context.RequestClients.Add(requestClient);
            _context.SaveChanges();

            concierge.RegionId = model.SelectedStateId;
            concierge.ConciergeName = model.YourFirstName + model.YourLastName;
            concierge.Address = model.Street + model.City + model.State + model.Zipcode;
            concierge.Street = model.Street;
            concierge.City = model.City;
            concierge.State = model.State;
            concierge.ZipCode = model.Zipcode;
            _context.Concierges.Add(concierge);
            _context.SaveChanges();

            var reqnotes = new RequestNote();
            reqnotes.RequestId = request.RequestId;
            reqnotes.AdminNotes = "-";
            reqnotes.CreatedDate = DateTime.Now;
            reqnotes.CreatedBy = request.FirstName + request.LastName;
            reqnotes.PhysicianNotes = "-";
            _context.RequestNotes.Add(reqnotes);
            _context.SaveChanges();
        }

        //  Business Form Request

        public void AddBusinessForm(OthersReqVM model)
        {
            var request = new Request();
            var requestClient = new RequestClient();
            var Business = new Business();

            request.RequestTypeId = 4;
            request.FirstName = model.YourFirstName;
            request.LastName = model.YourLastName;
            request.Email = model.YourEmail;
            request.PhoneNumber = model.PhoneNumber;
            request.CreatedDate = DateTime.Now;
            request.ConfirmationNumber = model.confirmationnumber;
            request.IsDeleted = new BitArray(new bool[] { true });
            _context.Requests.Add(request);
            _context.SaveChanges();

            requestClient.RequestId = request.RequestId;
            requestClient.FirstName = model.FirstName;
            requestClient.LastName = model.LastName;
            requestClient.Email = model.Email;
            requestClient.PhoneNumber = model.PhoneNumber;
            requestClient.StrMonth = model.BirthDate.Month.ToString();
            requestClient.IntYear = model.BirthDate.Year;
            requestClient.IntDate = model.BirthDate.Day;
            requestClient.Street = model.Street;
            requestClient.City = model.City;
            requestClient.State = model.State;
            requestClient.ZipCode = model.Zipcode;
            requestClient.RegionId = model.SelectedStateId;
            _context.RequestClients.Add(requestClient);
            _context.SaveChanges();

            Business.RegionId = model.SelectedStateId;
            Business.Name = model.BusinessName;
            Business.Address1 = model.Street + model.City;
            Business.Address2 = model.State + model.Zipcode;
            Business.CreatedDate = DateTime.Now;
            _context.Businesses.Add(Business);
            _context.SaveChanges();

            RequestBusiness requestBusiness = new()
            {
                RequestId = request.RequestId,
                BusinessId = Business.BusinessId,
            };
            _context.RequestBusinesses.Add(requestBusiness);
            _context.SaveChanges();

            var reqnotes = new RequestNote();
            reqnotes.RequestId = request.RequestId;
            reqnotes.AdminNotes = "-";
            reqnotes.CreatedDate = DateTime.Now;
            reqnotes.CreatedBy = request.FirstName + request.LastName;
            reqnotes.PhysicianNotes = "-";
            _context.RequestNotes.Add(reqnotes);
            _context.SaveChanges();
        }

        public string GenerateConfirmationNumber(OthersReqVM model)
        {
            string abr = _context.Regions.Where(s => s.Name == model.State).FirstOrDefault().Abbreviation;
            int numofrequests = _context.Requests.Where(s => s.CreatedDate.Date == model.CreatedDate).Count();
            string confirmationnum = abr + model.CreatedDate.Value.ToString("MMdd") + model.LastName.Substring(0, 2) + model.FirstName.Substring(0, 2) + numofrequests.ToString("D4");
            return confirmationnum;
        }

        public void AggrementReview(Request request, int requestid)
        {
            request.Status = 4;
            request.ModifiedDate = DateTime.Now;
            _context.Requests.Update(request);
            _context.SaveChanges();

            RequestStatusLog model = new RequestStatusLog();
            model.Status = request.Status;
            model.RequestId = requestid;
            model.CreatedDate = DateTime.Now;

            _context.RequestStatusLogs.Add(model);
            _context.SaveChanges();
        }

        public void cancelAgreement(int requestid, string PatientNote , Request request)
        {
            request.Status = 7;
            request.ModifiedDate = DateTime.Now;
            _context.Requests.Update(request);
            _context.SaveChanges();

            RequestStatusLog model = new RequestStatusLog();
            model.Status = request.Status;
            model.RequestId = requestid;
            model.Notes = PatientNote;
            model.CreatedDate = DateTime.Now;
            _context.RequestStatusLogs.Add(model);
            _context.SaveChanges();
        }
        public PatientDashboardVM getPatientDashboardData(string email)
        {
            var Dashboard = (from req in _context.Requests
                             join reqclient in _context.RequestClients on req.RequestId equals reqclient.RequestId
                             join requestfile in _context.RequestWiseFiles on req.RequestId equals requestfile.RequestId
                             into reqs
                             where reqclient.Email == email
                             from requestfile
                             in reqs.DefaultIfEmpty()
                             select new PatDashTableVM
                             {

                                 currentStatus = req.Status,
                                 CreatedDate = req.CreatedDate,
                                 Document = requestfile.FileName == null ? null : requestfile.FileName,
                                 requestid = req.RequestId,
                                 count = _context.RequestWiseFiles.Count(u => u.RequestId == req.RequestId),

                             }).ToList();

            var profiledata = (from user in _context.Users
                               where user.Email == email
                               select new ProfilePatient
                               {
                                   FirstName = user.FirstName,
                                   LastName = user.LastName,
                                   BirthDate = new DateTime((int)user.IntYear, int.Parse(user.StrMonth), (int)user.IntDate),
                                   PhoneNumber = user.Mobile,
                                   Email = user.Email,
                                   Street = user.Street,
                                   City = user.City,
                                   State = user.State,
                                   ZipCode = user.ZipCode

                               }).FirstOrDefault();

            PatientDashboardVM patientDashboardVM = new PatientDashboardVM();

            patientDashboardVM.DashTable = Dashboard;
            patientDashboardVM.ProfilePatient = (ProfilePatient)profiledata;
            return patientDashboardVM;
        }

        public void EditPatientProfile(ProfilePatient profilepatient)
        {
            var user = _context.Users.Where(u => u.Email == profilepatient.Email).FirstOrDefault();
            user.FirstName = profilepatient.FirstName;
            user.LastName = profilepatient.LastName;
            user.Email = profilepatient.Email;
            user.Street = profilepatient.Street;
            user.Mobile = profilepatient.PhoneNumber;
            user.City = profilepatient.City;
            user.State = profilepatient.State;
            user.ZipCode = profilepatient.ZipCode;
            user.IntDate = profilepatient.BirthDate.Day;
            user.IntYear = profilepatient.BirthDate.Year;
            user.StrMonth = profilepatient.BirthDate.Month.ToString();

            _context.Update(user);
            _context.SaveChanges();
        }
    }
}
