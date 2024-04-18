

namespace DAL.ViewModels
{
    public class UserAccessVM
    {
        public string? AccountType { get; set; }
        public int? AccountTypeid { get; set; }
        public string? AccountPOC { get; set; }

        public string? phone { get; set; }
        public int? roleid { get; set; }

        public string email { get; set; }
        public int? status { get; set; }
        public int? useraccessid { get; set; }
        public int? OpenRequest { get; set; }
    }
}
