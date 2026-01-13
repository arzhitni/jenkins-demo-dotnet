using Xunit;

namespace DemoApi.Tests;

public class SmokeTests
{
    [Fact]
    public void Sum_Works()
    {
        var a = 2;
        var b = 3;
        var sum = a + b;
        Assert.Equal(5, sum);
    }

    [Fact]
    public void Demo_Failing_Test_For_Show()
    {
        Assert.Equal(4, 2 + 2);
    }
}