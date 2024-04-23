using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using System.Collections;
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

        public List<Role> getRole()
        {
            return _context.Roles.ToList();
        }
        public EditRoleAccessVM editRoleAccess(int roleid, Role role)
        {
            EditRoleAccessVM model = new EditRoleAccessVM();
            model.roleid = roleid;
            model.Name = role.Name;
            model.accountType = role.AccountType;
            model.Menus = _context.Menus.Where(u => u.AccountType == role.AccountType).ToList();
            model.selectedmenus = _context.RoleMenus.Where(u => u.RoleId == roleid).ToList();
            return model;
        }
        public List<Menu> getMenubyRole(int role)
        {
            return _context.Menus.Where(item => role == 0 || item.AccountType == role).ToList();
        }
        public Role getRoleByRoleId(int roleid)
        {
            return _context.Roles.Where(u => u.RoleId == roleid).First();
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
                          from totalphy in phyusers.DefaultIfEmpty()
                          join admin in _context.Admins
                          on aspuser.Id equals admin.AspNetUserId into admins
                          from totaladmins in admins.DefaultIfEmpty()
                          where (accId == 0 || aspnetuserrole.RoleId == accId)
                          select (aspnetuserrole.RoleId == 1 ? new UserAccessVM()
                          {
                              AccountType = aspnetrole.Name,
                              AccountPOC = aspuser.UserName,
                              phone = totaladmins.Mobile,
                              status = totaladmins.AdminId,
                              roleid = aspnetuserrole.RoleId,
                              AccountTypeid = aspnetuserrole.RoleId,
                              useraccessid = totaladmins.AdminId,
                              email = totaladmins.Email,
                          } : new UserAccessVM()
                          {
                              AccountType = aspnetrole.Name,
                              AccountPOC = aspuser.UserName,
                              phone = totalphy.Mobile,
                              status = totalphy.Status,
                              roleid = aspnetuserrole.RoleId,
                              AccountTypeid = aspnetuserrole.RoleId,
                              useraccessid = totalphy.PhysicianId,
                              email = totalphy.Email,
                          }

                          )).ToList();

            return result;
        }

        public void createAccess(int[] rolemenu, string rolename, int accounttype)
        {
            Role role = new Role();
            role.Name = rolename;
            role.AccountType = (short)accounttype;
            role.CreatedBy = "admin";
            role.CreatedDate = DateTime.Now;
            role.IsDeleted = new BitArray(new[] { false });
            _context.Roles.Add(role);
            _context.SaveChanges();

            foreach (var menu in rolemenu)
            {
                RoleMenu rolemenu1 = new RoleMenu();
                rolemenu1.MenuId = menu;
                rolemenu1.RoleId = role.RoleId;
                _context.RoleMenus.Add(rolemenu1);
                _context.SaveChanges();
            }
        }

        public void editAccess(int[] rolemenu, string rolename, int roleid)
        {
            var role = _context.Roles.Where(u => u.RoleId == roleid).First();
            role.Name = rolename;
            role.ModifiedDate = DateTime.Now;
            role.ModifiedBy = rolename;
            _context.Roles.Update(role);
            _context.SaveChanges();

            var prevRoleMenu = _context.RoleMenus.Where(u => u.RoleId == roleid).ToList();
            _context.RoleMenus.RemoveRange(prevRoleMenu);
            _context.SaveChanges();
            foreach (var menu in rolemenu)
            {
                RoleMenu rolemenu1 = new RoleMenu();
                rolemenu1.MenuId = menu;
                rolemenu1.RoleId = role.RoleId;
                _context.RoleMenus.Add(rolemenu1);
                _context.SaveChanges();
            }
        }

        public void DeleteRole(int roleid)
        {
            var role = _context.Roles.Where(u => u.RoleId == roleid).First();
            var prevRoleMenu = _context.RoleMenus.Where(u => u.RoleId == roleid).ToList();
            _context.RoleMenus.RemoveRange(prevRoleMenu);
            _context.Roles.Remove(role);
            _context.SaveChanges();
        }
    }
}
