using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class SneakIntoSiteTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _attacker = null!;
    private Entity _defender = null!;
    private Entity _siteCiv = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _attacker = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Infiltrators",
            Icon = "civilization"
        };

        _defender = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Guarding Kingdom",
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
            Name = "Secret Fortress",
            Type = "CAVE"
        };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_attacker);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_defender);
        _mockWorld.Setup(w => w.GetEntity(3)).Returns(_siteCiv);
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
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var evt = new SneakIntoSite(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_attacker, evt.Attacker);
        Assert.AreEqual(_defender, evt.Defender);
        Assert.AreEqual(_siteCiv, evt.SiteCiv);
        Assert.AreEqual(_site, evt.Site);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsSneakString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "site_civ_id", Value = "3" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var evt = new SneakIntoSite(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("slipped into"));
        Assert.IsTrue(result.Contains("Secret Fortress"));
        Assert.IsTrue(result.Contains("undetected"));
    }
}
