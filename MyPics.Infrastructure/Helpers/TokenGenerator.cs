using System.Linq;
using System.Security.Cryptography;

namespace MyPics.Infrastructure.Helpers
{
    public class TokenGenerator
    {
        public static string GenerateToken(int size = 32)
        {
            var crypto = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[size / 2];
            crypto.GetNonZeroBytes(bytes);

            return ToHexString(bytes, true);
        }

        private static string ToHexString(byte[] bytes, bool useLowerCase = false)
        {
            var hex = string.Concat(bytes.Select(b => b.ToString(useLowerCase ? "x2" : "X2")));

            return hex;
        }
    }
}