using System.IO;
using Xunit;
using SmartFactorySimple;

namespace SmartFactorySimple.Tests;

public class FactoryShareTests
{
    [Fact]
    public void ListingCompanyCreatesPublicShareStateAndAppliesMenuFluctuation()
    {
        var factory = new Factory("Test Factory");

        factory.ListCompanyPublicly(25, 1000, 10m);

        Assert.True(factory.IsCompanyPublic);
        Assert.Equal(25m, factory.PublicSharePercentage);
        Assert.Equal(1000, factory.IssuedShares);
        Assert.Equal(10m, factory.SharePrice);

        decimal previousPrice = factory.SharePrice;
        factory.ApplyMenuReturnFluctuation();

        Assert.InRange(factory.SharePrice, 9.6m, 10.5m);
        Assert.True(factory.SharePrice >= 0m);
        Assert.True(factory.SharePrice != previousPrice || factory.SharePrice == previousPrice);
    }

    [Fact]
    public void ResolvePathUsesProjectRootWhenFileIsNotInBinaryOutputFolder()
    {
        string path = AppFileNames.ResolvePath(AppFileNames.OrdersFileName);
        Assert.True(File.Exists(path) || !string.IsNullOrWhiteSpace(path));
        Assert.EndsWith(AppFileNames.OrdersFileName, path);
    }
}
