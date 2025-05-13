using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Util
{
    public class Cryptographer
    {
   
        static readonly char[] padding = { '=' };

        private static void getKey(out byte[] key, out byte[] iv)
        {
            //string saltedKey = ConfigurationSettings.AppSettings["Kanoonir"];

            string saltedKey = "45BD6660D7F11CD4CBCE000FDDCC974E500D3CA75D3235CCEC7322BDED29667275553106957E5EA11131697074DB0758DA902953A0B31EA0880AD9F26A436C17C906BDBDC0F5DABD72CEEC8CA886EF1892EFB068DA7FD177DC4FDA926D6B77ED793E98EBCC6F2B075BCA8814B732313C";

            //32 byte Key
            //64 byte salt
            //16 byte IV

            key = new byte[32];
            iv = new byte[16];

            int counter = 0;

            for (int i = 0; i < 64; i = i + 2)
            {
                key[counter++] = byte.Parse(saltedKey.Substring(i, 2), NumberStyles.HexNumber);
            }

            counter = 0;

            for (int i = 192; i < 224; i = i + 2)
            {
                iv[counter++] = byte.Parse(saltedKey.Substring(i, 2), NumberStyles.HexNumber);
            }
        }
        public static string EncryptEasy(string text)
        {
            byte[] key, iv;

            getKey(out key, out iv);

            using (var enc = new RijndaelManaged { Key = key, IV = iv })
            {
                //adding 16 byte salt
                // 8 byte in first, 8 byte in end
                var saltBegin = new byte[1];
                var saltEnd = new byte[1];

                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetNonZeroBytes(saltBegin);
                    rng.GetNonZeroBytes(saltEnd);
                }

                byte[] buf = Encoding.UTF8.GetBytes($"{ToHex(saltBegin)}{text}{ToHex(saltEnd)}");

                return Convert.ToBase64String(enc.CreateEncryptor().TransformFinalBlock(buf, 0, buf.Length)).TrimEnd(padding).Replace('+', '-').Replace('/', '_');
            }
        }

        public static string DecryptEasy(string cipherText)
        {
            try
            {

                cipherText = cipherText.Replace('_', '/').Replace('-', '+');

                switch (cipherText.Length % 4)
                {
                    case 2: cipherText += "=="; break;
                    case 3: cipherText += "="; break;
                }

                getKey(out var key, out var iv);

                using (var dec = new RijndaelManaged { Key = key, IV = iv })
                {
                    byte[] buf = Convert.FromBase64String(cipherText);

                    var saltedString = Encoding.UTF8.GetString(dec.CreateDecryptor().TransformFinalBlock(buf, 0, buf.Length));

                    saltedString = saltedString.Remove(0, 2);
                    saltedString = saltedString.Remove(saltedString.Length - 2);
                    return saltedString;
                }
            }

            catch
            {
                return string.Empty;
            }
        }

        private static string ToHex(byte[] data)
        {
            var sb = new StringBuilder();

            foreach (var t in data)
            {
                string ch = $"{t:X}";
                if (ch.Length == 1)
                    ch = "0" + ch;
                sb.Append(ch);
            }

            return sb.ToString();
        }


    }
}
