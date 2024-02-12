using DAL.ViewModels;

namespace BAL.Interface
{
    public interface IRequests
    {
        
        void AddFamilyFriendForm(OthersReqVM model);
        void AddConciergeForm(OthersReqVM model);
        void AddBusinessForm(OthersReqVM model);
    }
}
