using Ribplanet;
using Ribplanet.HashCash;
using Org.BouncyCastle.Crypto;

namespace Ribplanet
{
    public class BlockSerialization
    {
        public Hash Hash;
    }
    public sealed class Block
    {
        public int Index { get; }
        public int Difficulty { get; }
        public DateTimeOffset Timestamp { get; }
        public Nonce Nonce { get; }
        public Address RewardBeneficiary { get; }
        public Hash? PreviousHash { get; }

        //public Hash hash(Block block)
        //{
        //    var serialized = this.bencode();
        //    }
        //public BlockSerialization serialize(){}
        //public byte[] bencode(){}
        // public void valideate{}
    }
}