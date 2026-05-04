using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfAttackedSiteTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _attacker = null!;
    private Entity _defenderCiv = null!;
    private Entity _siteCiv = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        // Create attacker
        _attacker = new HistoricalFigure
        {
            Id = 1,
            Name = "Warlord",
            Icon = "person"
        };

        // Create defender civ
        _defenderCiv = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Dwarven Kingdom",
            Icon = "civilization"
        };

        // Create site civ
        _siteCiv = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Mountain Hold",
            Icon = "civilization"
        };

        // Create site
        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Ironforge"
        };

        // Setup mock world
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_attacker);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_defenderCiv);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_siteCiv);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_hfid", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "1" },
            new Property { Name = "site_civ_id", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var hfAttackedSite = new HfAttackedSite(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_attacker, hfAttackedSite.Attacker);
        Assert.AreEqual(_defenderCiv, hfAttackedSite.DefenderCiv);
        Assert.AreEqual(_siteCiv, hfAttackedSite.SiteCiv);
        Assert.AreEqual(_site, hfAttackedSite.Site);
    }

    [TestMethod]
    public void Print_WithLinkTrue_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_hfid", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "1" },
            new Property { Name = "site_civ_id", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };
        var hfAttackedSite = new HfAttackedSite(properties, _mockWorld.Object);

        // Act
        var result = hfAttackedSite.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Warlord"));
        Assert.IsTrue(result.Contains("attacked"));
        Assert.IsTrue(result.Contains("Ironforge"));
    }

    [TestMethod]
    public void Print_WithDifferentDefenderAndSiteCiv_IncludesDefenderText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_hfid", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "1" },
            new Property { Name = "site_civ_id", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };
        var hfAttackedSite = new HfAttackedSite(properties, _mockWorld.Object);

        // Act
        var result = hfAttackedSite.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("of Dwarven Kingdom"));
    }

    [TestMethod]
    public void Constructor_AddsEventToAttacker()
    {
        // Arrange
        var initialEventCount = _attacker.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "attacker_hfid", Value = "1" },
            new Property { Name = "site_civ_id", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var hfAttackedSite = new HfAttackedSite(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _attacker.Events.Count);
    }
}
