using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfReachSummitTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _historicalFigure = null!;
    private WorldRegion _region = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        // Create historical figure
        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Mountain Climber",
            Icon = "person"
        };

        // Create region
        _region = new WorldRegion([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Great Mountain"
        };

        // Setup mock world
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetRegion(1)).Returns(_region);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "group_hfid", Value = "1" },
            new Property { Name = "subregion_id", Value = "1" }
        };

        // Act
        var hfReachSummit = new HfReachSummit(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_historicalFigure, hfReachSummit.HistoricalFigure);
        Assert.AreEqual(_region, hfReachSummit.Region);
    }

    [TestMethod]
    public void Print_WithLinkTrue_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "group_hfid", Value = "1" },
            new Property { Name = "subregion_id", Value = "1" }
        };
        var hfReachSummit = new HfReachSummit(properties, _mockWorld.Object);

        // Act
        var result = hfReachSummit.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Mountain Climber"));
        Assert.IsTrue(result.Contains("reached the summit"));
    }

    [TestMethod]
    public void Constructor_AddsEventToHistoricalFigure()
    {
        // Arrange
        var initialEventCount = _historicalFigure.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "group_hfid", Value = "1" }
        };

        // Act
        var hfReachSummit = new HfReachSummit(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _historicalFigure.Events.Count);
    }
}
