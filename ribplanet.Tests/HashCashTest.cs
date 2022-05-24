using Xunit;
using Ribplanet;
using Ribplanet.Blocks;
using Libplanet.Crypto;
using Libplanet.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ribplanet.Tests;

public class HashCashTests
{
    [Fact]
    public void AnswerSatisfiesDifficulty()
    {
        for (int i = 0; i < 5; i++)
        {
            byte[] challenge = new byte[32];
            new Random().NextBytes(challenge);
            int difficulty = 8;
            PrivateKey key = new PrivateKey(new byte[]
            {
                0x9b, 0xf4, 0x66, 0x4b, 0xa0, 0x9a, 0x89, 0xfa, 0xeb, 0x68, 0x4b,
                0x94, 0xe6, 0x9f, 0xfd, 0xe0, 0x1d, 0x26, 0xae, 0x14, 0xb5, 0x56,
                0x20, 0x4d, 0x3f, 0x6a, 0xb5, 0x8f, 0x61, 0xf7, 0x84, 0x18,
            });
            Func<Nonce, Hash> stamp = (nonce) =>
            {
                var block = new Block<Arithmetic>(0, difficulty, DateTimeOffset.UtcNow, Address.GetAddress(key.PublicKey), Hash.FromString("0"), null, nonce);
                return block.Hash;
            };
            Nonce nonce = HashCash.Answer(stamp, difficulty);
            Block<Arithmetic> block = new Block<Arithmetic>(0, difficulty, DateTimeOffset.UtcNow, Address.GetAddress(key.PublicKey), Hash.FromString("0"), null, nonce);
            Console.WriteLine($"difficulty was {difficulty}");
            Console.WriteLine($"{block.Hash.ToString()}");
            Assert.True(block.Hash.HasLeadingZerobits(difficulty));
        }
    }
    private String ByteArrayToString(byte[] arr)
    {
        return BitConverter.ToString(arr).Replace("-", string.Empty);
    }
}