using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class ProviderService : IProviders
    {
        private readonly ApplicationDbContext _context;
        public ProviderService(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Physician> getPhysicianList()
        {
            var result = (from phy in _context.Physicians
                         join role in _context.Roles
                         on phy.RoleId equals role.RoleId
                         select phy).ToList();
            return result;            
        }

        public List<Physician> getfilteredPhysicians(int stateid)
        {
            List<Physician> res = new List<Physician>();
            if (stateid > 0)
            {
                res = getPhysicianList().Where(u => u.RegionId == stateid).ToList();
            }
            else
            {
                res = getPhysicianList().ToList();
            }
            return res;
        }
    }
}
