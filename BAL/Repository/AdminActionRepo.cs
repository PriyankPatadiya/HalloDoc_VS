using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Abstractions;
using Org.BouncyCastle.Asn1.Mozilla;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using System.Collections;

namespace BAL.Repository
{
    public class AdminActionRepo : IAdminActions
    {

        private readonly ApplicationDbContext _context;

        public AdminActionRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public RequestNote reqnotebyreqid(int requestid)
        {
            RequestNote? notes = _context.RequestNotes.FirstOrDefault(u => u.RequestId == requestid);
            if(notes != null)
            {
                return notes;
            }
            else
            {
                return new RequestNote();
            }

        }
        public List<RequestClient> clientsbyreqid(int requestid)
        {
            var clients = _context.RequestClients.Where(r => r.RequestId == requestid).ToList();
            return clients;
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

        public bool changeStatusOnCancleCase(int requesid, string reason, string Notes)
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
                return true;
            }
            return false;
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
            try
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
            catch (Exception ex)
            {
                throw new Exception("",ex);
            }
            
        }

        public void addrequnotes(ViewNotesVM model, RequestNote notes)
        {
            notes.AdminNotes = model.AdminNotes;
            _context.RequestNotes.Update(notes);
            _context.SaveChanges();
        }

        public void BlockCase(int requestId, string reason)
        {
            var request = _context.Requests.Where(h => h.RequestId == requestId).First();
            var requestClient = _context.RequestClients.Where(r => r.RequestId == requestId).First();

            if(request != null && requestClient != null)
            {
                BlockRequest blockmodel = new BlockRequest();
                blockmodel.RequestId = requestId;
                blockmodel.Reason = reason;
                blockmodel.CreatedDate = request.CreatedDate;
                blockmodel.ModifiedDate = DateTime.Now;
                blockmodel.Email = requestClient.Email;
                blockmodel.PhoneNumber = requestClient.PhoneNumber;
                blockmodel.IsActive = new BitArray(new[] { false });

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

        public bool closecase(CloseCaseVM model)
        {
            var client = _context.RequestClients.Where(u => u.RequestId == model.requestid).FirstOrDefault();
            if (client != null)
            {
                client.FirstName = model.FirstName;
                client.LastName = model.LastName;
                client.PhoneNumber = model.Phonenum;
                client.IntDate = model.DateOfBirth.Day;
                client.IntYear = model.DateOfBirth.Year;
                client.StrMonth = model.DateOfBirth.Month.ToString();
                _context.RequestClients.Update(client);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public void closeRequest(DAL.DataModels.Request request, int reqid)
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
        }

        public List<EncounterFormVM> getEncounterformdata(int requestid)
        {

            var model = (from req in _context.Requests
                                     join
                                    reqclient in _context.RequestClients on
                                    req.RequestId equals reqclient.RequestId
                                     join encounter in _context.EncounterForms on
                                     req.RequestId equals encounter.RequestId
                                     into total
                                     from encounter in total.DefaultIfEmpty()
                                     where req.RequestId == requestid
                                     select new EncounterFormVM
                                     {
                                         FirstName = reqclient.FirstName,
                                         LastName = reqclient.LastName,
                                         //Location = reqclient.Location,
                                         BirthDate = new DateTime((int)reqclient.IntYear, int.Parse(reqclient.StrMonth), (int)reqclient.IntDate),
                                         //ServiceDate = (DateTime)req.ModifiedDate,
                                         IllnessOrInjury = encounter.HistoryOfPresentIllnessOrInjury,
                                         MedicalHistory = encounter.MedicalHistory,
                                         Medications = encounter.Medications,
                                         Allergies = encounter.Allergies,
                                         Temprature = encounter.Temp,
                                         HR = encounter.Hr,
                                         RR = encounter.Rr,
                                         SytolicBp = encounter.BloodPressureSystolic,
                                         DistolicBp = encounter.BloodPressureDiastolic,
                                         O2 = encounter.O2,
                                         Pain = encounter.Pain,
                                         Heent = encounter.Heent,
                                         Cv = encounter.Cv,
                                         Chest = encounter.Chest,
                                         ABD = encounter.Abd,
                                         Extr = encounter.Extremeties,
                                         Skin = encounter.Skin,
                                         Neuro = encounter.Neuro,
                                         Other = encounter.Other,
                                         Dignosis = encounter.Diagnosis,
                                         TreatmentPlan = encounter.TreatmentPlan,
                                         MedicationDispensed = encounter.MedicationsDispensed,
                                         Procedures = encounter.Procedures,
                                         Followup = encounter.FollowUp,
                                         requestid = requestid
                                     }).ToList();

            return model;

        }

        public void addencounterdata(EncounterFormVM model)
        {
            var encounter = _context.EncounterForms.FirstOrDefault(u => u.RequestId == model.requestid);
            if(encounter == null)
            {
                EncounterForm data = new EncounterForm
                {
                    RequestId = model.requestid,
                    HistoryOfPresentIllnessOrInjury = model.IllnessOrInjury,
                    MedicalHistory = model.MedicalHistory,
                    Medications = model.Medications,
                    Allergies = model.Allergies,
                    Temp = model.Temprature,
                    Hr = model.HR,
                    Rr = model.RR,
                    BloodPressureDiastolic = model.DistolicBp,
                    BloodPressureSystolic = model.SytolicBp,
                    O2 = model.O2,
                    Pain = model.Pain,
                    Heent = model.Heent,
                    Cv = model.Cv,
                    Chest = model.Chest,
                    Abd = model.ABD,
                    Extremeties = model.Extr,
                    Skin = model.Skin,
                    Neuro = model.Neuro,
                    Other = model.Other,
                    Diagnosis = model.Dignosis,
                    TreatmentPlan = model.TreatmentPlan,
                    MedicationsDispensed = model.MedicationDispensed,
                    Procedures = model.Procedures,
                    FollowUp = model.Followup,
                };
                _context.EncounterForms.Add(data);
                _context.SaveChanges();
            }
            else
            {
                    encounter.HistoryOfPresentIllnessOrInjury = model.IllnessOrInjury;
                    encounter.MedicalHistory = model.MedicalHistory;
                    encounter.Medications = model.Medications;
                    encounter.Allergies = model.Allergies;
                    encounter.Temp = model.Temprature;
                    encounter.Hr = model.HR;
                    encounter.Rr = model.RR;
                    encounter.BloodPressureDiastolic = model.DistolicBp;
                    encounter.BloodPressureSystolic = model.SytolicBp;
                    encounter.O2 = model.O2;
                    encounter.Pain = model.Pain;
                    encounter.Heent = model.Heent;
                    encounter.Cv = model.Cv;
                    encounter.Chest = model.Chest;
                    encounter.Abd = model.ABD;
                    encounter.Extremeties = model.Extr;
                    encounter.Skin = model.Skin;
                    encounter.Neuro = model.Neuro;
                    encounter.Other = model.Other;
                    encounter.Diagnosis = model.Dignosis;
                    encounter.TreatmentPlan = model.TreatmentPlan;
                    encounter.MedicationsDispensed = model.MedicationDispensed;
                    encounter.Procedures = model.Procedures;
                    encounter.FollowUp = model.Followup;
                _context.EncounterForms.Update(encounter);
                _context.SaveChanges();
            }
        }

        public bool finalize(int requestid)
        {
            var encounter = _context.EncounterForms.FirstOrDefault(u => u.RequestId == requestid);
            if (encounter != null)
            {
                encounter.IsFinalize = true;
                _context.EncounterForms.Update(encounter);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<HealthProfessionalType> healthProfessionalTypes()
        {
            var healthpros = _context.HealthProfessionalTypes.ToList();
            return healthpros;
        }

        public bool sendOrder(SendOrderVM model)
        {
            if (model != null)
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
                return true;
            }
            return false;
        }

        public List<SchedulingVM> getEvents(int regionId)
        {
            var events = (from s in _context.Shifts
                          join pd in _context.Physicians on s.PhysicianId equals pd.PhysicianId
                          join sd in _context.ShiftDetails on s.ShiftId equals sd.ShiftId into shiftGroup
                          from sd in shiftGroup.DefaultIfEmpty()
                          select new SchedulingVM
                          {
                              title = string.Concat(sd.StartTime, " ", sd.EndTime, " ", pd.FirstName, " ", pd.LastName),
                              Shiftid = sd.ShiftDetailId,
                              Startdate = sd.ShiftDate.Date.Add(sd.StartTime.ToTimeSpan()),
                              Enddate = sd.ShiftDate.Date.Add(sd.EndTime.ToTimeSpan()),
                              Status = sd.Status,
                              Physicianid = pd.PhysicianId,
                              PhysicianName = pd.FirstName + ' ' + pd.LastName,
                              Shiftdate = sd.ShiftDate,
                              ShiftDetailId = sd.ShiftDetailId,
                              Regionid = sd.RegionId,
                              ShiftDeleted = sd.IsDeleted[0]

                          }).Where(item => regionId == 0 || item.Regionid == regionId).ToList();
            events = events.Where(item => !item.ShiftDeleted).ToList();

            return events;
        }
    }
}

