using System;
using Libplanet.Crypto;
using Org.BouncyCastle.Crypto.Digests;
namespace Ribplanet
{
    public readonly struct Address : IComparable<Address>
    {
        private readonly byte[] ByteArray = new byte[20];
        public Address(byte[] address)
        {
            this.ByteArray = address;
        }
        public Address(PublicKey publicKey) : this(DeriveByteArrayfromPublicKey(publicKey))
        {
        }
        public Address(PrivateKey privateKey) : this(DeriveByteArrayfromPublicKey(privateKey.PublicKey))
        {
        }

        public static Address GetAddress(PublicKey key)
        {
            byte[] bytes = DeriveByteArrayfromPublicKey(key);
            return new Address(bytes);
        }

        private static byte[] CalculateHash(byte[] value)
        {
            var digest = new KeccakDigest(256);
            var output = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(value, 0, value.Length);
            digest.DoFinal(output, 0);
            return output;
        }
        private static byte[] DeriveByteArrayfromPublicKey(PublicKey key)
        {
            byte[] hashPayload = key.Format(false).Skip(1).ToArray();
            var output = CalculateHash(hashPayload);
            return output.Skip(output.Length - 20).ToArray();
        }
        public override string ToString()
        {
            string s = "0x" + BitConverter.ToString(this.ByteArray);
            return s.Replace("-", string.Empty).ToLower();
        }

        //from libplanet, for xunit Equal implementation
        int IComparable<Address>.CompareTo(Address other)
        {
            byte[] self = ByteArray, operand = other.ByteArray;

            for (int i = 0; i < 20; i++)
            {
                int cmp = ((IComparable<byte>)self[i]).CompareTo(operand[i]);
                if (cmp != 0)
                {
                    return cmp;
                }
            }
            return 0;
        }
    }
}
