

namespace DAL.ViewModels
{
    public class BlockedHistoryVM
    {
        public int BlockedRequestID { get; set; }

        public int RequestId { get; set; }

        public string? PatientName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public string? Notes { get; set; }

        public bool IsActive { get; set; }
    }
}
