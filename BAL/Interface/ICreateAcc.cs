using DAL.DataModels;

namespace BAL.Interface
{
    public interface ICreateAcc
    {
        void AddUser(AspNetUser user);
    }
}
