using DAL.DataModels;
using DAL.ViewModels;

namespace BAL.Interface
{
    public interface ICreateAcc
    {
        public void AddUser(CreateAccVM Obj, AspNetUser aspnetuser);
    }
}
