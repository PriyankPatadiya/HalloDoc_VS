using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using System.Collections;
using System.Drawing;

namespace BAL.Repository
{
    public class SchedulingRepo : IScheduling
    {
        private readonly ApplicationDbContext _context;
        public SchedulingRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool IsShiftExist(SchedulingVM model, int physicianId)
        {
            bool isShiftExist = _context.ShiftDetails.Any(sD =>
            (sD.IsDeleted == new BitArray(new bool[] { false })) && (
            sD.Shift.PhysicianId == (int)(physicianId != null ? physicianId : model.Physicianid) &&
                                    sD.ShiftDate.Date == model.Startdate.Date && // Shifts must start on the same day
                                    ((sD.StartTime < model.Endtime && sD.EndTime > model.Starttime) || // Check for overlap
                                    (sD.StartTime < model.Starttime && sD.EndTime > model.Starttime) ||
                                    (sD.StartTime < model.Endtime && sD.EndTime > model.Endtime) ||
                                    (sD.StartTime > model.Starttime && sD.EndTime < model.Endtime))));
            return isShiftExist;
        }
        public void addShift(SchedulingVM model, int physicianId, string? creater, Shift shift)
        {
            shift.PhysicianId = (int)(physicianId != null ? physicianId : model.Physicianid);
            shift.StartDate = model.Startdate;
            shift.IsRepeat = new BitArray(new[] { model.Isrepeat });
            shift.RepeatUpto = model.Repeatupto;
            shift.CreatedDate = DateTime.Now;
            shift.CreatedBy = creater.ToString();
            _context.Shifts.Add(shift);
            _context.SaveChanges();

            ShiftDetail sd = new ShiftDetail();
            sd.ShiftId = shift.ShiftId;
            sd.ShiftDate = new DateTime(model.Startdate.Year, model.Startdate.Month, model.Startdate.Day);
            sd.StartTime = model.Starttime;
            sd.EndTime = model.Endtime;
            sd.RegionId = model.Regionid;
            sd.Status = (short)(physicianId != null ? 1 : model.Status);
            sd.IsDeleted = new BitArray(new[] { false });
            _context.ShiftDetails.Add(sd);
            _context.SaveChanges();

            ShiftDetailRegion sr = new ShiftDetailRegion();
            sr.ShiftDetailId = sd.ShiftDetailId;
            sr.RegionId = (int)model.Regionid;
            sr.IsDeleted = new BitArray(new[] { false });
            _context.ShiftDetailRegions.Add(sr);
            _context.SaveChanges();
        }

        public Shift getShiftbyPhyId(int physicianId)
        {
            return _context.Shifts.FirstOrDefault();
        }
        public bool IsRepeatedShiftExist(SchedulingVM model, int physicianId, DateTime startDateForWeekday, int i)
        {
            return _context.ShiftDetails.Any(sD =>
                                (sD.IsDeleted == new BitArray(new bool[] { false })) &&
                                    (sD.Shift.PhysicianId == (int)(physicianId != null ? physicianId : model.Physicianid) &&
                                    sD.ShiftDate.Date == startDateForWeekday.AddDays(i * 7) && // Shifts must start on the same day
                                    ((sD.StartTime < model.Endtime && sD.EndTime > model.Starttime) || // Check for overlap
                                    (sD.StartTime < model.Starttime && sD.EndTime > model.Starttime) ||
                                    (sD.StartTime < model.Endtime && sD.EndTime > model.Endtime) ||
                                    (sD.StartTime > model.Starttime && sD.EndTime < model.Endtime))));
        }
        public void AddRepeatedShift(SchedulingVM model, int physicianId, DateTime startDateForWeekday, Shift shift, int i)
        {
            ShiftDetail shiftDetail = new ShiftDetail
            {
                ShiftId = shift.ShiftId,
                ShiftDate = startDateForWeekday.AddDays(i * 7),
                RegionId = (int)model.Regionid,
                StartTime = model.Starttime,
                EndTime = model.Endtime,
                Status = (short)(physicianId != null ? 1 : model.Status),
                IsDeleted = new BitArray(new[] { false })
            };

            _context.Add(shiftDetail);
            _context.SaveChanges();
        }

        public List<SchedulingVM> getEvents(int regionId)
        {
            var events = (from s in _context.Shifts
                          join pd in _context.Physicians on s.PhysicianId equals pd.PhysicianId
                          join sd in _context.ShiftDetails on s.ShiftId equals sd.ShiftId into shiftGroup
                          from sd in shiftGroup.DefaultIfEmpty()

                          select new SchedulingVM
                          {
                              title = string.Concat(sd.StartTime, " ", sd.EndTime, " ", pd.FirstName, " ", pd.LastName),
                              Shiftid = sd.ShiftDetailId,
                              Startdate = sd.ShiftDate.Date.Add(sd.StartTime.ToTimeSpan()),
                              Enddate = sd.ShiftDate.Date.Add(sd.EndTime.ToTimeSpan()),
                              Status = sd.Status,
                              Physicianid = pd.PhysicianId,
                              PhysicianName = pd.FirstName + ' ' + pd.LastName,
                              Shiftdate = sd.ShiftDate,
                              ShiftDetailId = sd.ShiftDetailId,
                              Regionid = sd.RegionId,
                              ShiftDeleted = sd.IsDeleted[0]
                          }).Where(item => regionId == 0 || item.Regionid == regionId).ToList(); ;
            return events;

        }
        public List<SchedulingVM> getProviderEvents(int physicianId)
        {
            var events = (from s in _context.Shifts
                          join pd in _context.Physicians on s.PhysicianId equals pd.PhysicianId
                          join sd in _context.ShiftDetails on s.ShiftId equals sd.ShiftId into shiftGroup
                          from sd in shiftGroup.DefaultIfEmpty()


                          select new SchedulingVM
                          {
                              title = string.Concat(sd.StartTime, " ", sd.EndTime, " ", pd.FirstName, " ", pd.LastName),
                              Shiftid = sd.ShiftDetailId,
                              Startdate = sd.ShiftDate.Date.Add(sd.StartTime.ToTimeSpan()),
                              Enddate = sd.ShiftDate.Date.Add(sd.EndTime.ToTimeSpan()),
                              Status = sd.Status,
                              Physicianid = pd.PhysicianId,
                              PhysicianName = pd.FirstName + ' ' + pd.LastName,
                              Shiftdate = sd.ShiftDate,
                              ShiftDetailId = sd.ShiftDetailId,
                              Regionid = sd.RegionId,
                              ShiftDeleted = sd.IsDeleted[0]

                          }).ToList();
            events = events.Where(item => item.Physicianid == physicianId && !item.ShiftDeleted).ToList();
            return events;
        }

