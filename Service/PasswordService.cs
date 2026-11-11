using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace DevameetCSharp.Service
{
    // Senhas em argon2id, no formato PHC ($argon2id$v=19$m=...,t=...,p=...$salt$hash).
    public static class PasswordService
    {
        private const int MemoriaKb = 65536;
        private const int Iteracoes = 3;
        private const int Paralelismo = 1;
        private const int TamanhoSalt = 16;
        private const int TamanhoHash = 32;
        private const string Prefixo = "$argon2id$";

        public static string Hash(string senha)
        {
            var salt = RandomNumberGenerator.GetBytes(TamanhoSalt);
            var hash = Calcular(senha, salt, MemoriaKb, Iteracoes, Paralelismo, TamanhoHash);
            return $"{Prefixo}v=19$m={MemoriaKb},t={Iteracoes},p={Paralelismo}${SemPadding(salt)}${SemPadding(hash)}";
        }

        // Devolve se a senha confere e se o valor armazenado precisa ser regravado.
        // As contas antigas guardam a senha em MD5; elas continuam entrando e quem
        // chama regrava com argon2 quando PrecisaRegravar vem verdadeiro.
        public static (bool Valida, bool PrecisaRegravar) Verify(string senha, string armazenada)
        {
            if (string.IsNullOrEmpty(senha) || string.IsNullOrEmpty(armazenada))
            {
                return (false, false);
            }
            if (!armazenada.StartsWith(Prefixo))
            {
                var valida = Iguais(Encoding.UTF8.GetBytes(LegacyMd5(senha)), Encoding.UTF8.GetBytes(armazenada));
                return (valida, valida);
            }

            try
            {
                var partes = armazenada.Split('$');
                var parametros = partes[3].Split(',').ToDictionary(p => p.Split('=')[0], p => int.Parse(p.Split('=')[1]));
                var salt = ComPadding(partes[4]);
                var esperado = ComPadding(partes[5]);
                var calculado = Calcular(senha, salt, parametros["m"], parametros["t"], parametros["p"], esperado.Length);
                var confere = Iguais(calculado, esperado);
                var desatualizado = parametros["m"] != MemoriaKb || parametros["t"] != Iteracoes;
                return (confere, confere && desatualizado);
            }
            catch (Exception e) when (e is FormatException or IndexOutOfRangeException or KeyNotFoundException or ArgumentException)
            {
                return (false, false);
            }
        }

        // Reproduz o formato antigo: cada byte do MD5 escrito em decimal, sem separador.
        public static string LegacyMd5(string texto)
        {
            var bytes = MD5.HashData(Encoding.UTF8.GetBytes(texto));
            return string.Concat(bytes.Select(b => b.ToString()));
        }

        private static byte[] Calcular(string senha, byte[] salt, int memoriaKb, int iteracoes, int paralelismo, int tamanho)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(senha))
            {
                Salt = salt,
                MemorySize = memoriaKb,
                Iterations = iteracoes,
                DegreeOfParallelism = paralelismo,
            };
            return argon2.GetBytes(tamanho);
        }

        private static bool Iguais(byte[] a, byte[] b) => CryptographicOperations.FixedTimeEquals(a, b);

        private static string SemPadding(byte[] bytes) => Convert.ToBase64String(bytes).TrimEnd('=');

        private static byte[] ComPadding(string texto) =>
            Convert.FromBase64String(texto.PadRight(texto.Length + (4 - texto.Length % 4) % 4, '='));
    }
}
