using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IProviders
    {
        public List<Physician> getPhysicianList();
        public List<Physician> getfilteredPhysicians(int stateid);
    }
}
