using System;
using Org.BouncyCastle.Crypto.Digests;

namespace Ribplanet
{
    public struct Hash
    {
        public byte[] hash;
        /*public static bool hasLeadingZerobits(Hash digest, int bits)
        {
        }*/
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
        public byte[] nonce;

        public Nonce answer(int difficulty)
        {
            int counter = 1;
            while (true)
            {
                double answerBytesLength = 1 + Math.Floor(Math.Log2(counter) / 8);
                counter += 1;
                Console.WriteLine(answerBytesLength);
            }

        }
    }


}