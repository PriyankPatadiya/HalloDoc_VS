using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IViewCaseAdmin
    {
        public IQueryable<ViewCaseVM> getViewCaseData(int reqclientId);
    }
}
