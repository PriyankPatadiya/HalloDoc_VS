

using DAL.DataModels;

namespace BAL.Interface
{
    public interface IProviderDashboard
    {
        public Physician getPhysicianbyEmail(int physicianId);
        public void AcceptRequest(int reqid);
        public void transferRequest(int reeqid, string Notes);
        public void concludeRequest(int reqId, int physicianId, string Notes);
        public List<PhysicianLocation> locationList();
    }
}
