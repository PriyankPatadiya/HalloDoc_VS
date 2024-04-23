using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using System.Numerics;

namespace BAL.Repository
{
    public class ProviderSite : IProviderSite
    {
        private readonly ApplicationDbContext _context;
        private readonly IProviders _provider;
        public ProviderSite(ApplicationDbContext context, IProviders provider)
        {
            _context = context;
            _provider = provider;
        }

        public int CountRequests(string StatusButton, int? physicianId)
        {
            List<int> myarray = new List<int>();

            if (StatusButton == "1")
            {
                myarray.Add(1);
            }
            if (StatusButton == "2")
            {
                myarray.Add(2);
            }
            if (StatusButton == "3")
            {
                myarray.Add(4);
                myarray.Add(5);
            }
            if (StatusButton == "4")
            {
                myarray.Add(6);
            }
            if (StatusButton == "5")
            {
                myarray.Add(3);
                myarray.Add(7);
                myarray.Add(8);
            }
            if (StatusButton == "6")
            {
                myarray.Add(9);
            }
            var count = (from req in _context.Requests
                         join reqclient in _context.RequestClients
                         on req.RequestId equals reqclient.RequestId
                         where (req.PhysicianId == physicianId) && !(req.IsDeleted == new System.Collections.BitArray(new bool[] { true }))
                         select req.Status).Where(item => myarray.Any(s => item == s)).Count();
            return count;
        }
        public PhysicianProfileVM getProviderProfileData(Physician? physician)
        {
            PhysicianProfileVM physicanProfile = new PhysicianProfileVM();
            physicanProfile.FirstName = physician.FirstName;
            physicanProfile.LastName = physician.LastName ?? "";
            physicanProfile.Email = physician.Email;
            physicanProfile.Address1 = physician.Address1 ?? "";
            physicanProfile.Address2 = physician.Address2 ?? "";
            physicanProfile.City = physician.City ?? "";
            physicanProfile.ZipCode = physician.Zip ?? "";
            physicanProfile.MobileNo = physician.Mobile ?? "";
            physicanProfile.MedicalLicense = physician.MedicalLicense;
            physicanProfile.NPINumber = physician.Npinumber;
            physicanProfile.SynchronizationEmail = physician.SyncEmailAddress;
            physicanProfile.physicianid = physician.PhysicianId;
            physicanProfile.WorkingRegions = _provider.getList(physician.PhysicianId);
            physicanProfile.State = physician.RegionId;
            physicanProfile.SignatureFilename = physician.Signature;
            physicanProfile.BusinessWebsite = physician.BusinessWebsite;
            physicanProfile.BusinessName = physician.BusinessName;
            physicanProfile.PhotoFileName = physician.Photo;
            physicanProfile.IsAgreement = physician.IsAgreementDoc;
            physicanProfile.IsBackground = physician.IsBackgroundDoc;
            physicanProfile.IsHippa = physician.IsAgreementDoc;
            physicanProfile.NonDiscoluser = physician.IsNonDisclosureDoc;
            physicanProfile.License = physician.IsLicenseDoc;
            return physicanProfile;
        }

        public List<PartnersVM> getVendors(string search, string regionId)
        {
            return  (from professions in _context.HealthProfessionals
                                         join type in _context.HealthProfessionalTypes
                                         on professions.Profession equals type.HealthProfessionalId
                                         select new PartnersVM()
                                         {
                                             profession = type.ProfessionName,
                                             vendorid = professions.VendorId,
                                             CreatedDate = professions.CreatedDate,
                                             BusinessName = professions.VendorName,
                                             Email = professions.Email,
                                             Fax = professions.FaxNumber,
                                             phoneNumber = professions.PhoneNumber,
                                             BusinessContact = professions.BusinessContact,
                                             regionId = professions.RegionId

                                         }).Where(s => (String.IsNullOrEmpty(search) || s.BusinessName.Contains(search)) && (regionId == "0" || s.regionId == int.Parse(regionId))).ToList();
        }

