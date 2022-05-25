
using Ribplanet;
using System.Security.Cryptography;
using Libplanet.Crypto;
using Libplanet.Action;
using Libplanet.Tests.Fixtures;
using Bencodex;
using Bencodex.Types;

namespace Ribplanet.Tx
{
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

        public Transaction(byte[] txId, PublicKey publicKey, Address recipient, Arithmetic action)
        {
            this.Sender = Address.GetAddress(publicKey);
            this.PublicKey = publicKey;
            this.Signature = new byte[] { 0x00 };
            this.Recipient = recipient;
            this.Action = action;
            this.Timestamp = DateTimeOffset.UtcNow;

            byte[] payload = Serialize();
            Console.WriteLine(payload.ToString());
            using SHA256 algo = SHA256.Create();
            this.TxId = algo.ComputeHash(payload);
        }
        private byte[] Serialize()
        {
            var bdict = Bencodex.Types.Dictionary.Empty
                .Add(nameof(Sender), Sender.ToString())
                .Add(nameof(PublicKey), PublicKey.ToString())
                .Add(nameof(Signature), Signature)
                .Add(nameof(Recipient), Recipient.ToString())
                .Add(nameof(Action), Action.PlainValue)
                .Add(nameof(Timestamp), Timestamp.ToString());

            return new Bencodex.Codec().Encode(bdict);
        }
        public bool Equals(Transaction<Arithmetic> other) => TxId.Equals(other?.TxId);
        public override bool Equals(object obj) => obj is Transaction<Arithmetic> other && Equals(other);

    }

}