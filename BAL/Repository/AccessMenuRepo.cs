using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using System.Data;

namespace BAL.Repository
{
    public class AccessMenuRepo : IAccessMenu
    {
        private readonly ApplicationDbContext _context;
        public AccessMenuRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<UserAccessVM> getUserAccessData(int accId)
            {
            var result = (from aspuser in _context.AspNetUsers
                         join aspnetuserrole in _context.AspNetUserRoles 
                         on aspuser.Id equals aspnetuserrole.UserId 
                         join aspnetrole in _context.AspNetRoles
                         on aspnetuserrole.RoleId equals aspnetrole.Id 
                         join phy in _context.Physicians
                         on aspuser.Id equals phy.AspNetUserId into phyusers
                         from totaluser in phyusers.DefaultIfEmpty()
                         join admin in _context.Admins
                         on aspuser.Id equals admin.AspNetUserId into admins
                         from totaladmins in admins.DefaultIfEmpty()
                         where (accId == 0 || aspnetuserrole.RoleId == accId)
                         select(aspnetuserrole.RoleId == 1? new UserAccessVM()
                         {
                             AccountType = aspnetrole.Name,
                             AccountPOC = aspuser.UserName,
                             phone = totaladmins.Mobile,
                             status = totaladmins.AdminId,
                             roleid = aspnetuserrole.RoleId,
                             AccountTypeid = aspnetuserrole.RoleId,
                             useraccessid = totaladmins.AdminId,
                         }: new UserAccessVM()
                         {
                             AccountType = aspnetrole.Name,
                             AccountPOC = aspuser.UserName,
                             phone = totaluser.Mobile,
                             status = totaluser.Status,
                             roleid = aspnetuserrole.RoleId,
                             AccountTypeid = aspnetuserrole.RoleId,
                             useraccessid = totaluser.PhysicianId,
                         }

                         )).ToList();

            return result;
        }
    }
}
