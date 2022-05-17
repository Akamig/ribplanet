using System;
using System.Linq;
using Libplanet.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace Ribplanet
{
    public readonly struct Address
    {
        private readonly byte[] ByteArray = new byte[20];
        public Address(byte[] address)
        {
            this.ByteArray = address;
        }
        public Address(PublicKey publicKey) : this(getAddress(publicKey))
        {
        }
        public Address(PrivateKey privateKey) : this(getAddress(privateKey.PublicKey))
        {
        }

        public static byte[] getAddress(PublicKey key)
        {
            byte[] hashPayload = key.Format(false).Skip(1).ToArray();
            var output = CalculateHash(hashPayload);
            return output.Skip(output.Length - 20).ToArray();
        }

        private static byte[] CalculateHash(byte[] value)
        {
            var digest = new KeccakDigest(256);
            var output = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(value, 0, value.Length);
            digest.DoFinal(output, 0);
            return output;
        }
        /*public Address getAddressString()
        {
        }*/
    }
}
