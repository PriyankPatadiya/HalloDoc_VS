using DAL.DataModels;

namespace Data_Layer.ViewModels.Admin
{
    public class EmailLogsVM
    {
        public IEnumerable<Role> roles { get; set; }
        public IEnumerable<EmailLog> emailLogs { get; set; }
    }
}