using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
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
                              isWeekend = (bool)timesheetdetail.IsWeekend,
                              HouseCallNo = timesheetdetail.NumberOfHouseCall,
                              PhoneCallNo = timesheetdetail.NumberOfPhoneCall

                          }).ToList();

        
            return result;   
        }

        public bool isTimeSheetExist(DateOnly startdate)
        {
            bool result = _context.Timesheets.Any(u => u.StartDate == startdate);
            return result;
        }

        public void AddNewSheet(DateOnly date, string physicianId)
        {
            DateOnly enddate = date.Day == 1 ? new DateOnly(date.Year, date.Month, 15) : new DateOnly(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
            var sheet = new Timesheet();
            sheet.PhysicianId = int.Parse(physicianId);
            sheet.StartDate = date;
            sheet.EndDate = enddate;
            sheet.CreatedDate = DateTime.Now;
            sheet.IsFinalize = false;
            sheet.IsApproved = false;
            sheet.CreatedBy = _context.Physicians.First(u => u.PhysicianId == int.Parse(physicianId)).AspNetUserId;
            _context.Timesheets.Add(sheet);
            _context.SaveChanges();

            for(int i=date.Day; i<=enddate.Day; i++)
            {
                var TimesheetDetailDate = new DateOnly(enddate.Year, enddate.Month, i);
                var timesheetdetail = new TimesheetDetail();
                timesheetdetail.TimesheetId = sheet.TimesheetId;
                timesheetdetail.TimesheetDate = TimesheetDetailDate;
                timesheetdetail.TotalHours = 0;
                timesheetdetail.IsWeekend = false;
                timesheetdetail.NumberOfHouseCall = 0;
                timesheetdetail.NumberOfPhoneCall = 0;
                _context.TimesheetDetails.Add(timesheetdetail);
            }
            _context.SaveChanges();
        }
        public TimeSheetTableMainVM getTimesheetTableData(string date, int physicianId)
        {
            var table = new TimeSheetTableMainVM();

            table.isFinalize = _context.Timesheets.Any(i => i.PhysicianId != physicianId && i.StartDate == DateOnly.Parse(date));

            table.firstTable = (from timesheetdetails in _context.TimesheetDetails
                                join timesheet in _context.Timesheets
                                on timesheetdetails.TimesheetId equals timesheet.TimesheetId into timesheetjoin
                                from timesheetTotal in timesheetjoin.DefaultIfEmpty()
                                where timesheetTotal.PhysicianId == physicianId && timesheetTotal.StartDate == DateOnly.Parse(date)
                                select new TimeSheetTableVM()
                                {
                                    shiftdate = timesheetdetails.TimesheetDate,
                                    shiftCount = _context.ShiftDetails.Where(u => DateOnly.FromDateTime(u.ShiftDate) == timesheetdetails.TimesheetDate && u.Shift.PhysicianId == physicianId && u.IsDeleted != new System.Collections.BitArray( new[]{ true })).Count(),
                                    NightShiftWeekend = 0,
                                    HouseCallNightSWeekend = 0,
                                    PhoneConsults = 0,
                                    housecall = 0,
                                    PhoneConsultsNightWeekend = 0,
                                    BatchTesting = 0,

                                }).Distinct().ToList();

            table.secondTable = null;

            return table;

        }

        public TimesheetDetail getTimesheetdetailInstanse(int? physicianId, DateOnly date)
        {
            return _context.TimesheetDetails.FirstOrDefault(u => u.Timesheet.PhysicianId == physicianId && u.TimesheetDate == date);
        }
    }
}
