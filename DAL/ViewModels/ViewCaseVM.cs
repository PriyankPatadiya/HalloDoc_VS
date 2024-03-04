using DAL.DataModels;

namespace DAL.ViewModels
{
    public class ViewCaseVM
    {
        public int? RequestClientId { get; set; }

        public int? Status { get; set; }
        public string? ConfirmationNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }
        public string? Notes { get; set; }
        public string? Email { get; set; } 
        public string? Phone { get; set; }

        public string address { get; set; } = null!;

        public string? region { get; set; }

        public int? requestid { get; set; }

        public List<Region>? regiontable { get; set; }
    }
}
