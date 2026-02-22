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
    private Entity _source = null!;
    private Entity _dest = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        
        _source = new Entity([], _mockWorld.Object) { Id = 1, Name = "Source", Icon = "civilization" };
        _dest = new Entity([], _mockWorld.Object) { Id = 2, Name = "Dest", Icon = "civilization" };
        var site = new Site([], _mockWorld.Object) { Id = 1, Name = "City", Icon = "location" };
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_source);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_dest);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "source", Value = "1" },
            new Property { Name = "destination", Value = "2" },
            new Property { Name = "site", Value = "1" }
        };

        var evt = new Merchant(props, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual(_source, evt.Source);
        Assert.AreEqual(_dest, evt.Destination);
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
