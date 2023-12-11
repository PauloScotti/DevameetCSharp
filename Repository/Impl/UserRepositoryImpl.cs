using DevameetCSharp.Models;

namespace DevameetCSharp.Repository.Impl
{
    public class UserRepositoryImpl : IUserRepository
    {
        private readonly DevameetContext _context;
        public UserRepositoryImpl(DevameetContext context)
        {
            _context = context;
        }

        public User GetUserByLoginPassword(string login, string password)
        {
            return _context.Users.FirstOrDefault(u => u.Email == login && u.Password == password);
        }
        public User GetUserByLogin(int iduser)
        {
            return _context.Users.FirstOrDefault(u => u.Id == iduser);
        }

        public void Save(User user)
        {
            _context.Add(user);
            _context.SaveChanges();
        }

        public bool VerifyEmail(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
    }
}
