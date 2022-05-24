using System;
using System.Numerics;
using Ribplanet;
using Ribplanet.Blocks;
using Libplanet.Crypto;
using Bencodex.Types;
using Libplanet.Tests.Fixtures;
using Libplanet.Action;
using Xunit;
namespace Libplanet.Tests.Blocks
{

    public class BlockHash
    {
        [Fact]
        public void BlockHashTest()
        {
            PrivateKey key = PrivateKey.FromString(
                    "ea0493b0ed67fc97b2e5e85a1d145adea294112f09df15398cb10f2ed5ad1a83"
                );
            Hash previousHash = Hash.FromString(
                    "341e8f360597d5bc45ab96aabc5f1b0608063f30af7bd4153556c9536a07693a"
                );
            Block<Arithmetic> block = Block<Arithmetic>.Mine(0, 20, DateTimeOffset.UtcNow, new Ribplanet.Address(key), previousHash, null);
            Console.WriteLine(block.Hash.ToString());

        }
    }
}