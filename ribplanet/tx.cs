using System;
using Ribplanet;
using Libplanet.Crypto;

namespace Ribplanet
{
    public class Transaction
    {
        public Address Sender;
        public PublicKey PublicKey;
        public byte[] Signature;
        public Address Recipient;
        public IImmutableList<T> Actions;
        public DateTimeOffset Timestamp;
        public byte[] TxId;

        public Transaction Create(PrivateKey privateKey, Address recipient, DateTimeOffset timestamp){
            publicKey = privateKey.PublicKey;
        }
    }
}