using System;
using System.Security.Cryptography;
namespace Ribplanet
{
    public struct Hash
    {
        private byte[] hash;
        public Hash(byte[] hash)
        {
            this.hash = hash;
        }
        public bool HasLeadingZerobits(int bits)
        {
            int leadingBytes = (int)Math.Round(bits / 8d, 1);
            int trailingBits = bits % 8;
            for (int i = 0; i < leadingBytes; i++)
            {
                if (this.hash[i] != 0x00)
                {
                    return false;
                }
            }
            if (trailingBits != 0)
            {
                if (hash.Length <= leadingBytes)
                {
                    return false;
                }
                int mask = 0b_1111_1111 << (8 - trailingBits) & 0xff;
                if ((mask & this.hash[leadingBytes]) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;

        }
    }
    public struct Stamp
    {
        public byte[] stamp { get; set; }
        public Stamp(byte[] stamp)
        {
            this.stamp = stamp;
        }
    }
    public struct Nonce
    {
        private byte[] nonce;
        public Nonce(byte[] nonce)
        {
            this.nonce = nonce;
        }

        public Nonce Answer(Stamp stamp, int difficulty)
        {
            SHA256 hashAlgo = SHA256.Create();
            int counter = 1;
            while (true)
            {
                int answerBytesLength = 1 + (int)Math.Floor(Math.Log2(counter) / 8);
                Nonce answer = new Nonce(BitConverter.GetBytes(counter));
                Hash digest = new Hash(hashAlgo.ComputeHash(stamp(answer.nonce)));
                if (digest.HasLeadingZerobits(difficulty))
                {
                    return answer;
                }
                counter += 1;
            }


        }
    }


}