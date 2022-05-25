using Xunit;
using Xunit.Abstractions;
using Ribplanet;
using Ribplanet.Tx;
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
    }
}