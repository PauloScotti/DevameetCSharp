using System.Security.Claims;
using DevameetCSharp.Models;
using DevameetCSharp.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevameetCSharp.Controllers
{
    [Authorize]
    public class BaseController : ControllerBase
    {
        protected readonly IUserRepository _userRepository;

        public BaseController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        protected User GetToken()
        {
            var iduser = User.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).FirstOrDefault();

            if(string.IsNullOrEmpty(iduser))
            {
                return null;
            }
            else
            {
                return _userRepository.GetUserByLogin(int.Parse(iduser));
            }
        }
    }
}
