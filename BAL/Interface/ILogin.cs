
using DAL.ViewModels;


namespace BAL.Interface
{
    // Login Interface Contains Method LoginVarify
    public interface ILogin
    {
        public bool LoginVarify(LoginVM user);
    }
}
