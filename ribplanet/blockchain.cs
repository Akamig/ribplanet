
using System.Text;
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
            Block<Arithmetic> genBlock = Block<Arithmetic>.Mine(0, 20, timestamp, new Address(privateKey), null, serTx);
            return genBlock;
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


        private Blockchain(
        )
        {
            Block<Arithmetic> genesisBlock = MakeGenesisBlock();
            this._blocks.Add(genesisBlock.Hash, genesisBlock);
        };

        private Block<Arithmetic> MineBlock(Address rewardBeneficiary)
        {
            var index = this._blocks.Count;
            var difficulty = this.ExpectDifficulty();
            var block = Block<Arithmetic>.Mine(index, difficulty, DateTimeOffset.UtcNow, rewardBeneficiary, this._blocks);
        }

        public static ExpectDifficulty()
        {
            _blocks.Last<>
        };
    }
}