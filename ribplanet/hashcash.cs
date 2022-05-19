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
    public struct Nonce
    {
        public byte[] nonce;
        public Nonce(byte[] nonce)
        {
            this.nonce = nonce;
        }
        public bool Equals(Nonce other) => nonce.SequenceEqual(other.nonce);
        public override bool Equals(object? obj) => obj is Nonce other && Equals(other);
        //from libplanet for testing purpose
    }
    public delegate byte[] Stamp(Nonce nonce);
    public static class HashCash
    {
        public static (Nonce Nonce, Hash digest) Answer(Stamp stamp, int difficulty)
        {
            var random = new Random();
            var nonceBytes = new byte[10];
            int counter = 1;
            while (true)
            {
                random.NextBytes(nonceBytes);
                Nonce answer = new Nonce(nonceBytes);
                Hash digest = new Hash(SHA256.HashData(stamp(answer)));
                if (digest.HasLeadingZerobits(difficulty))
                {
                    return (answer, digest);
                }
                counter += 1;
            }
        }


    }
}