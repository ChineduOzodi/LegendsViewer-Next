using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class EntityIncorporatedTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var properties = new List<Property>
        {
            new Property { Name = "joiner_entity_id", Value = "1" },
            new Property { Name = "joined_entity_id", Value = "2" },
            new Property { Name = "site_id", Value = "3" },
            new Property { Name = "leader_hfid", Value = "4" },
            new Property { Name = "partial_incorporation", Value = "true" }
        };

        var evt = new EntityIncorporated(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.IsTrue(evt.PartialIncorporation);
    }

    [TestMethod]
    public void Print_FullIncorporation_ReturnsFormattedString()
    {
        var joiner = new Entity([], _mockWorld.Object) { Id = 1, Name = "Trading Company", Icon = "civilization" };
        var joined = new Entity([], _mockWorld.Object) { Id = 2, Name = "Dwarven Kingdom", Icon = "civilization" };
        var site = new Site([], _mockWorld.Object) { Id = 3, Name = "Fort", Icon = "location" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(joiner);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(joined);
        _mockWorld.Setup(w => w.GetSite(3)).Returns(site);

        var properties = new List<Property>
        {
            new Property { Name = "joiner_entity_id", Value = "1" },
            new Property { Name = "joined_entity_id", Value = "2" },
            new Property { Name = "site_id", Value = "3" }
        };

        var evt = new EntityIncorporated(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Trading Company"));
        Assert.IsTrue(result.Contains("Dwarven Kingdom"));
        Assert.IsTrue(result.Contains("Fort"));
        Assert.IsTrue(result.Contains("fully incorporated"));
    }

    [TestMethod]
    public void Print_PartialIncorporation_ReturnsFormattedString()
    {
        var joiner = new Entity([], _mockWorld.Object) { Id = 1, Name = "Trading Company", Icon = "civilization" };
        var joined = new Entity([], _mockWorld.Object) { Id = 2, Name = "Dwarven Kingdom", Icon = "civilization" };
        var leader = new HistoricalFigure { Id = 4, Name = "Thorin", Icon = "person" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(joiner);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(joined);
        _mockWorld.Setup(w => w.GetHistoricalFigure(4)).Returns(leader);

        var properties = new List<Property>
        {
            new Property { Name = "joiner_entity_id", Value = "1" },
            new Property { Name = "joined_entity_id", Value = "2" },
            new Property { Name = "leader_hfid", Value = "4" },
            new Property { Name = "partial_incorporation", Value = "true" }
        };

        var evt = new EntityIncorporated(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Trading Company"));
        Assert.IsTrue(result.Contains("began operating at the direction"));
        Assert.IsTrue(result.Contains("Thorin"));
    }
}
