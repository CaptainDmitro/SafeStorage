using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeStorage
{
    interface ICIpher
    {
        int BlockSize { get; }
        int KeyLength { get; }
        void SetKey(byte[] key);
        byte[] Encrypt(byte[] data);
        byte[] Decrypt(byte[] data);
    }
}
