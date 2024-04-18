using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;

namespace BAL.Repository
{
    public class CreateAccRepo : ICreateAcc
    {
        private readonly ApplicationDbContext _context;
        public CreateAccRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddUser(CreateAccVM Obj, AspNetUser aspnetuser)
        {
            var client = _context.RequestClients.FirstOrDefault(u => u.Email == Obj.UserName);
            


            aspnetuser.Id = Guid.NewGuid().ToString();
            aspnetuser.Email = Obj.UserName;
            aspnetuser.CreatedDate = DateTime.Now;
            aspnetuser.UserName = Obj.UserName;
            
            aspnetuser.PhoneNumber = client.PhoneNumber;


            _context.AspNetUsers.Add(aspnetuser);
            _context.SaveChanges();

            var user = new User();
            user.AspNetUserId = aspnetuser.Id;
            user.FirstName = client.FirstName;
            user.LastName = client.LastName;
            user.Mobile = client.PhoneNumber;
            user.Street = client.Street;
            user.City = client.City;
            user.State = client.State;
            user.ZipCode = client.ZipCode;
            user.Email = Obj.UserName;
            user.CreatedBy = client.FirstName;
            user.IntYear = client.IntYear;
            user.IntDate = client.IntDate;
            user.StrMonth = client.StrMonth;
            user.RegionId = client.RegionId;

            _context.Users.Add(user);
            _context.SaveChanges();

            var aspnetuserrole = new AspNetUserRole
            {
                UserId = aspnetuser.Id,
                RoleId = 2
            };

            _context.AspNetUserRoles.Add(aspnetuserrole);
            _context.SaveChanges();
        }
    }
}
