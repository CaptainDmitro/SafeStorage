using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SafeStorage
{
    class CryptoSystem
    {
        public void Encrypt(string inputFile, byte[] password)
        {
            string outputFile = inputFile + Settings.ENCRYPTION_EXTENSION;
            byte[] key = password;

            Magma magma = new Magma();
            magma.SetKey(key);

            using (FileStream fs = File.OpenRead(inputFile))
            {
                using (FileStream fw = File.OpenWrite(outputFile))
                {
                    byte[] buf = new byte[8];
                    while (fs.Read(buf, 0, buf.Length) > 0)
                    {
                        byte[] encrypted = magma.Encrypt(buf, true);
                        fw.Write(encrypted, 0, encrypted.Length);
                    }
                }

            }
        }

        public void Decrypt(string inputFile, byte[] password)
        {
            byte[] key = password;
            string outputFile = Path.ChangeExtension(inputFile, null);

            Magma magma = new Magma();
            magma.SetKey(key);

            using (FileStream fs = File.OpenRead(inputFile))
            {
                using (FileStream fw = File.OpenWrite(outputFile))
                {
                    byte[] buf = new byte[8];
                    while (fs.Read(buf, 0, buf.Length) > 0)
                    {
                        buf = magma.Encrypt(buf, false);
                        fw.Write(buf, 0, buf.Length);
                    }
                }
            }
        }

        public static byte[] GenerateFirstKey(byte[] key)
        {
            byte[] hash = GetSHA256(key);

            return hash;
        }

        private static byte[] GetSHA256(byte[] str)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                byte[] hash = mySHA256.ComputeHash(Encoding.UTF8.GetBytes("qweasd"));
                return hash;
            }
        }
    }
}
