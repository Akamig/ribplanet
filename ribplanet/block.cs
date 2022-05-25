using Ribplanet.Tx;
using Libplanet.Action;
using System.Security.Cryptography;
namespace Ribplanet.Blocks
{
    public sealed class Block<Arithmetic> : IEquatable<Block<Arithmetic>>
        where Arithmetic : IAction, new()
    {

        public int Index { get; set; }
        public int Difficulty { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public Address RewardBeneficiary { get; set; }
        public Hash PreviousHash { get; set; }
        public Transaction<Arithmetic>[] Transactions { get; set; }
        public Nonce Nonce { get; set; }

        public Hash Hash { get; set; }

        public Block(
            int index,
            int difficulty,
            DateTimeOffset timestamp,
            Address rewardBeneficiary,
            Hash previousHash,
            Transaction<Arithmetic>[] transactions,
            Nonce nonce)
        {
            this.Index = index;
            this.Difficulty = difficulty;
            this.Timestamp = timestamp;
            this.RewardBeneficiary = rewardBeneficiary;
            this.PreviousHash = previousHash;
            this.Transactions = transactions;
            this.Nonce = nonce;

            byte[] payload = Serialize();
            using SHA256 algo = SHA256.Create();
            this.Hash = new Hash(algo.ComputeHash(payload));
        }

        private byte[] Serialize()
        {
            var bdict = Bencodex.Types.Dictionary.Empty
                .Add(nameof(Index), Index)
                .Add(nameof(Difficulty), Difficulty)
                .Add(nameof(Timestamp), Timestamp.ToString())
                .Add(nameof(RewardBeneficiary), RewardBeneficiary.ToString())
                .Add(nameof(PreviousHash), PreviousHash.hash)
               // .Add(nameof(Transactions), Transactions.Select(a=> a.))
                .Add(nameof(Nonce), Nonce.nonce);

            return new Bencodex.Codec().Encode(bdict);
        }

        public static Block<Arithmetic> Mine(int index, int difficulty, DateTimeOffset timestamp, Address rewardBeneficiary, Hash previousHash, Transaction<Arithmetic>[] transactions)
        {
            Func<Nonce, Hash> stamp = (nonce) =>
            {
                var block = new Block<Arithmetic>(index, difficulty, timestamp, rewardBeneficiary, previousHash, transactions, nonce);
                return block.Hash;
            };

            Nonce nonce = HashCash.Answer(stamp, difficulty);
            return new Block<Arithmetic>(index, difficulty, timestamp, rewardBeneficiary, previousHash, transactions, nonce);
        }

        public static bool operator ==(Block<Arithmetic>? left, Block<Arithmetic>? right) =>
        Equals(left, right);
        public static bool operator !=(Block<Arithmetic>? left, Block<Arithmetic>? right) =>
        !Equals(left, right);
        public bool Equals(Block<Arithmetic>? other)
        {
            if (other is null)
            {
                return false;
            }

            return ReferenceEquals(this, other) ||
                (Hash.Equals(other.Hash));
        }

        public override bool Equals(object? obj) =>
        obj is Block<Arithmetic> other && Equals(other);

        /// <inheritdoc cref="object.GetHashCode()"/>
        public override int GetHashCode() =>
        unchecked((17 * 31 + Hash.GetHashCode()) * 31);

        /// <inheritdoc cref="object.ToString()"/>
        public override string ToString() =>
        Hash.ToString();


    }
}