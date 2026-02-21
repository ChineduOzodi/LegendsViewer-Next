using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class FirstContactTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _contactor = null!;
    private Entity _contacted = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        // Create contactor entity
        _contactor = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Explorers",
            Icon = "civilization"
        };

        // Create contacted entity
        _contacted = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Locals",
            Icon = "civilization"
        };

        // Create site
        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Trading Post"
        };

        // Setup mock world
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_contactor);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_contacted);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "contactor_enid", Value = "1" },
            new Property { Name = "contacted_enid", Value = "2" }
        };

        // Act
        var firstContact = new FirstContact(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_site, firstContact.Site);
        Assert.AreEqual(_contactor, firstContact.Contactor);
        Assert.AreEqual(_contacted, firstContact.Contacted);
    }

    [TestMethod]
    public void Print_WithLinkTrue_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "contactor_enid", Value = "1" },
            new Property { Name = "contacted_enid", Value = "2" }
        };
        var firstContact = new FirstContact(properties, _mockWorld.Object);

        // Act
        var result = firstContact.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Explorers"));
        Assert.IsTrue(result.Contains("Locals"));
        Assert.IsTrue(result.Contains("Trading Post"));
        Assert.IsTrue(result.Contains("made contact with"));
    }

    [TestMethod]
    public void Print_WithLinkFalse_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "contactor_enid", Value = "1" },
            new Property { Name = "contacted_enid", Value = "2" }
        };
        var firstContact = new FirstContact(properties, _mockWorld.Object);

        // Act
        var result = firstContact.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("Explorers"));
        Assert.IsTrue(result.Contains("Locals"));
        Assert.IsTrue(result.Contains("Trading Post"));
    }

    [TestMethod]
    public void Constructor_AddsEventToEntities()
    {
        // Arrange
        var initialContactorEventCount = _contactor.Events.Count;
        var initialContactedEventCount = _contacted.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "contactor_enid", Value = "1" },
            new Property { Name = "contacted_enid", Value = "2" }
        };

        // Act
        var firstContact = new FirstContact(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialContactorEventCount + 1, _contactor.Events.Count);
        Assert.AreEqual(initialContactedEventCount + 1, _contacted.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToSite()
    {
        // Arrange
        var initialSiteEventCount = _site.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "contactor_enid", Value = "1" },
            new Property { Name = "contacted_enid", Value = "2" }
        };

        // Act
        var firstContact = new FirstContact(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialSiteEventCount + 1, _site.Events.Count);
    }

    [TestMethod]
    public void Constructor_WithNullEntities_DoesNotThrow()
    {
        // Arrange
        _mockWorld.Setup(w => w.GetEntity(It.IsAny<int>())).Returns((Entity?)null);
        _mockWorld.Setup(w => w.GetSite(It.IsAny<int>())).Returns((Site?)null);

        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "999" },
            new Property { Name = "contactor_enid", Value = "998" },
            new Property { Name = "contacted_enid", Value = "997" }
        };

        // Act & Assert
        var firstContact = new FirstContact(properties, _mockWorld.Object);
        Assert.IsNull(firstContact.Site);
        Assert.IsNull(firstContact.Contactor);
        Assert.IsNull(firstContact.Contacted);
    }
}
