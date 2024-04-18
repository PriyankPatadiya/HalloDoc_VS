
using DAL.DataModels;
using DAL.ViewModels;

namespace BAL.Interface
{
    public interface IAccessMenu
    {
        public List<UserAccessVM> getUserAccessData(int accId);
        public List<Role> getRole();
        public List<Menu> getMenubyRole(int role);
        public Role getRoleByRoleId(int roleId);    
        public void createAccess(int[] rolemenu, string rolename, int accounttype);
        public void editAccess(int[] rolemenu, string rolename, int roleid);
        public void DeleteRole(int roleid);
        public EditRoleAccessVM editRoleAccess(int roleid, Role role);
    }
}
