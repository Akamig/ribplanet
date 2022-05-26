using Xunit;
using Xunit.Abstractions;
using Ribplanet;
using Ribplanet.Blocks;
using Ribplanet.Tx;
using System;
using System.Text;
using Libplanet.Tests.Fixtures;
using Libplanet.Crypto;

namespace ribplanet.Tests.Tx
{

    public class TxTest
    {
        private readonly ITestOutputHelper output;

        public TxTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void SerializeTx()
        {
            var privateKey = new PrivateKey(
                new byte[]
                {
                0xcf, 0x36, 0xec, 0xf9, 0xe4, 0x7c, 0x87, 0x9a, 0x0d, 0xbf,
                0x46, 0xb2, 0xec, 0xd8, 0x3f, 0xd2, 0x76, 0x18, 0x2a, 0xde,
                0x02, 0x65, 0x82, 0x5e, 0x3b, 0x8c, 0x6b, 0xa2, 0x14, 0x46,
                0x7b, 0x76,
                }
            );
            var Sender = new Address(privateKey.PublicKey);
            Arithmetic action = Arithmetic.Add(1);
            long txNonce = 1;

            Transaction<Arithmetic> tx = new Transaction<Arithmetic>(privateKey, Sender, action, txNonce);
            var serializedTx = new SerializedTx(tx);
            output.WriteLine(Encoding.Default.GetString(serializedTx.SerializedTransaction));

            string expected = Encoding.Default.GetString(serializedTx.SerializedTransaction);

            Assert.Equal(
                expected,
                Encoding.Default.GetString(tx.SerializeSigned())
            );
        }

        [Fact]
        public void ValidateOwnTransaction()
        {
            PrivateKey key1 = PrivateKey.FromString(
            "ea0493b0ed67fc97b2e5e85a1d145adea294112f09df15398cb10f2ed5ad1a83"
            );
            Hash previousHash = Hash.FromString(
            "341e8f360597d5bc45ab96aabc5f1b0608063f30af7bd4153556c9536a07693a"
            );
            Transaction<Arithmetic> tx = new Transaction<Arithmetic>(key1, new Ribplanet.Address(key1.PublicKey), Arithmetic.Add(1), 1);
            tx.Validate();
        }

        [Fact]
        public void ValidateDeserializedTx()
        {
            PrivateKey key1 = PrivateKey.FromString(
            "ea0493b0ed67fc97b2e5e85a1d145adea294112f09df15398cb10f2ed5ad1a83"
            );
            Hash previousHash = Hash.FromString(
            "341e8f360597d5bc45ab96aabc5f1b0608063f30af7bd4153556c9536a07693a"
            );
            Transaction<Arithmetic> tx = new Transaction<Arithmetic>(key1, new Ribplanet.Address(key1.PublicKey), Arithmetic.Add(1), 1);

            var Tx = new SerializedTx(tx);

            Block<Arithmetic> block = Block<Arithmetic>.Mine(0, 7, DateTimeOffset.UtcNow, new Ribplanet.Address(key1), previousHash, Tx);
            SerializedBlock sBlock = new SerializedBlock(block);

            Block<Arithmetic> desBlock = SerializedBlock.Deserialize(sBlock);
            Transaction<Arithmetic> desTx = SerializedTx.Deserialize(desBlock.Transaction);
            Assert.True(desTx.Validate());
        }
    }
}