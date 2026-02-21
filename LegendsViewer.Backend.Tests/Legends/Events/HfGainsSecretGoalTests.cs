using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfGainsSecretGoalTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _historicalFigure = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        // Create historical figure
        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Schemer",
            Icon = "person",
            Caste = "male"
        };

        // Setup mock world
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "secret_goal", Value = "immortality" }
        };

        // Act
        var hfGainsSecretGoal = new HfGainsSecretGoal(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_historicalFigure, hfGainsSecretGoal.HistoricalFigure);
        Assert.AreEqual(SecretGoal.Immortality, hfGainsSecretGoal.Goal);
    }

    [TestMethod]
    public void Print_WithLinkTrue_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "secret_goal", Value = "immortality" }
        };
        var hfGainsSecretGoal = new HfGainsSecretGoal(properties, _mockWorld.Object);

        // Act
        var result = hfGainsSecretGoal.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Schemer"));
        Assert.IsTrue(result.Contains("obsessed"));
    }

    [TestMethod]
    public void Constructor_AddsEventToHistoricalFigure()
    {
        // Arrange
        var initialEventCount = _historicalFigure.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "secret_goal", Value = "immortality" }
        };

        // Act
        var hfGainsSecretGoal = new HfGainsSecretGoal(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _historicalFigure.Events.Count);
    }
}
