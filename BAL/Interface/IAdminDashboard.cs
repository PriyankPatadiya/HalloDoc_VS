using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IAdminDashboard
    {
        public IQueryable<AdminDashboardTableVM> GetRequestsQuery(string status);
        public AdminDashboardTableVM GetRequestModel();
        public int CountRequests(string StatusButton);
    }
}
