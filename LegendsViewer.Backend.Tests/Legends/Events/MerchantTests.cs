using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class MerchantTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        var e1 = new Entity([], _mockWorld.Object) { Id = 1, Name = "Source", Icon = "civilization" };
        var e2 = new Entity([], _mockWorld.Object) { Id = 2, Name = "Dest", Icon = "civilization" };
        var site = new Site([], _mockWorld.Object) { Id = 1, Name = "City" };
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(e1);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(e2);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(site);
    }

    [TestMethod]
    public void Print_ContainsMerchantText()
    {
        var props = new List<Property>
        {
            new() { Name = "source", Value = "1" },
            new() { Name = "destination", Value = "2" },
            new() { Name = "site", Value = "1" }
        };
        var evt = new Merchant(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("merchants"));
    }
}
