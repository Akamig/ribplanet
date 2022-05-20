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
            int difficulty = 20;
            Nonce nonce = new Nonce(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0x1a, 0xbc, 0xde, 0xf0, 0x12 });
            IEnumerable<byte[]> Stamp(Nonce nonce) => new[] { challenge, nonce.ToByteArray() };
            (Nonce answer, Hash digest, int count) = HashCash.Answer(Stamp, difficulty);
            Console.WriteLine($"Total Trial = {count}");
            Console.WriteLine($"difficulty was {difficulty}, and digest is {ByteArrayToString(digest.hash)}");
            Console.WriteLine($"Challenge was {ByteArrayToString(challenge)}, and nonce is {ByteArrayToString(answer.ToByteArray())}");
        }

    }

    private String ByteArrayToString(byte[] arr){
        return BitConverter.ToString(arr).Replace("-", string.Empty);
    }
}