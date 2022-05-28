using System;
using Ribplanet;
using Ribplanet.Blocks;
using Libplanet;
using System.Security.Cryptography;

namespace Ribplanet
{
    [Serializable]
    public struct Hash : IEquatable<Hash>
    {
        private byte[] _hash;
        public Hash(byte[] hash) => _hash = hash;
        public byte[] hash
        {
            get
            {
                if (this._hash.Equals(null))
                {
                    this._hash = new byte[] { 0x00 };
                }

                return this._hash;
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
                return ((mask & this.hash[leadingBytes]) == 0);
            }
            return true;

        }
        public static Hash FromString(string hex)
        {
            var bytes = new byte[hex.Length / 2];
            for (var i = 0; i < hex.Length / 2; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return new Hash(bytes);
        }
        public override string ToString()
        {
            string s = "0x" + BitConverter.ToString(this.hash);
            return s.Replace("-", string.Empty).ToLower();
        }

        public static bool operator ==(Hash left, Hash right) => left.Equals(right);
        public static bool operator !=(Hash left, Hash right) => !left.Equals(right);
        public bool Equals(Hash other)
        {
            for (int i = 0; i < other.hash.Length; i++)
            {
                if (!hash[i].Equals(other.hash[i]))
                {
                    return false;
                }
            }
            return true;
        }
        public override bool Equals(object? obj) => obj is Hash other && Equals(other);
    }
    public struct Nonce
    {
        private byte[] _nonce;
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
    public static class HashCash
    {
        public static Nonce Answer(Func<Nonce, Hash> stamp, int difficulty)
        {
            Random random = new Random();
            byte[] nonceBytes = new byte[32];
            while (true)
            {
                random.NextBytes(nonceBytes);
                Nonce nonce = new Nonce(nonceBytes);
                Hash answer = stamp(nonce);

                if (answer.HasLeadingZerobits(difficulty))
                {
                    return nonce;
                }


            }


        }
    }
}