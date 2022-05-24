using Ribplanet.Tx;
using Libplanet.Action;
using System.Security.Cryptography;
namespace Ribplanet.Blocks
{
    public sealed class Block<T> : IEquatable<Block<T>>
        where T : IAction, new()
    {

        public int Index { get; set; }
        public int Difficulty { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public Address RewardBeneficiary { get; set; }
        public Hash PreviousHash { get; set; }
        public Transaction[] Transactions { get; set; }
        public Nonce Nonce { get; set; }

        public Hash Hash { get; set; }

        public Block(
            int index,
            int difficulty,
            DateTimeOffset timestamp,
            Address rewardBeneficiary,
            Hash previousHash,
            Transaction[] transactions,
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
                .Add(nameof(Nonce), Nonce.nonce);

            return new Bencodex.Codec().Encode(bdict);
        }

        public static Block<T> Mine(int index, int difficulty, DateTimeOffset timestamp, Address rewardBeneficiary, Hash previousHash, Transaction[] transactions)
        {
            Func<Nonce, Hash> stamp = (nonce) =>
            {
                var block = new Block<T>(index, difficulty, timestamp, rewardBeneficiary, previousHash, transactions, nonce);
                return block.Hash;
            };

            Nonce nonce = HashCash.Answer(stamp, difficulty);
            return new Block<T>(index, difficulty, timestamp, rewardBeneficiary, previousHash, transactions, nonce);
        }

        public static bool operator ==(Block<T>? left, Block<T>? right) =>
        Equals(left, right);
        public static bool operator !=(Block<T>? left, Block<T>? right) =>
        !Equals(left, right);
        public bool Equals(Block<T>? other)
        {
            if (other is null)
            {
                return false;
            }

            return ReferenceEquals(this, other) ||
                (Hash.Equals(other.Hash));
        }

        public override bool Equals(object? obj) =>
        obj is Block<T> other && Equals(other);

        /// <inheritdoc cref="object.GetHashCode()"/>
        public override int GetHashCode() =>
        unchecked((17 * 31 + Hash.GetHashCode()) * 31);

        /// <inheritdoc cref="object.ToString()"/>
        public override string ToString() =>
        Hash.ToString();


    }
}