using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class CeremonyTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _civ = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _civ = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Civ",
            Icon = "civilization"
        };
        _civ.Honors = [];

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };
        _site.Structures = [];

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_civ);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_SetsOccasionTypeToCeremony()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var ceremony = new Ceremony(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(OccasionType.Ceremony, ceremony.OccasionType);
    }

    [TestMethod]
    public void Constructor_WithBasicProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var ceremony = new Ceremony(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(ceremony);
        Assert.AreEqual(_civ, ceremony.Civ);
        Assert.AreEqual(_site, ceremony.Site);
    }

    [TestMethod]
    public void Constructor_AddsEventToCiv()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" }
        };
        var initialEventCount = _civ.Events.Count;

        // Act
        var ceremony = new Ceremony(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _civ.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToSite()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" }
        };
        var initialEventCount = _site.Events.Count;

        // Act
        var ceremony = new Ceremony(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _site.Events.Count);
    }

    [TestMethod]
    public void Print_ReturnsNonEmptyString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var ceremony = new Ceremony(properties, _mockWorld.Object);

        // Act
        var result = ceremony.Print(link: true);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsNonEmptyString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var ceremony = new Ceremony(properties, _mockWorld.Object);

        // Act
        var result = ceremony.Print(link: false);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }
}
