using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfFreedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _freeingHf = null!;
    private HistoricalFigure _rescuedHf = null!;
    private Entity _freeingCiv = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        // Create freeing figure
        _freeingHf = new HistoricalFigure
        {
            Id = 1,
            Name = "Liberator",
            Icon = "person"
        };

        // Create rescued figure
        _rescuedHf = new HistoricalFigure
        {
            Id = 2,
            Name = "Captive",
            Icon = "person"
        };

        // Create freeing civ
        _freeingCiv = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Rebels",
            Icon = "civilization"
        };

        // Create site
        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Dungeon"
        };

        // Setup mock world
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_freeingHf);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_rescuedHf);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_freeingCiv);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "freeing_hfid", Value = "1" },
            new Property { Name = "rescued_hfid", Value = "2" },
            new Property { Name = "freeing_civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var hfFreed = new HfFreed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_freeingHf, hfFreed.FreeingHf);
        Assert.AreEqual(1, hfFreed.RescuedHistoricalFigures.Count);
    }

    [TestMethod]
    public void Print_WithLinkTrue_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "freeing_hfid", Value = "1" },
            new Property { Name = "rescued_hfid", Value = "2" },
            new Property { Name = "freeing_civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var hfFreed = new HfFreed(properties, _mockWorld.Object);

        // Act
        var result = hfFreed.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Liberator"));
        Assert.IsTrue(result.Contains("freed"));
        Assert.IsTrue(result.Contains("Captive"));
    }

    [TestMethod]
    public void Constructor_AddsEventToRescued()
    {
        // Arrange
        var initialEventCount = _rescuedHf.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "freeing_hfid", Value = "1" },
            new Property { Name = "rescued_hfid", Value = "2" }
        };

        // Act
        var hfFreed = new HfFreed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _rescuedHf.Events.Count);
    }
}
