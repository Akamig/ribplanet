using System;
using Libplanet;
using System.Security.Cryptography;
namespace Ribplanet
{
    public struct Hash
    {
        private byte[] _hash;
        public Hash(byte[] hash) => _hash = hash;
        public byte[] hash
        {
            get
            {
                if (_hash.Equals(null))
                {
                    _hash = new byte[] { 0x00 };
                }

                return _hash;
            }
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
                if ((mask & this.hash[leadingBytes]) == 0)
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
        public byte[] _nonce;
        public Nonce(byte[] nonce) => _nonce = nonce;
        public byte[] nonce
        {
            get
            {
                if (_nonce.Equals(null))
                {
                    _nonce = new byte[] { 0x00 };
                }

                return _nonce;
            }
        }

        public static bool operator ==(Nonce left, Nonce right) => left.Equals(right);

        public static bool operator !=(Nonce left, Nonce right) => !left.Equals(right);
        public bool Equals(Nonce other) => nonce.SequenceEqual(other.nonce);
        public override bool Equals(object? obj) => obj is Nonce other && Equals(other);
        //from libplanet for testing purpose
        public override int GetHashCode() => ByteUtil.CalculateHashCode(ToByteArray());
        public byte[] ToByteArray() => _nonce.ToArray();
    }
    public delegate IEnumerable<byte[]> Stamp(Nonce nonce);
    public static class HashCash
    {
        public static (Nonce Nonce, Hash digest, int counter) Answer(Stamp stamp, int difficulty)
        {
            SHA256 algo = SHA256.Create();
            var nonceBytes = new byte[10];
            var random = new Random();
            int counter = 0;
            while (true)
            {
                random.NextBytes(nonceBytes);
                algo.Initialize();
                Nonce answer = new Nonce(nonceBytes);
                IEnumerable<byte[]> chunks = stamp(answer);

                //Digest in Hash AlgorithmType.cs
                foreach (byte[] chunk in chunks)
                {
                    algo.TransformBlock(chunk, 0, chunk.Length, null, 0);
                }
                algo.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
                //ends here
                var digest = new Hash(algo.Hash);
                if (digest.HasLeadingZerobits(difficulty))
                {
                    return (answer, digest, counter);
                }
                counter++;
            }
        }


    }
}