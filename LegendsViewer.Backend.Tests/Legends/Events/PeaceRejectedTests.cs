using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class PeaceRejectedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _source = null!;
    private Entity _destination = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _source = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Source Entity",
            Icon = "civilization"
        };

        _destination = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Destination Entity",
            Icon = "civilization"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "City"
        };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_source);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_destination);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_SetsDecisionToRejected()
    {
        var properties = new List<Property>
        {
            new Property { Name = "source", Value = "1" },
            new Property { Name = "destination", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };

        var evt = new PeaceRejected(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual("rejected", evt.Decision);
    }

    [TestMethod]
    public void Print_WithBothEntities_ReturnsPeaceRejectedText()
    {
        var properties = new List<Property>
        {
            new Property { Name = "source", Value = "1" },
            new Property { Name = "destination", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };

        var evt = new PeaceRejected(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("rejected"));
        Assert.IsTrue(result.Contains("offer of peace"));
        Assert.IsTrue(result.Contains("Source Entity"));
        Assert.IsTrue(result.Contains("Destination Entity"));
    }

    [TestMethod]
    public void Print_WithoutSourceEntity_ReturnsFallbackText()
    {
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" }
        };

        var evt = new PeaceRejected(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Peace rejected"));
    }
}
