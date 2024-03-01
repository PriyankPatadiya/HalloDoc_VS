using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IAssignCase
    {
        public void ChangeOnAssign(int reeqid,int phyid,string notes);
    }
}
