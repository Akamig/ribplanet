using Ribplanet;
using Bencodex;
using Bencodex.Types;
namespace Ribplanet
{
    public sealed class Block
    {

        public int Index;
        public int Difficulty;
        public DateTimeOffset Timestamp;
        public Nonce Nonce;
        public Address? RewardBeneficiary;
        public Hash? PreviousHash;
        public Transaction[] Transactions;

        public Dictionary Mine(
        int Index,
        int Difficulty,
        DateTimeOffset Timestamp,
        Nonce Nonce,
        Address? RewardBeneficiary,
        Hash? PreviousHash,
        Transaction[] Transactions
        )
        {
            new Bencodex.Types.Dictionary(new Dictionary<Bencodex.Types.IKey, Bencodex.Types.IValue>)
            {
                
            }
        }
        public Block MakeBlock(Nonce nonce)
        {

        }
        //public BlockSerialization serialize(){}
        // public void valideate{}
    }

    public static class BlockMarshaler
    {
        private const string TimestampFormat = "yyyy-MM-ddTHH:mm:ss.ffffffZ";

        // Header fields:
        private static readonly byte[] IndexKey = { 0x69 }; // 'i'
        private static readonly byte[] TimestampKey = { 0x74 }; // 't'
        private static readonly byte[] DifficultyKey = { 0x64 }; // 'd'
        private static readonly byte[] NonceKey = { 0x6e }; // 'n'
        private static readonly byte[] MinerKey = { 0x6d }; // 'm'
        private static readonly byte[] PublicKeyKey = { 0x50 }; // 'P'
        private static readonly byte[] PreviousHashKey = { 0x70 }; // 'p'
        private static readonly byte[] TxHashKey = { 0x78 }; // 'x'
        private static readonly byte[] HashKey = { 0x68 }; // 'h'
        private static readonly byte[] StateRootHashKey = { 0x73 }; // 's'
        private static readonly byte[] SignatureKey = { 0x53 }; // 'S'
        private static readonly byte[] PreEvaluationHashKey = { 0x63 }; // 'c'

    }
}