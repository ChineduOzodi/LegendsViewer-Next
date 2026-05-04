using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfCarouseTests
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
            Name = "Party Group",
            Icon = "person"
        };

        // Create site
        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Tavern"
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
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var hfCarouse = new HfCarouse(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_groupHf, hfCarouse.GroupHf);
        Assert.AreEqual(_site, hfCarouse.Site);
    }

    [TestMethod]
    public void Print_WithLinkTrue_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "group_hfid", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var hfCarouse = new HfCarouse(properties, _mockWorld.Object);

        // Act
        var result = hfCarouse.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Party Group"));
        Assert.IsTrue(result.Contains("caroused"));
    }

    [TestMethod]
    public void Print_WithSite_IncludesSiteName()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "group_hfid", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var hfCarouse = new HfCarouse(properties, _mockWorld.Object);

        // Act
        var result = hfCarouse.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("Tavern"));
    }

    [TestMethod]
    public void Constructor_AddsEventToGroupHf()
    {
        // Arrange
        var initialEventCount = _groupHf.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "group_hfid", Value = "1" }
        };

        // Act
        var hfCarouse = new HfCarouse(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _groupHf.Events.Count);
    }
}
