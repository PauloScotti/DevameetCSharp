using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevameetCSharp.Models;
using Microsoft.IdentityModel.Tokens;

namespace DevameetCSharp.Service
{
    public class TokenService
    {
        public const int MinimumKeyLength = 32;
        private static readonly TimeSpan Lifetime = TimeSpan.FromDays(7);
        private readonly byte[] _key;

        public TokenService(IConfiguration configuration)
        {
            _key = ReadKey(configuration);
        }

        // A chave vem da configuração (variável JWT__SecretKey ou user-secrets), nunca do repositório.
        public static byte[] ReadKey(IConfiguration configuration)
        {
            var key = configuration["JWT:SecretKey"];
            if (string.IsNullOrWhiteSpace(key) || key.Length < MinimumKeyLength)
            {
                throw new InvalidOperationException(
                    $"Defina JWT:SecretKey com pelo menos {MinimumKeyLength} caracteres (variável de ambiente JWT__SecretKey ou dotnet user-secrets).");
            }
            return Encoding.UTF8.GetBytes(key);
        }

        public string CreateToken(User user)
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Sid, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name)
                }),
                Expires = DateTime.UtcNow.Add(Lifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenhandler.CreateToken(tokenDescriptor);
            return tokenhandler.WriteToken(token);
        }
    }
}
