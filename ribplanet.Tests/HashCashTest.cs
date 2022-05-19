using Xunit;
using Ribplanet;
using Libplanet.Crypto;
namespace ribplanet.Tests;

public class HashCashTests
{
    public void HasLeadingZeroBitsTest()
    {
        bool f(byte[] digest, int bits)
        {
            Console.WriteLine($"Expect leading ${bits} zero bits in the {digest} ")
            Console.WriteLine($"{digest.hex()} = {" ".)}")
        }

    }
}