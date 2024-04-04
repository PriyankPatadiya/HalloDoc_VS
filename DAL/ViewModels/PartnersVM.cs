using DAL.DataModels;

namespace DAL.ViewModels
{
    public class PartnersVM
    {
        public string? profession { get; set; }
        public int vendorid { get; set; }
        public DateTime CreatedDate { get; set; }
        public string createdby { get; set; }
        public string BusinessName { get; set; }
        public string? BusinessContact { get; set; }
        public string? Email { get; set; }
        public string? phoneNumber { get; set; }
        public string? Fax { get; set; }
        public int? regionId { get; set; }
    }
}
