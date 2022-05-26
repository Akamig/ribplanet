
using System.Text;
using System.Security.Cryptography;
using Ribplanet;
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

        public static Transaction<Arithmetic> Deserialize(SerializedTx ser)
        {
            using SHA256 algo = SHA256.Create();
            Codec codec = new Codec();
            if (ser.TxId == new Hash(algo.ComputeHash(ser.SerializedTransaction)))
            {
                var dict = (Bencodex.Types.Dictionary)codec.Decode(ser.SerializedTransaction);
                Arithmetic action = new Arithmetic();
                action.LoadPlainValue(dict.GetValue<Dictionary>("Action"));
                Transaction<Arithmetic> tx = new Transaction<Arithmetic>(
                    Address.FromString(dict.GetValue<Text>("Sender")),
                    new PublicKey((byte[])dict.GetValue<Binary>("PublicKey")),
                    (byte[])dict.GetValue<Binary>("Signature"),
                    Address.FromString(dict.GetValue<Text>("Recipient")),
                    action,
                    DateTimeOffset.Parse(dict.GetValue<Text>("Timestamp")),
                    dict.GetValue<Integer>("TxNonce"),
                    ser.TxId.hash
                );
                return tx;
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

        public byte[] TxId { get; set; }
        public Address Sender { get; set; }
        public PublicKey PublicKey { get; set; }
        public byte[] Signature { get; set; }
        public Address Recipient { get; set; }
        public Arithmetic Action { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public long TxNonce { get; set; }
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
        public Transaction(Address sender, PublicKey publicKey, byte[] signature, Address recipient, Arithmetic action, DateTimeOffset timestamp, long txNonce, byte[] txid)
        {
            this.Sender = sender;
            this.PublicKey = publicKey;
            this.Signature = signature;
            this.Action = action;
            this.Recipient = recipient;
            this.Timestamp = timestamp;
            this.TxNonce = txNonce;
            this.TxId = txid;
        }

        public byte[] SerializeUnsigned()
        {
            var bdict = Bencodex.Types.Dictionary.Empty
                .Add(nameof(Sender), Sender.ToString())
                .Add(nameof(PublicKey), PublicKey.Format(false))
                .Add(nameof(Recipient), Recipient.ToString())
                .Add(nameof(Action), Action.PlainValue)
                .Add(nameof(Timestamp), Timestamp.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffffzzz"))
                .Add(nameof(TxNonce), TxNonce);

            return new Bencodex.Codec().Encode(bdict);
        }

        public byte[] SerializeSigned()
        {
            var bdict = Bencodex.Types.Dictionary.Empty
                .Add(nameof(Sender), Sender.ToString())
                .Add(nameof(PublicKey), PublicKey.Format(false))
                .Add(nameof(Recipient), Recipient.ToString())
                .Add(nameof(Action), Action.PlainValue)
                .Add(nameof(Timestamp), Timestamp.ToString())
                .Add(nameof(TxNonce), TxNonce)
                .Add(nameof(Signature), Signature);

            return new Bencodex.Codec().Encode(bdict);
        }

        public bool Validate()
        {
            return this.PublicKey.Verify(this.SerializeUnsigned(), this.Signature);
        }

        public bool Equals(Transaction<Arithmetic> other) => TxId.Equals(other?.TxId);
        public override bool Equals(object obj) => obj is Transaction<Arithmetic> other && Equals(other);

    }

}