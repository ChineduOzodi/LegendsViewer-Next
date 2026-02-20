using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class EntityCreatedTests
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
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "site_id", Value = "2" },
            new Property { Name = "structure_id", Value = "3" },
            new Property { Name = "creator_hfid", Value = "4" }
        };

        var evt = new EntityCreated(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
    }

    [TestMethod]
    public void Print_WithCreator_ReturnsFormattedString()
    {
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Dwarven Kingdom", Icon = "civilization" };
        var site = new Site([], _mockWorld.Object) { Id = 2, Name = "Fort", Icon = "location" };
        var creator = new HistoricalFigure { Id = 4, Name = "Thorin", Icon = "person" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);
        _mockWorld.Setup(w => w.GetSite(2)).Returns(site);
        _mockWorld.Setup(w => w.GetHistoricalFigure(4)).Returns(creator);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "site_id", Value = "2" },
            new Property { Name = "creator_hfid", Value = "4" }
        };

        var evt = new EntityCreated(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Thorin"));
        Assert.IsTrue(result.Contains("Dwarven Kingdom"));
        Assert.IsTrue(result.Contains("formed"));
    }

    [TestMethod]
    public void Print_WithoutCreator_ReturnsFormattedString()
    {
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Dwarven Kingdom", Icon = "civilization" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" }
        };

        var evt = new EntityCreated(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Dwarven Kingdom"));
        Assert.IsTrue(result.Contains("formed"));
    }

    [TestMethod]
    public void Print_WithoutLinks_ReturnsFormattedString()
    {
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Dwarven Kingdom", Icon = "civilization" };
        var site = new Site([], _mockWorld.Object) { Id = 2, Name = "Fort", Icon = "location" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);
        _mockWorld.Setup(w => w.GetSite(2)).Returns(site);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "site_id", Value = "2" }
        };

        var evt = new EntityCreated(properties, _mockWorld.Object);

        var result = evt.Print(link: false);

        Assert.IsTrue(result.Contains("Dwarven Kingdom"));
        Assert.IsTrue(result.Contains("Fort"));
    }
}
