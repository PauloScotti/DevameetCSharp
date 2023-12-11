using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevameetCSharp.Models;
using Microsoft.IdentityModel.Tokens;

namespace DevameetCSharp.Service
{
    public class TokenService
    {
        public static string CreateToken(User user, string jwtkey)
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            var secretkey = Encoding.ASCII.GetBytes(jwtkey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Sid, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name)
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretkey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenhandler.CreateToken(tokenDescriptor);

            return tokenhandler.WriteToken(token);
        }
    }
}
