using BAL.Interface;
using DAL.DataContext;
using Microsoft.EntityFrameworkCore;

namespace BAL.Repository
{
    public class ProviderSite : IProviderSite
    {
        private readonly ApplicationDbContext _context;
        public ProviderSite(ApplicationDbContext context)
        {
            _context = context;
        }

        public int CountRequests(string StatusButton, int? physicianId)
        {
            List<int> myarray = new List<int>();

            if (StatusButton == "1")
            {
                myarray.Add(1);
            }
            if (StatusButton == "2")
            {
                myarray.Add(2);
            }
            if (StatusButton == "3")
            {
                myarray.Add(4);
                myarray.Add(5);
            }
            if (StatusButton == "4")
            {
                myarray.Add(6);
            }
            if (StatusButton == "5")
            {
                myarray.Add(3);
                myarray.Add(7);
                myarray.Add(8);
            }
            if (StatusButton == "6")
            {
                myarray.Add(9);
            }
            var count = (from req in _context.Requests
                         join reqclient in _context.RequestClients
                         on req.RequestId equals reqclient.RequestId
                         where req.PhysicianId == physicianId
                         select req.Status).Where(item => myarray.Any(s => item == s)).Count();
            return count;
        }
    }
}