        public List<Physician> getShiftPhysicians(string region)
        {
            return (from physician in _context.Physicians
                    where int.Parse(region) == 0 || physician.RegionId == int.Parse(region)
                    select physician)
                             .ToList();
        }

        public ShiftDetail sdBysDId(int shiftDetailId)
        {
            return _context.ShiftDetails.Find(shiftDetailId);
        }

        public void saveShift(ShiftDetail shiftdetail, DateTime startDate, TimeOnly startTime, TimeOnly endTime)
        {
            shiftdetail.ShiftDate = startDate;
            shiftdetail.StartTime = startTime;
            shiftdetail.EndTime = endTime;

            // Update the database
            _context.ShiftDetails.Update(shiftdetail);
            _context.SaveChanges();
        }

        public void deleteShift(ShiftDetail shiftdetail)
        {
            shiftdetail.IsDeleted = new BitArray(new[] { true });
            _context.ShiftDetails.Update(shiftdetail);
            _context.SaveChanges();
        }

        public void changeStatus(ShiftDetail shiftDetail)
        {
            if (shiftDetail.Status == 0)
            {
                shiftDetail.Status = 1;
            }
            else
            {
                shiftDetail.Status = 0;
            }
            _context.ShiftDetails.Update(shiftDetail); _context.SaveChanges();
        }

        public List<ShiftReviewVM> listBYRegion(int region)
        {
            List<ShiftReviewVM> sdList = (from sd in _context.ShiftDetails
                                          where ((sd.RegionId == region || region == 0) &&
                                                                        sd.Status != 0 && sd.IsDeleted != new BitArray(new[] { true }))
                                          select new ShiftReviewVM()
                                          {
                                              shiftDetailId = sd.ShiftDetailId,
                                              physicianName = _context.Shifts.First(u => u.ShiftId == sd.ShiftId).Physician.FirstName,
                                              Regioname = _context.Regions.First(u => u.RegionId == sd.RegionId).Name,
                                              day = sd.ShiftDate.ToString("MMMM dd, yyyy"),
                                              startTime = sd.StartTime,
                                              endTime = sd.EndTime,
                                          }).ToList();
            return sdList;
        }

        public void ApproveShifts(string[] shiftIds)
        {
            foreach (var item in shiftIds)
            {
                var shiftdetail = _context.ShiftDetails.First(u => u.ShiftDetailId == int.Parse(item));
                shiftdetail.Status = 0;
                _context.ShiftDetails.Update(shiftdetail);
            }
            _context.SaveChanges();
        }

        public void DeleteShifts(string[] shiftIds)
        {
            foreach (var item in shiftIds)
            {
                var shiftdetails = _context.ShiftDetails.First(u => u.ShiftDetailId == int.Parse(item));
                shiftdetails.IsDeleted = new BitArray(new[] { true });
                _context.ShiftDetails.Update(shiftdetails);
            }
            _context.SaveChanges();
        }

        public List<Physician> OnDuty(string regionId)
        {
            var currentTime = DateTime.Now.Hour;
            return (from shiftDetail in _context.ShiftDetails
            join physician in _context.Physicians on shiftDetail.Shift.PhysicianId equals physician.PhysicianId
            join physicianRegion in _context.PhysicianRegions on physician.PhysicianId equals physicianRegion.PhysicianId
            where (regionId == "0" || physicianRegion.RegionId == int.Parse(regionId)) &&
                  shiftDetail.ShiftDate.Date == DateTime.Now.Date &&
                  currentTime >= shiftDetail.StartTime.Hour &&
                  currentTime <= shiftDetail.EndTime.Hour &&
                  shiftDetail.IsDeleted == new BitArray(new[] { false }) && physician.IsDeleted == new BitArray(new[] { false })
            select physician).Distinct().ToList();
        }

        public List<Physician> OffDuty(string regionId)
        {
            var currentTime = DateTime.Now.Hour;

            return (from physician in _context.Physicians
                   join physicianRegion in _context.PhysicianRegions on physician.PhysicianId equals physicianRegion.PhysicianId
                   where (regionId == "0" || physicianRegion.RegionId == int.Parse(regionId)) &&
                         !_context.ShiftDetails.Any(item => item.Shift.PhysicianId == physician.PhysicianId &&
                                                            item.ShiftDate.Date == DateTime.Now.Date &&
                                                           currentTime >= item.StartTime.Hour &&
                                                           currentTime <= item.EndTime.Hour &&
                                                           item.IsDeleted == new BitArray(new[] { false }))
                   select physician).Distinct().ToList();
        }
    }
}