

namespace DAL.ViewModels
{
    public class ViewNotesVM
    {
        public String? AdminNotes { get; set; }

        public List<String>? TransferNotes { get; set; }

        public String? PhysicianNotes { get; set; }

        public String? AdditionalNotes { get; set; }

        public int? RequestId { get; set; }
    }
}
