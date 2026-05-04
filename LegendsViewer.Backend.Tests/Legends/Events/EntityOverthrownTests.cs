using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class EntityOverthrownTests
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
            new Property { Name = "overthrown_hfid", Value = "2" },
            new Property { Name = "instigator_hfid", Value = "3" }
        };

        var evt = new EntityOverthrown(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
    }

    [TestMethod]
    public void Print_WithPositionTaker_ReturnsFormattedString()
    {
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Dwarven Kingdom", Icon = "civilization" };
        var overthrown = new HistoricalFigure { Id = 2, Name = "King", Icon = "person" };
        var instigator = new HistoricalFigure { Id = 3, Name = "General", Icon = "person" };
        var positionTaker = new HistoricalFigure { Id = 4, Name = "New King", Icon = "person" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(overthrown);
        _mockWorld.Setup(w => w.GetHistoricalFigure(3)).Returns(instigator);
        _mockWorld.Setup(w => w.GetHistoricalFigure(4)).Returns(positionTaker);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "overthrown_hfid", Value = "2" },
            new Property { Name = "instigator_hfid", Value = "3" },
            new Property { Name = "pos_taker_hfid", Value = "4" }
        };

        var evt = new EntityOverthrown(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("General"));
        Assert.IsTrue(result.Contains("King"));
        Assert.IsTrue(result.Contains("Dwarven Kingdom"));
        Assert.IsTrue(result.Contains("toppled"));
        Assert.IsTrue(result.Contains("placed"));
    }

    [TestMethod]
    public void Print_SameInstigatorAndPositionTaker_ReturnsFormattedString()
    {
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Dwarven Kingdom", Icon = "civilization" };
        var overthrown = new HistoricalFigure { Id = 2, Name = "King", Icon = "person" };
        var positionTaker = new HistoricalFigure { Id = 3, Name = "General", Icon = "person" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(overthrown);
        _mockWorld.Setup(w => w.GetHistoricalFigure(3)).Returns(positionTaker);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "overthrown_hfid", Value = "2" },
            new Property { Name = "instigator_hfid", Value = "3" },
            new Property { Name = "pos_taker_hfid", Value = "3" }
        };

        var evt = new EntityOverthrown(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("General"));
        Assert.IsTrue(result.Contains("assumed control"));
    }
}
