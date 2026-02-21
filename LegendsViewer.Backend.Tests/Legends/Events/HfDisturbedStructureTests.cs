using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfDisturbedStructureTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _historicalFigure = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        // Create historical figure
        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Adventurer",
            Icon = "person"
        };

        // Create site
        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Old Tomb"
        };

        // Setup mock world
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hist_fig_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure_id", Value = "5" }
        };

        // Act
        var hfDisturbedStructure = new HfDisturbedStructure(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_historicalFigure, hfDisturbedStructure.HistoricalFigure);
        Assert.AreEqual(_site, hfDisturbedStructure.Site);
    }

    [TestMethod]
    public void Print_WithLinkTrue_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hist_fig_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure_id", Value = "5" }
        };
        var hfDisturbedStructure = new HfDisturbedStructure(properties, _mockWorld.Object);

        // Act
        var result = hfDisturbedStructure.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Adventurer"));
        Assert.IsTrue(result.Contains("disturbed"));
        Assert.IsTrue(result.Contains("Old Tomb"));
    }

    [TestMethod]
    public void Constructor_AddsEventToHistoricalFigure()
    {
        // Arrange
        var initialEventCount = _historicalFigure.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "hist_fig_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var hfDisturbedStructure = new HfDisturbedStructure(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _historicalFigure.Events.Count);
    }
}
