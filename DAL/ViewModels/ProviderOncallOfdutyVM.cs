using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class ProviderOncallOfdutyVM
    {
        public List<Physician>? OffDuty { get; set; }
        public List<Physician>? OnDuty { get; set; }
        public List<Region>? Regions { get; set; }
    }
}
