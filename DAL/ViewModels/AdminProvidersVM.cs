using DAL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class AdminProvidersVM
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public BitArray OnCallStatus { get; set; }
        public short? Status { get; set; }
        public List<Region> resions { get; set; }
        public int physicianid { get; set; }
    }
}