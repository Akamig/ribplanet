
using System.Text;
using System.Security.Cryptography;
using Libplanet.Crypto;
using Libplanet.Action;
using Libplanet.Tests.Fixtures;
using Bencodex;
using Bencodex.Types;

namespace Ribplanet.Tx
{
    [Serializable]
    public sealed class SerializedTx
    {
        public byte[] SerializedTransaction;
        public Hash TxId;
        public SerializedTx(Transaction<Arithmetic> Tx)
        {
            this.SerializedTransaction = Tx.SerializeSigned();
            this.TxId = new Hash(Tx.TxId);
        }
        public SerializedTx(byte[] SerializedTransaction, Hash TxId)
        {
            this.SerializedTransaction = SerializedTransaction;
            this.TxId = TxId;
        }

        public Transaction<Arithmetic> Deserialize(SerializedTx ser)
        {
            using SHA256 algo = SHA256.Create();
            Codec codec = new Codec();
            if (ser.TxId == new Hash(algo.ComputeHash(ser.SerializedTransaction)))
            {
                var dict = (Bencodex.Types.Dictionary)codec.Decode(ser.SerializedTransaction);
                Transaction<Arithmetic> tx = new Transaction<Arithmetic>(

                )

            }
            else
            {
                throw new InvalidDataException("TX Hash Mismatch.");
            }
        }
    }
    public sealed class Transaction<Arithmetic> : IEquatable<Transaction<Arithmetic>>
        where Arithmetic : IAction, new()
    {

        public byte[] TxId;
        public Address Sender;
        public PublicKey PublicKey;
        public byte[] Signature;
        public Address Recipient;
        public Arithmetic Action;
        public DateTimeOffset Timestamp;
        public long TxNonce;
        public Transaction(PrivateKey privateKey, Address recipient, Arithmetic action, long txNonce)
        {
            this.Sender = Address.GetAddress(privateKey.PublicKey);
            this.PublicKey = privateKey.PublicKey;
            this.Recipient = recipient;
            this.Action = action;
            this.Timestamp = DateTimeOffset.UtcNow;
            this.TxNonce = txNonce;

            this.Signature = privateKey.Sign(SerializeUnsigned());
            using SHA256 algo = SHA256.Create();
            this.TxId = algo.ComputeHash(SerializeSigned());
        }

        public 
        public byte[] SerializeUnsigned()
        {
            var bdict = Bencodex.Types.Dictionary.Empty
                .Add(nameof(Sender), Sender.ToString())
                .Add(nameof(PublicKey), PublicKey.ToString())
                .Add(nameof(Recipient), Recipient.ToString())
                .Add(nameof(Action), Action.PlainValue)
                .Add(nameof(Timestamp), Timestamp.ToString())
                .Add(nameof(TxNonce), TxNonce);

            return new Bencodex.Codec().Encode(bdict);
        }

        public byte[] SerializeSigned()
        {
            var bdict = Bencodex.Types.Dictionary.Empty
                .Add(nameof(Sender), Sender.ToString())
                .Add(nameof(PublicKey), PublicKey.ToString())
                .Add(nameof(Recipient), Recipient.ToString())
                .Add(nameof(Action), Action.PlainValue)
                .Add(nameof(Timestamp), Timestamp.ToString())
                .Add(nameof(Signature), Signature);

            return new Bencodex.Codec().Encode(bdict);
        }
        /*
                public static Transaction<Arithmetic> Deserialize(byte[] bytes, bool validate = true){
                    IValue value = new Codec().Decode(bytes);
                    if (!(value is Bencodex.Types.Dictionary dict))
                    {
                        throw new DecodingException(
                            $"Expected {typeof(Bencodex.Types.Dictionary)} but " +
                            $"{value.GetType()}");
                    }

                    var tx = new Transaction<Arithmetic>(dict);
                    if (validate)
                    {
                        tx.Validate();
                    }

                    return tx;
                }

        */
        public bool Equals(Transaction<Arithmetic> other) => TxId.Equals(other?.TxId);
        public override bool Equals(object obj) => obj is Transaction<Arithmetic> other && Equals(other);

    }

}