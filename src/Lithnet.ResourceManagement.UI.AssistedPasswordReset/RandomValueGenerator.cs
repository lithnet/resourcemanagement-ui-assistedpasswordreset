using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Lithnet.ResourceManagement.UI.AssistedPasswordReset
{
    public static class RandomValueGenerator
    {
        private static char[] availableCharacters = AppConfigurationSection.CurrentConfig.AllowedPasswordCharacterArray;

        public static string GenerateRandomString(int length)
        {
            char[] identifier = new char[length];
            byte[] randomData = new byte[length];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomData);
            }

            for (int idx = 0; idx < identifier.Length; idx++)
            {
                int pos = randomData[idx] % availableCharacters.Length;
                identifier[idx] = availableCharacters[pos];
            }

            return new string(identifier);
        }

        public static long GenerateRandomNumber(int length)
        {
            if (!(length == 2 || length == 4 || length == 8))
            {
                throw new ArgumentException(@"Number length can only be 2, 4 or 8", nameof(length));
            }

            byte[] randomData = new byte[length];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomData);
            }

            long returnValue = 0;

            if (length == 2)
            {
                returnValue = Math.Abs(BitConverter.ToInt64(randomData, 0));
            }
            else if (length == 4)
            {
                returnValue = Math.Abs(BitConverter.ToInt32(randomData, 0));
            }
            else if (length == 8)
            {
                returnValue = Math.Abs(BitConverter.ToInt16(randomData, 0));
            }

            return returnValue;
        }

        public static long GenerateRandomNumber()
        {
            int length = 8;

            byte[] randomData = new byte[length];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomData);
            }

            return Math.Abs(BitConverter.ToInt64(randomData, 0));
        }
    }
}
