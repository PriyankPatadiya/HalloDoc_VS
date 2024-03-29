﻿using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;

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
    }
}
