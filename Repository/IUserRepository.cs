using DevameetCSharp.Models;

namespace DevameetCSharp.Repository
{
    public interface IUserRepository
    {
        User GetUserByLoginPassword(string login, string password);
        User GetUserByLogin(int iduser);
        void Save(User user);
        bool VerifyEmail (string email);
        void UpdateUser(User user);
    }
}
