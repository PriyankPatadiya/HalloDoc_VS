using DAL.DataModels;

namespace DAL.ViewModels
{
    public class EditRoleAccessVM
    {
        public int roleid { get; set; }
        public string Name { get; set; }
        public int accountType { get; set; }
        public List<Menu> Menus { get; set; }
        public List<RoleMenu> selectedmenus { get; set; }
    }
}
