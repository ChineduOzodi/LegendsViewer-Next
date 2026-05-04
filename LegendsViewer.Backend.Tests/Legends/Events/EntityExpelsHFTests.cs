using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class EntityExpelsHFTests
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
            new Property { Name = "hfid", Value = "2" },
            new Property { Name = "site_id", Value = "3" }
        };

        var evt = new EntityExpelsHf(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Dwarven Kingdom", Icon = "civilization" };
        var hf = new HistoricalFigure { Id = 2, Name = "Thorin", Icon = "person" };
        var site = new Site([], _mockWorld.Object) { Id = 3, Name = "Fort", Icon = "location" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(hf);
        _mockWorld.Setup(w => w.GetSite(3)).Returns(site);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "hfid", Value = "2" },
            new Property { Name = "site_id", Value = "3" }
        };

        var evt = new EntityExpelsHf(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Dwarven Kingdom"));
        Assert.IsTrue(result.Contains("Thorin"));
        Assert.IsTrue(result.Contains("Fort"));
        Assert.IsTrue(result.Contains("expelled"));
    }

    [TestMethod]
    public void Print_WithoutLinks_ReturnsFormattedString()
    {
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Dwarven Kingdom", Icon = "civilization" };
        var hf = new HistoricalFigure { Id = 2, Name = "Thorin", Icon = "person" };
        var site = new Site([], _mockWorld.Object) { Id = 3, Name = "Fort", Icon = "location" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(hf);
        _mockWorld.Setup(w => w.GetSite(3)).Returns(site);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "hfid", Value = "2" },
            new Property { Name = "site_id", Value = "3" }
        };

        var evt = new EntityExpelsHf(properties, _mockWorld.Object);

        var result = evt.Print(link: false);

        Assert.IsTrue(result.Contains("Dwarven Kingdom"));
        Assert.IsTrue(result.Contains("Thorin"));
        Assert.IsTrue(result.Contains("Fort"));
    }
}
