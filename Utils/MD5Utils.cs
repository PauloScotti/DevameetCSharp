using System.Security.Cryptography;
using System.Text;

namespace DevameetCSharp.Utils
{
    public class MD5Utils
    {
        public static string GenerateMD5 (string text)
        {
            MD5 md5hash = MD5.Create ();
            var bytes = md5hash.ComputeHash(Encoding.UTF8.GetBytes(text));

            StringBuilder sb = new StringBuilder ();

            foreach (var b in bytes)
            {
                sb = sb.Append (b);
            }
            return sb.ToString ();
        }
    }
}
