using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfInterrogatedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _targetHf = null!;
    private HistoricalFigure _interrogatorHf = null!;
    private Entity _arrestingEntity = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        // Create target
        _targetHf = new HistoricalFigure
        {
            Id = 1,
            Name = "Prisoner",
            Icon = "person"
        };

        // Create interrogator
        _interrogatorHf = new HistoricalFigure
        {
            Id = 2,
            Name = "Inquisitor",
            Icon = "person"
        };

        // Create arresting entity
        _arrestingEntity = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "City Guard",
            Icon = "civilization"
        };

        // Setup mock world
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_targetHf);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_interrogatorHf);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_arrestingEntity);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "target_hfid", Value = "1" },
            new Property { Name = "interrogator_hfid", Value = "2" },
            new Property { Name = "arresting_enid", Value = "1" }
        };

        // Act
        var hfInterrogated = new HfInterrogated(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_targetHf, hfInterrogated.TargetHf);
        Assert.AreEqual(_interrogatorHf, hfInterrogated.InterrogatorHf);
    }

    [TestMethod]
    public void Print_WithLinkTrue_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "target_hfid", Value = "1" },
            new Property { Name = "interrogator_hfid", Value = "2" },
            new Property { Name = "arresting_enid", Value = "1" }
        };
        var hfInterrogated = new HfInterrogated(properties, _mockWorld.Object);

        // Act
        var result = hfInterrogated.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Prisoner"));
        Assert.IsTrue(result.Contains("interrogated"));
    }

    [TestMethod]
    public void Constructor_AddsEventToTarget()
    {
        // Arrange
        var initialEventCount = _targetHf.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "target_hfid", Value = "1" },
            new Property { Name = "interrogator_hfid", Value = "2" }
        };

        // Act
        var hfInterrogated = new HfInterrogated(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _targetHf.Events.Count);
    }
}
