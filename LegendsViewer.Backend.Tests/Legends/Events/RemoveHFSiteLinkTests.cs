using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class RemoveHFSiteLinkTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _historicalFigure = null!;
    private Entity _civ = null!;
    private Site _site = null!;
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
            Name = "Test Figure",
            Icon = "person"
        };

        _civ = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Kingdom",
            Icon = "civilization"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };

        _structure = new Structure([], _mockWorld.Object, _site)
        {
            LocalId = 42,
            Name = "Test Structure",
            Type = "TOWER"
        };
        _structuresList.Add(_structure);
        _site.Structures.Add(_structure);

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_civ);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "civ", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure", Value = "42" },
            new Property { Name = "link_type", Value = "home_site_building" }
        };

        // Act
        var evt = new RemoveHfSiteLink(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_historicalFigure, evt.HistoricalFigure);
        Assert.AreEqual(_civ, evt.Civ);
        Assert.AreEqual(_site, evt.Site);
        Assert.AreEqual(SiteLinkType.HomeSiteBuilding, evt.LinkType);
    }

    [TestMethod]
    public void Print_WithHomeSiteLink_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "civ", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure", Value = "42" },
            new Property { Name = "link_type", Value = "home_site_building" }
        };

        // Act
        var evt = new RemoveHfSiteLink(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert - just check that we get some output
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public void Print_WithHangoutLink_ReturnsStoppedRulingString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "civ", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure", Value = "42" },
            new Property { Name = "link_type", Value = "hangout" }
        };

        // Act
        var evt = new RemoveHfSiteLink(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("stopped ruling from"));
    }
}
