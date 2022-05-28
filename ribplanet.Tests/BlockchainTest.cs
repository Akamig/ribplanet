using Xunit;
using Ribplanet;
using Ribplanet.Blockchain;
using Ribplanet.Tx;
using Libplanet.Crypto;
using Libplanet.Tests.Fixtures;
namespace Ribplanet.Tests
{
    public class AddressTests
    {
        [Fact]
        public void BlockchainTest()
        {
            var blockchain = new Blockchain.Blockchain();
            PrivateKey key1 = PrivateKey.FromString(
                    "ea0493b0ed67fc97b2e5e85a1d145adea294112f09df15398cb10f2ed5ad1a83"
                );
            Arithmetic action = Arithmetic.Add(1);
            var Tx = new SerializedTx(new Transaction<Arithmetic>(key1, new Address(key1), action, 1));
            blockchain.updateTx(Tx);
            blockchain.MineBlock(new Address(key1));
            Tx = new SerializedTx(new Transaction<Arithmetic>(key1, new Address(key1), action, 2));
            blockchain.updateTx(Tx);
            blockchain.MineBlock(new Address(key1));
            Tx = new SerializedTx(new Transaction<Arithmetic>(key1, new Address(key1), action,3));
            blockchain.updateTx(Tx);
            blockchain.MineBlock(new Address(key1));
            Tx = new SerializedTx(new Transaction<Arithmetic>(key1, new Address(key1), action, 4));
            blockchain.updateTx(Tx);
            blockchain.MineBlock(new Address(key1));

            blockchain.listBlocks();

        }
    }

}