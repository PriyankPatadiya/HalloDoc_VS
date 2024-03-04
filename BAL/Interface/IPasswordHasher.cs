
namespace BAL.Interface
{
    public interface IPasswordHasher
    {
        public string HashPassword(string password);
        public bool varifyPassword(string email , string password);
    }
}
