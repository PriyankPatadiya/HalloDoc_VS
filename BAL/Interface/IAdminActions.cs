﻿using DAL.DataModels;
using DAL.ViewModels;

namespace BAL.Interface
{
    public interface IAdminActions
    {
        // Assign Case 
        public void ChangeOnAssign(int reeqid, int phyid, string notes);

        // View Case 
        public IQueryable<ViewCaseVM> getViewCaseData(int reqclientId);
        public void changeStatusOnCancleCase(int requesid, string reason, string Notes);

        public List<Physician> GetPhysicianByRegion(string RegionId);

        // View Notes
        public ViewNotesVM viewnotes(int id);

        // Block Case
        public void BlockCase(int requestId, string reason);
    }
}