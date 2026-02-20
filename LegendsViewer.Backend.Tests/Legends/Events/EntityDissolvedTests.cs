using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class EntityDissolvedTests
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
            new Property { Name = "reason", Value = "heavy losses in battle" }
        };

        var evt = new EntityDissolved(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual(DissolveReason.HeavyLossesInBattle, evt.Reason);
    }

    [TestMethod]
    public void Print_WithHeavyLosses_ReturnsFormattedString()
    {
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Dwarven Kingdom", Icon = "civilization" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "reason", Value = "heavy losses in battle" }
        };

        var evt = new EntityDissolved(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Dwarven Kingdom"));
        Assert.IsTrue(result.Contains("dissolved"));
        Assert.IsTrue(result.Contains("taking"));
    }

    [TestMethod]
    public void Print_WithLackOfFunds_ReturnsFormattedString()
    {
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Dwarven Kingdom", Icon = "civilization" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "reason", Value = "lack of funds" }
        };

        var evt = new EntityDissolved(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Dwarven Kingdom"));
        Assert.IsTrue(result.Contains("due to"));
    }

    [TestMethod]
    public void Print_WithoutLinks_ReturnsFormattedString()
    {
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Dwarven Kingdom", Icon = "civilization" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" }
        };

        var evt = new EntityDissolved(properties, _mockWorld.Object);

        var result = evt.Print(link: false);

        Assert.IsTrue(result.Contains("Dwarven Kingdom"));
    }
}
