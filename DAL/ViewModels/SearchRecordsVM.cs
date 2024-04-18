

namespace DAL.ViewModels
{
    public class SearchRecordsVM
    {
        public string? PatientName { get; set; }
        public string? Requestor { get; set; }
        public int RequestTypeId { get; set; }
        public int RequestId { get; set; }
        public string? RequestTypeName { get; set; }
        public string? DateofClose { get; set; }
        public DateTime? CloseDate { get; set; }
        public string? ServiceDate { get; set; }
        public DateTime? DateOfService { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Zip { get; set; }
        public int RequestStatus { get; set; }
        public string? PhysicianName { get; set; }
        public string? PhysicianNote { get; set; }
        public string? CancelledByProvidor { get; set; }
        public string? AdminNotes { get; set; }
        public string? PatientNote { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
