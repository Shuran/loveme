using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace meloveService.Security
{
    public static class PasswordUtility
    {
        public static byte[] generateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[256];
            rng.GetBytes(salt);
            return salt;
        }

        public static byte[] hash(string pPlainText, byte[] pSalt)
        {
            SHA512Cng hashFunc = new SHA512Cng();
            byte[] plainBytes = Encoding.ASCII.GetBytes(pPlainText);
            byte[] toBeHashed = new byte[plainBytes.Length + pSalt.Length];
            plainBytes.CopyTo(toBeHashed, 0);
            pSalt.CopyTo(toBeHashed, plainBytes.Length);

            return hashFunc.ComputeHash(toBeHashed);
        }

        public static bool slowEquals(byte[] pa, byte[] pb)
        {
            int diff = pa.Length ^ pb.Length;

            for (int i = 0; i < pa.Length && i < pb.Length; i++)
            {
                diff |= pa[i] ^ pb[i];
            }

            return diff == 0;
        }
    }
}
