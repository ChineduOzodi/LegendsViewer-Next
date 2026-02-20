using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class AddHfSiteLinkTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _historicalFigure = null!;
    private Site _site = null!;
    private Entity _civ = null!;
    private Structure _structure = null!;
    private List<Structure> _structuresList = null!;

    [TestInitialize]
    public void Setup()
    {
        _structuresList = [];
        
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        _mockWorld.Setup(w => w.Structures).Returns(_structuresList);

        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Hero Figure",
            Icon = "person"
        };

        _civ = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Civilization",
            Icon = "civilization"
        };

        // Create site
        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };
        _site.Coordinates.Add(new Location(0, 0));

        // Create structure - this will add itself to _structuresList via the constructor
        _structure = new Structure([], _mockWorld.Object, _site)
        {
            Id = 0, // Will be set to _structuresList.Count by constructor
            LocalId = 100,
            Name = "Great Hall",
            TypeEnum = StructureType.MeadHall
        };
        
        // Add structure to site's structures
        _site.Structures = [_structure];

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_civ);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "civ", Value = "1" },
            new Property { Name = "structure", Value = "100" },
            new Property { Name = "link_type", Value = "home site building" }
        };

        // Act
        var addHfSiteLink = new AddHfSiteLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_historicalFigure, addHfSiteLink.HistoricalFigure);
        Assert.AreEqual(_site, addHfSiteLink.Site);
        Assert.AreEqual(_civ, addHfSiteLink.Civ);
        Assert.AreEqual(100, addHfSiteLink.StructureId);
        Assert.AreEqual(SiteLinkType.HomeSiteBuilding, addHfSiteLink.LinkType);
    }

    [TestMethod]
    public void Constructor_WithHomeSiteLinkType_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "link_type", Value = "home site building" }
        };

        // Act
        var addHfSiteLink = new AddHfSiteLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(SiteLinkType.HomeSiteBuilding, addHfSiteLink.LinkType);
    }

    [TestMethod]
    public void Constructor_WithHangoutLinkType_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "link_type", Value = "hangout" }
        };

        // Act
        var addHfSiteLink = new AddHfSiteLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(SiteLinkType.Hangout, addHfSiteLink.LinkType);
    }

    [TestMethod]
    public void Constructor_WithOccupationLinkType_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "link_type", Value = "occupation" }
        };

        // Act
        var addHfSiteLink = new AddHfSiteLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(SiteLinkType.Occupation, addHfSiteLink.LinkType);
    }

    [TestMethod]
    public void Constructor_WithSeatOfPowerLinkType_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "link_type", Value = "seat of power" }
        };

        // Act
        var addHfSiteLink = new AddHfSiteLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(SiteLinkType.SeatOfPower, addHfSiteLink.LinkType);
    }

    [TestMethod]
    public void Constructor_AddsEventToHistoricalFigure()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "link_type", Value = "home site building" }
        };
        var initialEventCount = _historicalFigure.Events.Count;

        // Act
        var addHfSiteLink = new AddHfSiteLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _historicalFigure.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToSite()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "link_type", Value = "home site building" }
        };
        var initialEventCount = _site.Events.Count;

        // Act
        var addHfSiteLink = new AddHfSiteLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _site.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToCiv()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "civ", Value = "1" },
            new Property { Name = "link_type", Value = "home site building" }
        };
        var initialEventCount = _civ.Events.Count;

        // Act
        var addHfSiteLink = new AddHfSiteLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _civ.Events.Count);
    }

    [TestMethod]
    public void Print_HomeSiteBuilding_ReturnsCorrectFormat()
    {
        // Arrange - use "home site realization building" which is handled in Print()
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure", Value = "100" },
            new Property { Name = "link_type", Value = "home site realization building" }
        };
        var addHfSiteLink = new AddHfSiteLink(properties, _mockWorld.Object);

        // Act
        var result = addHfSiteLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Hero Figure"));
        Assert.IsTrue(result.Contains("took up residence in"));
        Assert.IsTrue(result.Contains("Great Hall"));
    }

    [TestMethod]
    public void Print_WithCiv_ReturnsCivInOutput()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "civ", Value = "1" },
            new Property { Name = "structure", Value = "100" },
            new Property { Name = "link_type", Value = "home site building" }
        };
        var addHfSiteLink = new AddHfSiteLink(properties, _mockWorld.Object);

        // Act
        var result = addHfSiteLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Test Civilization"));
        Assert.IsTrue(result.Contains("of"));
    }

    [TestMethod]
    public void Print_WithSite_ReturnsSiteInOutput()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure", Value = "100" },
            new Property { Name = "link_type", Value = "home site building" }
        };
        var addHfSiteLink = new AddHfSiteLink(properties, _mockWorld.Object);

        // Act
        var result = addHfSiteLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Test Site"));
        Assert.IsTrue(result.Contains("in"));
    }

    [TestMethod]
    public void Print_Hangout_ReturnsRuledFromFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure", Value = "100" },
            new Property { Name = "link_type", Value = "hangout" }
        };
        var addHfSiteLink = new AddHfSiteLink(properties, _mockWorld.Object);

        // Act
        var result = addHfSiteLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("ruled from"));
    }

    [TestMethod]
    public void Print_UnknownLinkType_ReturnsUnknownFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "link_type", Value = "unknown_type" }
        };
        var addHfSiteLink = new AddHfSiteLink(properties, _mockWorld.Object);

        // Act
        var result = addHfSiteLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("UNKNOWN LINKTYPE"));
    }

    [TestMethod]
    public void Print_WithNullHistoricalFigure_ReturnsUnknownText()
    {
        // Arrange
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns((HistoricalFigure?)null);

        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "link_type", Value = "home site building" }
        };
        var addHfSiteLink = new AddHfSiteLink(properties, _mockWorld.Object);

        // Act
        var result = addHfSiteLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("UNKNOWN HISTORICAL FIGURE"));
    }

    [TestMethod]
    public void Print_WithNullSite_ReturnsUnknownStructureText()
    {
        // Arrange
        _mockWorld.Setup(w => w.GetSite(1)).Returns((Site?)null);

        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure", Value = "100" },
            new Property { Name = "link_type", Value = "home site building" }
        };
        var addHfSiteLink = new AddHfSiteLink(properties, _mockWorld.Object);

        // Act
        var result = addHfSiteLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("UNKNOWN STRUCTURE"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange - use "home site realization building" which is handled in Print()
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure", Value = "100" },
            new Property { Name = "link_type", Value = "home site realization building" }
        };
        var addHfSiteLink = new AddHfSiteLink(properties, _mockWorld.Object);

        // Act
        var result = addHfSiteLink.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("took up residence in"));
    }
}
