using DAL.DataModels;
namespace DAL.ViewModels
{
    public class AdminDashboardTableVM
    {
        public string PatientName { get; set; }
        public string? BirthDate { get; set; }
        public string RequestorName { get; set; }
        public string RequestDate { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string YourPhone { get; set; }
        public int requestId { get; set; }
        public int? physicianId { get; set; }
        public int requestStatus { get; set; }
        public int status { get; set; }
        public string? physicianname { get; set; }
        public DateTime? DateOfService { get; set; }
        public string? Region { get; set; }
        public string? Email { get; set; }
        public int? regionId { get; set; }
        public int reqclientId { get; set; }  
        public List<Region> regionTable { get; set; }
        public int reqid { get; set; }
        public bool? IsEncounterFinalize { get; set; }
    }
}
