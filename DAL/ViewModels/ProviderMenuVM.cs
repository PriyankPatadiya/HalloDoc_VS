using DAL.DataModels;

namespace DAL.ViewModels
{
    public class ProviderMenuVM
    {
        public List<Physician> physicians { get; set; }
        public List<Region> regions { get; set; } 
    }
}
