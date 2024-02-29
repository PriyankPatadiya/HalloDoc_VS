using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
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
        public void changeStatusOnCancleCase(int requesid, string reason, string Notes);
    }
}
