using Microsoft.AspNetCore.SignalR;

namespace HalloDoc.NewFolder1
{
    public class RegisterDTO
    {
        public String UserId { get; set; }
        public String Password { get; set; }

        internal static Task FirstOrDefaultAsync(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }
    }
}
