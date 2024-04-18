
using DAL.DataModels;
using DAL.ViewModels;


namespace BAL.Interface
{
    // Login Interface Contains Method LoginVarify
    public interface ILogin
    {
        public bool LoginVarify(LoginVM user);
        public AspNetUser AspuserbyEmail(string email); 
        public AspNetUserRole getUserRoleById(string id);

        public AspNetRole roleByRoleId(int roleId);
        public bool isAdmin(string email);
        public bool isPatient (string email);
        public bool isProvider (string email);
        public Physician physicianByAspUserId(string userId);
        public User userByEmail(string email);
        public bool isAspNetUser(string email);
        public void updateAspNetUser(AspNetUser user);
    }
}
