using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfAbductedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _target = null!;
    private HistoricalFigure _snatcher = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        // Create target
        _target = new HistoricalFigure
        {
            Id = 1,
            Name = "Victim",
            Icon = "person"
        };

        // Create snatcher
        _snatcher = new HistoricalFigure
        {
            Id = 2,
            Name = "Kidnapper",
            Icon = "person"
        };

        // Create site
        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Dark Tower"
        };

        // Setup mock world
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_target);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_snatcher);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "target_hfid", Value = "1" },
            new Property { Name = "snatcher_hfid", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var hfAbducted = new HfAbducted(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_target, hfAbducted.Target);
        Assert.AreEqual(_snatcher, hfAbducted.Snatcher);
        Assert.AreEqual(_site, hfAbducted.Site);
    }

    [TestMethod]
    public void Print_WithLinkTrue_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "target_hfid", Value = "1" },
            new Property { Name = "snatcher_hfid", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };
        var hfAbducted = new HfAbducted(properties, _mockWorld.Object);

        // Act
        var result = hfAbducted.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Kidnapper"));
        Assert.IsTrue(result.Contains("abducted"));
        Assert.IsTrue(result.Contains("Victim"));
        Assert.IsTrue(result.Contains("Dark Tower"));
    }

    [TestMethod]
    public void Print_WithLinkFalse_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "target_hfid", Value = "1" },
            new Property { Name = "snatcher_hfid", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };
        var hfAbducted = new HfAbducted(properties, _mockWorld.Object);

        // Act
        var result = hfAbducted.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("Kidnapper"));
        Assert.IsTrue(result.Contains("Victim"));
    }

    [TestMethod]
    public void Constructor_AddsEventToTarget()
    {
        // Arrange
        var initialEventCount = _target.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "target_hfid", Value = "1" },
            new Property { Name = "snatcher_hfid", Value = "2" }
        };

        // Act
        var hfAbducted = new HfAbducted(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _target.Events.Count);
    }

    [TestMethod]
    public void Constructor_WithNullSnatcher_DoesNotThrow()
    {
        // Arrange
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns((HistoricalFigure?)null);

        var properties = new List<Property>
        {
            new Property { Name = "target_hfid", Value = "1" },
            new Property { Name = "snatcher_hfid", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act & Assert
        var hfAbducted = new HfAbducted(properties, _mockWorld.Object);
        var result = hfAbducted.Print(link: false);
        
        Assert.IsTrue(result.Contains("UNKNOWN HISTORICAL FIGURE"));
    }

    [TestMethod]
    public void Constructor_WithRegion_ParsesCorrectly()
    {
        // Arrange
        var region = new WorldRegion([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Forest"
        };
        _mockWorld.Setup(w => w.GetRegion(1)).Returns(region);

        var properties = new List<Property>
        {
            new Property { Name = "target_hfid", Value = "1" },
            new Property { Name = "snatcher_hfid", Value = "2" },
            new Property { Name = "subregion_id", Value = "1" }
        };
        var hfAbducted = new HfAbducted(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(region, hfAbducted.Region);
    }
}
