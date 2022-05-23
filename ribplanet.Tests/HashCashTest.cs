using Xunit;
using Ribplanet;
using Libplanet.Crypto;
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
            byte[] challenge = new byte[40];
            new Random().NextBytes(challenge);
            int difficulty = 8;
            Nonce nonce = new Nonce();
            IEnumerable<byte[]> Stamp(Nonce nonce) => new[] { challenge, nonce.ToByteArray() };
            (Nonce answer, Hash digest) = HashCash.Answer(Stamp, difficulty);
            Console.WriteLine($"difficulty was {difficulty}, and digest is {ByteArrayToString(digest.hash)}");
            Console.WriteLine($"Challenge was {ByteArrayToString(challenge)}, and nonce is {ByteArrayToString(answer.ToByteArray())}");
            Assert.True(digest.HasLeadingZerobits(difficulty));
        }
    }

    private String ByteArrayToString(byte[] arr){
        return BitConverter.ToString(arr).Replace("-", string.Empty);
    }
}