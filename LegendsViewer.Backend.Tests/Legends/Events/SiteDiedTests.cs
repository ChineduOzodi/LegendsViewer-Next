using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class SiteDiedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _civ = null!;
    private Entity _siteEntity = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _civ = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Kingdom",
            Icon = "civilization"
        };

        _siteEntity = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Site Faction",
            Icon = "civilization"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Dead Settlement",
            Type = "TOWER"
        };

        // Setup proper owner history
        var ownerPeriod = new OwnerPeriod(_site, _civ, -1, "established");
        _site.OwnerHistory.Add(ownerPeriod);
        
        // Setup Entity.SiteHistory (also uses OwnerPeriod)
        var siteEntityPeriod = new OwnerPeriod(_site, _siteEntity, -1, "occupied");
        _siteEntity.SiteHistory.Add(siteEntityPeriod);
        
        // Setup Civ.SiteHistory
        var civSitePeriod = new OwnerPeriod(_site, _civ, -1, "occupied");
        _civ.SiteHistory.Add(civSitePeriod);

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_civ);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_siteEntity);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_civ_id", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var evt = new SiteDied(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_civ, evt.Civ);
        Assert.AreEqual(_siteEntity, evt.SiteEntity);
        Assert.AreEqual(_site, evt.Site);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsWitheredString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_civ_id", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var evt = new SiteDied(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("withered"));
        Assert.IsTrue(result.Contains("Dead Settlement"));
    }

    [TestMethod]
    public void Print_WithAbandonedFlag_ReturnsAbandonedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_civ_id", Value = "2" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "abandoned", Value = "" }
        };

        // Act
        var evt = new SiteDied(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("abandoned"));
    }
}
