using Ribplanet.Tx;
using Libplanet.Action;
using Libplanet.Tests.Fixtures;
using System.Text;
using System.Collections;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using Bencodex;
using Bencodex.Types;
using System.Runtime.Serialization;
namespace Ribplanet.Blocks
{
    public sealed class SerializedBlock
    {
        public byte[] Block;
        public Hash hash;
        public SerializedBlock(Block<Arithmetic> block)
        {
            this.Block = block.Serialize();
            this.hash = block.Hash;
        }

        public static Block<Arithmetic> Deserialize(SerializedBlock sBlock)
        {
            using SHA256 algo = SHA256.Create();
            Codec codec = new Codec();
            BinaryFormatter fmt = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            if (sBlock.hash == new Hash(algo.ComputeHash(sBlock.Block)))
            {
                var dict = (Bencodex.Types.Dictionary)codec.Decode(sBlock.Block);
                var timestamp = dict.GetValue<Text>("Timestamp");
                var rewardBeneficiary = dict.GetValue<Text>("RewardBeneficiary");
                
                var memoryStream = new MemoryStream();
                var binaryTxs = dict.GetValue<Binary>("Transactions");
                memoryStream.Write(binaryTxs, 0, (int)binaryTxs.EncodingLength);
                Hashtable txs = (Hashtable)fmt.Deserialize(memoryStream);
                var structedTxs = new SerializedTx[txs.Count];

                foreach (DictionaryEntry entry in txs)
                {
                    structedTxs.Append(new SerializedTx((byte[])entry.Key, new Hash((byte[])entry.Value)));
                }


                Block<Arithmetic> block = new Block<Arithmetic>(
                    dict.GetValue<Integer>("Index"),
                    dict.GetValue<Integer>("Difficulty"),
                    DateTimeOffset.Parse(timestamp),
                    Address.FromString(rewardBeneficiary),
                    new Hash(dict.GetValue<Binary>("PreviousHash")),
                    structedTxs,
                    new Nonce(dict.GetValue<Binary>("Nonce"))
                );
                return block;
            }
            else{
                throw new InvalidDataException("Block Hash Mismatch.");
            }
        }
    }
    public sealed class Block<Arithmetic> : IEquatable<Block<Arithmetic>>
        where Arithmetic : IAction, new()
    {

        public int Index { get; set; }
        public int Difficulty { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public Address RewardBeneficiary { get; set; }
        public Hash PreviousHash { get; set; }
        public Hashtable Transactions { get; set; }
        public Nonce Nonce { get; set; }

        public Hash Hash { get; set; }

        public Block(
            int index,
            int difficulty,
            DateTimeOffset timestamp,
            Address rewardBeneficiary,
            Hash previousHash,
            SerializedTx[] transactions,
            Nonce nonce)
        {
            Hashtable Txs = new Hashtable();
            foreach (SerializedTx tx in transactions)
            {
                Txs.Add(tx.TxId, tx.SerializedTransaction);
            }

            this.Index = index;
            this.Difficulty = difficulty;
            this.Timestamp = timestamp;
            this.RewardBeneficiary = rewardBeneficiary;
            this.PreviousHash = previousHash;
            this.Transactions = Txs;
            this.Nonce = nonce;

            byte[] payload = Serialize();
            using SHA256 algo = SHA256.Create();
            this.Hash = new Hash(algo.ComputeHash(payload));
        }

        public byte[] Serialize()
        {
            BinaryFormatter fmt = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            fmt.Serialize(ms, this.Transactions);
            ms.Seek(0, SeekOrigin.Begin);
            byte[] TxByteArray = new byte[ms.Length];
            ms.Read(TxByteArray, 0, (int)ms.Length);

            var bdict = Bencodex.Types.Dictionary.Empty
                .Add(nameof(Index), Index)
                .Add(nameof(Difficulty), Difficulty)
                .Add(nameof(Timestamp), Timestamp.ToString())
                .Add(nameof(RewardBeneficiary), RewardBeneficiary.ToString())
                .Add(nameof(PreviousHash), PreviousHash.hash)
                .Add(nameof(Transactions), TxByteArray)
                .Add(nameof(Nonce), Nonce.nonce);

            return new Bencodex.Codec().Encode(bdict);
        }

        public static Block<Arithmetic> Mine(int index, int difficulty, DateTimeOffset timestamp, Address rewardBeneficiary, Hash previousHash, SerializedTx[] transactions)
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