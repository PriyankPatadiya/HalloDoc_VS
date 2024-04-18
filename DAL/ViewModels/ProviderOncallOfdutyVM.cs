using DAL.DataModels;

namespace DAL.ViewModels
{
    public class ProviderOncallOfdutyVM
    {
        public List<Physician>? OffDuty { get; set; }
        public List<Physician>? OnDuty { get; set; }
        public List<Region>? Regions { get; set; }
    }
}
