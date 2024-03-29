
using DAL.ViewModels;

namespace BAL.Interface
{
    public interface IAccessMenu
    {
        public List<UserAccessVM> getUserAccessData(int accId);
    }
}