        public void addBusiness(AddBusinessVM vendor)
        {
            HealthProfessional model = new HealthProfessional()
            {
                VendorName = vendor.BusinessName,
                Profession = vendor.SelecteProfession,
                FaxNumber = vendor.FaxNumber,
                Address = vendor.StreetAddress + ", " + vendor.City + ", " + _context.Regions.Where(u => u.RegionId == vendor.StateId).First().Name,
                City = vendor.City,
                State = _context.Regions.Where(u => u.RegionId == vendor.StateId).First().Name,
                Zip = vendor.ZipCode,
                RegionId = vendor.StateId,
                CreatedDate = DateTime.Now,
                PhoneNumber = vendor.PhoneNumber,
                Email = vendor.EmailAddress,
                BusinessContact = vendor.BusinessContact
            };
            _context.HealthProfessionals.Add(model);
            _context.SaveChanges();
        }

        public AddBusinessVM getBusinessDetails(int Vendorid)
        {
            var Vendor = _context.HealthProfessionals.First(u => u.VendorId == Vendorid);
            AddBusinessVM model = new AddBusinessVM()
            {
                vendorId = Vendor.VendorId,
                BusinessName = Vendor.VendorName,
                FaxNumber = Vendor.FaxNumber,
                PhoneNumber = Vendor.PhoneNumber,
                EmailAddress = Vendor.Email,
                BusinessContact = Vendor.BusinessContact,
                StateId = Vendor.RegionId,
                ZipCode = Vendor.Zip,
                City = Vendor.City,
                SelecteProfession = Vendor.Profession
            };
            return model;
        }
        public void updateBusinessDetails(AddBusinessVM vendor, HealthProfessional Vendor)
        {
            Vendor.VendorName = vendor.BusinessName;
            Vendor.Profession = vendor.SelecteProfession;
            Vendor.FaxNumber = vendor.FaxNumber;
            Vendor.Address = vendor.City + ", " + _context.Regions.Where(u => u.RegionId == vendor.StateId).First().Name;
            Vendor.City = vendor.City;
            Vendor.State = _context.Regions.Where(u => u.RegionId == vendor.StateId).First().Name;
            Vendor.Zip = vendor.ZipCode;
            Vendor.RegionId = vendor.StateId;
            Vendor.CreatedDate = DateTime.Now;
            Vendor.PhoneNumber = vendor.PhoneNumber;
            Vendor.Email = vendor.EmailAddress;
            Vendor.BusinessContact = vendor.BusinessContact;
            _context.HealthProfessionals.Update(Vendor);
            _context.SaveChanges();
        }

