
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using System.Globalization;
using Ribplanet;
using Ribplanet.Tx;
using Ribplanet.Blocks;
using Libplanet.Tests.Fixtures;
using Libplanet.Crypto;

namespace Ribplanet.Blockchain
{
    public class Blockchain
    {
        internal readonly ReaderWriterLockSlim _rwlock;
        private IDictionary<Hash, Block<Arithmetic>> _blocks;
        private SerializedTx _tx;
        private int difficulty = 20;
        private Block<Arithmetic> _genesis;

        public Block<Arithmetic> this[in Hash blockHash]
        {
            get
            {
                if (!ContainsBlock(blockHash))
                {
                    throw new KeyNotFoundException(
                        $"The given hash[{blockHash}] was not found in this chain."
                    );
                }

                _rwlock.EnterReadLock();
                try
                {
                    return _blocks[blockHash];
                }
                finally
                {
                    _rwlock.ExitReadLock();
                }
            }
        }
        public static Block<Arithmetic> MakeGenesisBlock(
            PrivateKey privateKey = null,
            Arithmetic blockAction = null
        )
        {
            privateKey ??= new PrivateKey();
            blockAction ??= new Arithmetic(OperatorType.Add, 0);
            var timestamp = DateTimeOffset.UtcNow;
            var transaction = new Transaction<Arithmetic>(privateKey, new Address(privateKey), blockAction, 0);
            SerializedTx serTx = new SerializedTx(transaction);
            Block<Arithmetic> genBlock = Block<Arithmetic>.Mine(0, 20, timestamp, new Address(privateKey), new Hash(new byte[] { 0x00 }), serTx);
            return genBlock;
        }

        public string listBlocks()
        {
            StringBuilder sb = new StringBuilder();
            var values = this._blocks.Values;
            foreach(var value in values){
                sb.AppendLine($"{values.Count}");
                sb.AppendLine(JsonSerializer.Serialize(value));
            }    
            return sb.ToString();
        }

        public bool ContainsBlock(Hash blockHash)
        {
            _rwlock.EnterReadLock();
            try
            {
                return _blocks.ContainsKey(blockHash);
            }
            finally
            {
                _rwlock.ExitReadLock();
            }
        }

        public void updateTx(SerializedTx tx)
        {
            this._tx = tx;
        }


        public Blockchain()
        {
            this._rwlock = new ReaderWriterLockSlim();
            Block<Arithmetic> genesisBlock = MakeGenesisBlock();
            this._tx = genesisBlock.Transaction;
            this._blocks = new Dictionary<Hash, Block<Arithmetic>>();
            _blocks.Add(genesisBlock.Hash, genesisBlock);
            this._genesis = genesisBlock;
        }

        public Block<Arithmetic> MineBlock(Address rewardBeneficiary)
        {
            var index = this._blocks.Count;
            var difficulty = this.ExpectDifficulty();

            if (_tx == this._blocks.OrderBy(a => a.Value.Index).Last().Value.Transaction)
            {
                _tx = null;
                // duplicated transaction prevention
            };

            var nextBlock = Block<Arithmetic>.Mine(index, difficulty, DateTimeOffset.UtcNow, rewardBeneficiary, this._blocks.OrderBy(a => a.Value.Index).Last().Value.Hash, _tx);
            this._blocks.Add(new KeyValuePair<Hash, Block<Arithmetic>>(nextBlock.Hash, nextBlock));
            return nextBlock;
        }

        private int ExpectDifficulty()
        {
            if (_blocks.Count() < 2)
            {
                return this.difficulty;
            }
            else
            {
                var blocks = this._blocks.OrderBy(a => a.Value.Index).TakeLast(2).ToArray();
                var timeSpan = blocks[1].Value.Timestamp - blocks[0].Value.Timestamp;
                if (timeSpan > TimeSpan.FromSeconds(10))
                {
                    this.difficulty = difficulty - 1;
                }
                else
                {
                    this.difficulty = difficulty + 1;
                }
                return this.difficulty;
            }

        }
    }
}