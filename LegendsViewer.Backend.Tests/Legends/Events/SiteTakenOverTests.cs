using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class SiteTakenOverTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _attacker = null!;
    private Entity _defender = null!;
    private Entity _siteEntity = null!;
    private Entity _newSiteEntity = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _attacker = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Victorious Kingdom",
            Icon = "civilization"
        };

        _defender = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Defeated Kingdom",
            Icon = "civilization"
        };

        _siteEntity = new Entity([], _mockWorld.Object)
        {
            Id = 3,
            Name = "Old Government",
            Icon = "civilization"
        };

        _newSiteEntity = new Entity([], _mockWorld.Object)
        {
            Id = 4,
            Name = "New Government",
            Icon = "civilization"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Conquered Fortress",
            Type = "CASTLE"
        };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_attacker);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_defender);
        _mockWorld.Setup(w => w.GetEntity(3)).Returns(_siteEntity);
        _mockWorld.Setup(w => w.GetEntity(4)).Returns(_newSiteEntity);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "site_civ_id", Value = "3" },
            new Property { Name = "new_site_civ_id", Value = "4" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var evt = new SiteTakenOver(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_attacker, evt.Attacker);
        Assert.AreEqual(_defender, evt.Defender);
        Assert.AreEqual(_siteEntity, evt.SiteEntity);
        Assert.AreEqual(_newSiteEntity, evt.NewSiteEntity);
        Assert.AreEqual(_site, evt.Site);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsTakenOverString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "site_civ_id", Value = "3" },
            new Property { Name = "new_site_civ_id", Value = "4" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var evt = new SiteTakenOver(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("defeated"));
        Assert.IsTrue(result.Contains("took over"));
        Assert.IsTrue(result.Contains("Conquered Fortress"));
        Assert.IsTrue(result.Contains("New Government"));
    }
}
