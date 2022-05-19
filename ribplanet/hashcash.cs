using System;
using Org.BouncyCastle.Crypto.Digests;

namespace Ribplanet
{
    public struct Hash
    {
        public byte[] hash;
        public bool HasLeadingZerobits(int bits)
        {
            int leadingBytes = (int) Math.Round(bits / 8d, 1);
            int trailingBits = bits % 8;
            for(int i = 0; i < leadingBytes; i++){
                if(this.hash[i] != 0x00){
                    return false;
                }
            }
            if(trailingBits != 0){
                if(hash.Length <= leadingBytes){
                    return false;
                }
                Int32 mask = 0b1111_1111 << (8 - trailingBits) & 0xff;
                return (!(bool)(mask & this.hash[leadingBytes]))
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
        public byte[] nonce;

        public Nonce Answer(int difficulty)
        {
            int counter = 1;
            while (true)
            {
                double answerBytesLength = 1 + Math.Floor(Math.Log2(counter) / 8);
                counter += 1;
            }

        }
    }


}