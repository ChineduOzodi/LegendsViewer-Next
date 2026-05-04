using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class EntityEquipmentPurchaseTests
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
            new Property { Name = "new_equipment_level", Value = "3" }
        };

        var evt = new EntityEquipmentPurchase(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual(3, evt.Quality);
    }

    [TestMethod]
    public void Print_WithQuality_ReturnsFormattedString()
    {
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Dwarven Kingdom", Icon = "civilization" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "new_equipment_level", Value = "5" }
        };

        var evt = new EntityEquipmentPurchase(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Dwarven Kingdom"));
        Assert.IsTrue(result.Contains("purchased"));
        Assert.IsTrue(result.Contains("masterwork"));
    }

    [TestMethod]
    public void Print_WithHistoricalFigure_ReturnsFormattedString()
    {
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Dwarven Kingdom", Icon = "civilization" };
        var hf = new HistoricalFigure { Id = 2, Name = "Thorin", Icon = "person" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(hf);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "hfid", Value = "2" }
        };

        var evt = new EntityEquipmentPurchase(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Thorin"));
        Assert.IsTrue(result.Contains("received"));
    }

    [TestMethod]
    public void Print_WithoutLinks_ReturnsFormattedString()
    {
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Dwarven Kingdom", Icon = "civilization" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "new_equipment_level", Value = "2" }
        };

        var evt = new EntityEquipmentPurchase(properties, _mockWorld.Object);

        var result = evt.Print(link: false);

        Assert.IsTrue(result.Contains("Dwarven Kingdom"));
        Assert.IsTrue(result.Contains("finely-crafted"));
    }
}
