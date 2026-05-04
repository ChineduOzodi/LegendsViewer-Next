using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class SpottedLeavingSiteTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _spotter = null!;
    private Entity _leaverCiv = null!;
    private Entity _siteCiv = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _spotter = new HistoricalFigure
        {
            Id = 1,
            Name = "Watchful Guard",
            Icon = "person"
        };

        _leaverCiv = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Leaving Forces",
            Icon = "civilization"
        };

        _siteCiv = new Entity([], _mockWorld.Object)
        {
            Id = 3,
            Name = "Site Guardians",
            Icon = "civilization"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Spotted Fortress",
            Type = "CASTLE"
        };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_spotter);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_leaverCiv);
        _mockWorld.Setup(w => w.GetEntity(3)).Returns(_siteCiv);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "spotter_hfid", Value = "1" },
            new Property { Name = "leaver_civ_id", Value = "2" },
            new Property { Name = "site_civ_id", Value = "3" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var evt = new SpottedLeavingSite(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_spotter, evt.Spotter);
        Assert.AreEqual(_leaverCiv, evt.LeaverCiv);
        Assert.AreEqual(_siteCiv, evt.SiteCiv);
        Assert.AreEqual(_site, evt.Site);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsSpottedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "spotter_hfid", Value = "1" },
            new Property { Name = "leaver_civ_id", Value = "2" },
            new Property { Name = "site_civ_id", Value = "3" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var evt = new SpottedLeavingSite(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("spotted"));
        Assert.IsTrue(result.Contains("slipping out"));
        Assert.IsTrue(result.Contains("Spotted Fortress"));
    }
}
