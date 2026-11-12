using DevameetCSharp.Models;

namespace DevameetCSharp.Repository
{
    public interface IUserRepository
    {
        User? GetUserByEmail(string email);
        User GetUserByLogin(int iduser);
        void Save(User user);
        bool VerifyEmail (string email);
        void UpdateUser(User user);
    }
}
