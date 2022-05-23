using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Ribplanet;
using Libplanet.Crypto;
using Bencodex;
using Bencodex.Types;

namespace Ribplanet
{
    public sealed class Transaction : IEquatable<Transaction<T>>
        where T : IAction, new()
    {

        public Transaction(
            Address Sender;
        PublicKey PublicKey;
        byte[] Signature;
        Address Recipient;
        IActions<T> Actions;
        DateTimeOffset Timestamp;
        byte[] TxId;
    ){
        
    }

        public Transaction Create(PrivateKey privateKey, Address recipient, DateTimeOffset timestamp)
        {
            Codec codec = new Codec();
            PublicKey publicKey = privateKey.PublicKey;
            Transaction t = new Transaction(
                this.Sender = Address.GetAddress(publicKey),
                this.PublicKey = publicKey,
                this.Signature = new byte[] { 0x00 },
                this.Recipient = recipient,
                //this.Actions = List<Action>
                this.Timestamp = timestamp
            );
            byte[] encoded = codec.Encode(t);

        }
    }
}