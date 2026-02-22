using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class SiteRetiredTests
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
            Name = "Retired Faction",
            Icon = "civilization"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Former Outpost",
            Type = "TOWER"
        };

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
        var evt = new SiteRetired(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_civ, evt.Civ);
        Assert.AreEqual(_siteEntity, evt.SiteEntity);
        Assert.AreEqual(_site, evt.Site);
    }

    [TestMethod]
    public void Constructor_WithFirstProperty_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_civ_id", Value = "2" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "first", Value = "true" }
        };

        // Act
        var evt = new SiteRetired(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual("true", evt.First);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsRetiredString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_civ_id", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var evt = new SiteRetired(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("regained their senses"));
        Assert.IsTrue(result.Contains("Former Outpost"));
    }
}
