using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfConfrontedTests
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
            Name = "Suspicious Figure",
            Icon = "person"
        };

        // Create site
        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Town Square"
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
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "situation", Value = "general suspicion" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var hfConfronted = new HfConfronted(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_historicalFigure, hfConfronted.HistoricalFigure);
        Assert.AreEqual(ConfrontSituation.GeneralSuspicion, hfConfronted.Situation);
    }

    [TestMethod]
    public void Print_WithLinkTrue_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "situation", Value = "general suspicion" },
            new Property { Name = "site_id", Value = "1" }
        };
        var hfConfronted = new HfConfronted(properties, _mockWorld.Object);

        // Act
        var result = hfConfronted.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Suspicious Figure"));
        Assert.IsTrue(result.Contains("aroused general suspicion"));
    }

    [TestMethod]
    public void Constructor_WithReason_ParsesReason()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "situation", Value = "general suspicion" },
            new Property { Name = "reason", Value = "murder" }
        };

        // Act
        var hfConfronted = new HfConfronted(properties, _mockWorld.Object);

        // Assert
        Assert.IsTrue(hfConfronted.Reasons.Contains(ConfrontReason.Murder));
    }

    [TestMethod]
    public void Constructor_AddsEventToHistoricalFigure()
    {
        // Arrange
        var initialEventCount = _historicalFigure.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "situation", Value = "general suspicion" }
        };

        // Act
        var hfConfronted = new HfConfronted(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _historicalFigure.Events.Count);
    }
}
