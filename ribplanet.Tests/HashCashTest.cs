using Xunit;
using Ribplanet;
using Libplanet.Crypto;
using System;
using System.Security.Cryptography;

namespace ribplanet.Tests;

public class HashCashTests
{
    [Fact]
    public void AnswerSatisfiesDifficulty(byte[] challenge, int difficulty)
    {
        Nonce nonce = new Nonce()
        Stamp stamp = challenge. nonce 
        (Nonce answer, Hash digest) = HashCash.Answer(stamp, difficulty);
        Console.WriteLine($"{answer}, {digest}");
    }
}