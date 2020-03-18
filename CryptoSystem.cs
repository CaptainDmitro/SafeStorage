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

            using (FileStream fs = new FileStream(inputFile, FileMode.Open))
            {
                using (FileStream fw = new FileStream(outputFile, FileMode.Create))
                {
                    byte[] buf = new byte[8];
                    long pos = 0;

                    while (fs.Read(buf, 0, 8) == 8)
                    {
                        byte[] encrypted = magma.Encrypt(buf, true);
                        fw.Write(encrypted, 0, encrypted.Length);
                        pos = fs.Position;
                    }

                    if (fs.Length % 8 != 0)
                    {
                        byte[] buf2 = new byte[8];
                        Array.Copy(buf, buf2, fs.Length % 8);
                        byte[] encrypted = magma.Encrypt(buf2, true);
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

            using (FileStream fs = new FileStream(inputFile, FileMode.Open))
            {
                using (FileStream fw = new FileStream(outputFile, FileMode.Create))
                {
                    byte[] buf = new byte[8];

                    while (fs.Read(buf, 0, 8) > 0)
                    {
                        byte[] encrypted = magma.Encrypt(buf, false);

                        if (fs.Position == fs.Length)
                        {
                            int zeros = encrypted.Count(e => e == 0);
                            fw.Write(encrypted, 0, 8 - zeros);
                        }
                        else
                        {
                            fw.Write(encrypted, 0, encrypted.Length);
                        }
                    }
                }
            }
        }

        public static byte[] GenerateKey(byte[] key)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                byte[] hash = mySHA256.ComputeHash(key);
                return hash;
            }
        }
    }
}
