using DAL.DataModels;
using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    // Login Interface Contains Method LoginVarify
    public interface ILogin
    {
        public bool LoginVarify(LoginVM user);
    }
}
