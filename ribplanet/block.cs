using Ribplanet;
namespace Ribplanet
{
    public sealed class Block
    {
        public int Index { get; }
        public int Difficulty { get; }
        public DateTimeOffset Timestamp { get; }
        public Nonce Nonce { get; }
        public Address RewardBeneficiary { get; }
        public Hash? PreviousHash { get; }

        // public Hash hash(){}
        // public Blockserialization serialize(){}
        // public bencode(){}
        // public valideate{}
    }
}