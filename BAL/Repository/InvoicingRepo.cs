using BAL.Interface;
using DAL.DataContext;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class InvoicingRepo : Iinvoicing
    {
        private readonly ApplicationDbContext _context;

        public InvoicingRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<TimeSheetForm> getTimesheetdetails(string physicianId, string date)
        {
            var startdate = DateTime.Parse(date);
            var enddate = startdate.Date.Day == 1 ? new DateTime(startdate.Year, startdate.Month, 15) : new DateTime(startdate.Year, startdate.Month, 1).AddMonths(1).AddDays(-1);

            var result = (from timesheet in _context.Timesheets
                          join timesheetdetail in _context.TimesheetDetails
                          on timesheet.TimesheetId equals timesheetdetail.TimesheetId
                          where timesheet.StartDate == DateOnly.FromDateTime(startdate) && timesheet.PhysicianId ==int.Parse( physicianId)
                          orderby timesheetdetail.TimesheetDate
                          select new TimeSheetForm()
                          {
                              date = timesheetdetail.TimesheetDate,
                              physicianId = timesheet.PhysicianId,
                              onCallhours = 0,
                              totalHours = timesheetdetail.TotalHours,
                              isWeekend = timesheetdetail.IsWeekend,
                              HouseCallNo = timesheetdetail.NumberOfHouseCall,
                              PhoneCallNo = timesheetdetail.NumberOfPhoneCall

                          }).ToList();

        
            return result;   
        }
    }
}
