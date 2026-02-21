using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfEquipmentPurchaseTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _groupHf = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        // Create historical figure
        _groupHf = new HistoricalFigure
        {
            Id = 1,
            Name = "Adventurer",
            Icon = "person"
        };

        // Create site
        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Smithy"
        };

        // Setup mock world
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_groupHf);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "group_hfid", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "quality", Value = "5" }
        };

        // Act
        var hfEquipmentPurchase = new HfEquipmentPurchase(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_groupHf, hfEquipmentPurchase.GroupHistoricalFigure);
        Assert.AreEqual(_site, hfEquipmentPurchase.Site);
        Assert.AreEqual(5, hfEquipmentPurchase.Quality);
    }

    [TestMethod]
    public void Print_WithLinkTrue_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "group_hfid", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "quality", Value = "5" }
        };
        var hfEquipmentPurchase = new HfEquipmentPurchase(properties, _mockWorld.Object);

        // Act
        var result = hfEquipmentPurchase.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Adventurer"));
        Assert.IsTrue(result.Contains("purchased"));
        Assert.IsTrue(result.Contains("masterwork"));
    }

    [TestMethod]
    public void Constructor_AddsEventToGroupHf()
    {
        // Arrange
        var initialEventCount = _groupHf.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "group_hfid", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var hfEquipmentPurchase = new HfEquipmentPurchase(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _groupHf.Events.Count);
    }
}
