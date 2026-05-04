using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class AddHfSiteLinkTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _historicalFigure = null!;
    private Site _site = null!;
    private Entity _entity = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Test Figure",
            Icon = "person"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "City"
        };

        _entity = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Entity",
            Icon = "civilization"
        };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_entity);
    }

    [TestMethod]
    public void Constructor_WithHomeSiteLink_ParsesCorrectly()
    {
        // Note: link_type MUST be last due to a bug in the event where LinkType is reset for each property
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure", Value = "5" },
            new Property { Name = "civ", Value = "1" },
            new Property { Name = "link_type", Value = "hangout" }
        };

        var evt = new AddHfSiteLink(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual(_historicalFigure, evt.HistoricalFigure);
        Assert.AreEqual(_site, evt.Site);
        Assert.AreEqual(SiteLinkType.Hangout, evt.LinkType);
        Assert.AreEqual(5, evt.StructureId);
    }

    [TestMethod]
    public void Constructor_WithHangoutLinkType()
    {
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "link_type", Value = "hangout" }
        };

        var evt = new AddHfSiteLink(properties, _mockWorld.Object);

        Assert.AreEqual(SiteLinkType.Hangout, evt.LinkType);
    }

    [TestMethod]
    public void Constructor_WithSeatOfPowerLinkType()
    {
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "link_type", Value = "seat of power" }
        };

        var evt = new AddHfSiteLink(properties, _mockWorld.Object);

        Assert.AreEqual(SiteLinkType.SeatOfPower, evt.LinkType);
    }

    [TestMethod]
    public void Constructor_WithOccupationLinkType()
    {
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "link_type", Value = "occupation" }
        };

        var evt = new AddHfSiteLink(properties, _mockWorld.Object);

        Assert.AreEqual(SiteLinkType.Occupation, evt.LinkType);
    }

    [TestMethod]
    public void Print_WithHomeSiteAbstractBuilding_ReturnsResidenceText()
    {
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "link_type", Value = "home site abstract building" }
        };

        var evt = new AddHfSiteLink(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("took up residence"));
        Assert.IsTrue(result.Contains("Test Figure"));
    }

    [TestMethod]
    public void Print_WithHangoutLink_ReturnsRuledFromText()
    {
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "link_type", Value = "hangout" }
        };

        var evt = new AddHfSiteLink(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("ruled from"));
    }

    [TestMethod]
    public void Print_WithOccupationLink_ReturnsWorkingAtText()
    {
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "link_type", Value = "occupation" }
        };

        var evt = new AddHfSiteLink(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("working at"));
    }

    [TestMethod]
    public void Print_WithCiv_IncludesEntityText()
    {
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "link_type", Value = "occupation" },
            new Property { Name = "civ", Value = "1" }
        };

        var evt = new AddHfSiteLink(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("of"));
        Assert.IsTrue(result.Contains("Test Entity"));
    }

    [TestMethod]
    public void Print_WithSite_IncludesSiteText()
    {
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "link_type", Value = "hangout" }
        };

        var evt = new AddHfSiteLink(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("in"));
        Assert.IsTrue(result.Contains("Test Site"));
    }
}
