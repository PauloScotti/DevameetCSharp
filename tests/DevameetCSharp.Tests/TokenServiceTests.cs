using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DevameetCSharp.Models;
using DevameetCSharp.Service;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DevameetCSharp.Tests
{
    public class TokenServiceTests
    {
        private static IConfiguration Configuracao(string? chave) =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> { ["JWT:SecretKey"] = chave })
                .Build();

        [Fact]
        public void TokenLevaOIdDoUsuarioETemValidade()
        {
            var service = new TokenService(Configuracao(new string('k', 32)));

            var token = new JwtSecurityTokenHandler().ReadJwtToken(
                service.CreateToken(new User { Id = 7, Name = "Ana" }));

            Assert.Equal("7", token.Claims.First(c => c.Type == ClaimTypes.Sid).Value);
            Assert.True(token.ValidTo > DateTime.UtcNow.AddDays(6));
            Assert.True(token.ValidTo < DateTime.UtcNow.AddDays(8));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("chave-curta")]
        public void RecusaSubirSemChaveOuComChaveCurta(string? chave)
        {
            Assert.Throws<InvalidOperationException>(() => new TokenService(Configuracao(chave)));
        }
    }
}