        public List<User> getFileteredPatient(string firstName, string lastName, string email, string phoneNumber)
        {
            return _context.Users.ToList().Where(u => (String.IsNullOrEmpty(firstName) || u.FirstName.Contains(firstName)) &&
            (String.IsNullOrEmpty(lastName) || u.LastName.Contains(lastName)) &&
            (String.IsNullOrEmpty(email) || u.Email.Contains(email)) &&
            (String.IsNullOrEmpty(phoneNumber) || u.Mobile.Contains(phoneNumber))).ToList();
        }
        public List<explorePatientVM> getExploreRequests(int userId)
        {
            return (from request in _context.Requests
             join requestClient in _context.RequestClients
             on request.RequestId equals requestClient.RequestId
             join physician in _context.Physicians
             on request.PhysicianId equals physician.PhysicianId into totalphy
             from leftphy in totalphy.DefaultIfEmpty()
             join Encounter in _context.EncounterForms
             on request.RequestId equals Encounter.RequestId into totalEnc
             from encLeft in totalEnc.DefaultIfEmpty()
             where request.UserId == userId
             select new explorePatientVM()
             {
                 FirstName = request.FirstName + " " + request.LastName,
                 RequestId = request.RequestId,
                 CreatedDate = request.CreatedDate,
                 ConfirmationNumber = request.ConfirmationNumber,
                 ProvideName = leftphy.FirstName,
                 ConcludeDate = _context.RequestStatusLogs.FirstOrDefault(u => u.RequestId == request.RequestId && u.Status == 9).CreatedDate,
                 IsFinalize = encLeft.IsFinalize,
                 RequestClientId = requestClient.RequestClientId
             }).ToList();
        }
        public IEnumerable<SearchRecordsVM> getSearchRecordsWithFilter(int[] status, string patientName,
        string providerName, string phoneNum, string email, string requestType, DateTime fromDate, DateTime toDate)
        {
            var record = (from request in _context.Requests
                          join requestclient in _context.RequestClients
                          on request.RequestId equals requestclient.RequestId
                          join physician in _context.Physicians
                          on request.PhysicianId equals physician.PhysicianId into physicians
                          from allphysician in physicians.DefaultIfEmpty()
                          where !(request.IsDeleted == new System.Collections.BitArray(new bool[] { true }))
                          select new
                          {
                              Request = request,
                              RequestClient = requestclient,
                              Physician = allphysician,

                          }).ToList();

            var searchedRecord = record.Select(item =>
            {
                var requestNotes = _context.RequestNotes.FirstOrDefault(u => u.RequestId == item.Request.RequestId);
                string? physicianNotes = requestNotes == null ? "-" : requestNotes.PhysicianNotes;
                string? adminNotes = requestNotes == null ? "-" : requestNotes.AdminNotes;
                
                return new SearchRecordsVM
                {
                    PatientName = item.RequestClient.FirstName + " " + item.RequestClient.LastName,
                    Requestor = item.Request.FirstName + " " + item.Request.LastName,
                    //DateOfService = ,
                    //CloseDate = ,
                    Email = item.RequestClient.Email,
                    PhoneNumber = item.RequestClient.PhoneNumber,
                    Address = item.RequestClient.Address,
                    Zip = item.RequestClient.ZipCode,
                    RequestStatus = item.Request.Status,
                    PhysicianName = item.Physician != null ? item.Physician.FirstName : " ",
                    PatientNote = item.RequestClient.Notes,
                    PhysicianNote = physicianNotes,
                    AdminNotes = adminNotes,
                    RequestTypeId = item.Request.RequestTypeId,
                    RequestId = item.Request.RequestId,
                    IsDeleted = item.Request.IsDeleted[0],
                };
            }).Where(item =>
            (string.IsNullOrEmpty(email) || item.Email.Contains(email)) &&
            (string.IsNullOrEmpty(phoneNum) || item.PhoneNumber.Contains(phoneNum)) &&
            (string.IsNullOrEmpty(patientName) || item.PatientName.ToLower().Contains(patientName.ToLower())) &&
            (string.IsNullOrEmpty(providerName) || item.PhysicianName.ToLower().Contains(providerName.ToLower())) &&
            (status.Length == 0 || status.Contains(item.RequestStatus)) &&
            (requestType == "0" || item.RequestTypeId == int.Parse(requestType))).ToList();
            return searchedRecord;
        }


        public IQueryable<BlockedHistoryVM> getBlockedRequests()
        {
            return (from blockedrequest in _context.BlockRequests
                    join reqclient in _context.RequestClients on blockedrequest.RequestId.ToString() equals reqclient.RequestId.ToString()
                    select new BlockedHistoryVM()
                    {
                        BlockedRequestID = blockedrequest.BlockRequestId,
                        RequestId = blockedrequest.RequestId,
                        PatientName = reqclient.FirstName,
                        CreatedDate = blockedrequest.CreatedDate,
                        PhoneNumber = blockedrequest.PhoneNumber,
                        Email = blockedrequest.Email,
                        Notes = reqclient.Notes,
                        IsActive = blockedrequest.IsActive[0]
                    });
        }

        public void deleteBusiness(HealthProfessional vendor)
        {
            _context.Remove(vendor);
            _context.SaveChanges();
        }
    }
}
