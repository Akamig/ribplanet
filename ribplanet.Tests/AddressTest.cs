using Xunit;
using Ribplanet;
namespace ribplanet.Tests;

public class UnitTest1
{
    [Fact]
    public void ConstructorwithByteArray()
    {
        byte[] addr = new byte[20]
        {
            0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef, 0xab,
            0xcd, 0xef, 0xab, 0xcd, 0xef, 0xab, 0xcd, 0xef, 0xab,
            0xcd, 0xef,
        };

        Assert.Equal(
    new Address("0123456789ABcdefABcdEfABcdEFabcDEFabCDEF"),
    new Address(addr));

    }
}