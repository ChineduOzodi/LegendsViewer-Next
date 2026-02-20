using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class AttackedSiteTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _attacker = null!;
    private Entity _defender = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _attacker = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Attacker Entity",
            Icon = "civilization"
        };
        _attacker.Honors = [];

        _defender = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Defender Entity",
            Icon = "civilization"
        };
        _defender.Honors = [];

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };
        _site.Structures = [];

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_attacker);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_defender);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithBasicProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var attackedSite = new AttackedSite(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(attackedSite);
        Assert.AreEqual(_attacker, attackedSite.Attacker);
        Assert.AreEqual(_defender, attackedSite.Defender);
        Assert.AreEqual(_site, attackedSite.Site);
    }

    [TestMethod]
    public void Constructor_WithSiteEntity_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "site_civ_id", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var attackedSite = new AttackedSite(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_defender, attackedSite.SiteEntity);
    }

    [TestMethod]
    public void Constructor_WithGenerals_ParsesCorrectly()
    {
        // Arrange
        var attackerGeneral = new HistoricalFigure
        {
            Id = 1,
            Name = "Attacker General",
            Icon = "person"
        };
        var defenderGeneral = new HistoricalFigure
        {
            Id = 2,
            Name = "Defender General",
            Icon = "person"
        };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(attackerGeneral);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(defenderGeneral);

        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "attacker_general_hfid", Value = "1" },
            new Property { Name = "defender_general_hfid", Value = "2" }
        };

        // Act
        var attackedSite = new AttackedSite(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(attackerGeneral, attackedSite.AttackerGeneral);
        Assert.AreEqual(defenderGeneral, attackedSite.DefenderGeneral);
    }

    [TestMethod]
    public void Constructor_WithMercenaries_ParsesCorrectly()
    {
        // Arrange
        var attackerMerc = new Entity([], _mockWorld.Object)
        {
            Id = 3,
            Name = "Attacker Mercs",
            Icon = "civilization"
        };
        attackerMerc.Honors = [];

        _mockWorld.Setup(w => w.GetEntity(3)).Returns(attackerMerc);

        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "attacker_merc_enid", Value = "3" }
        };

        // Act
        var attackedSite = new AttackedSite(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(attackerMerc, attackedSite.AttackerMercenaries);
    }

    [TestMethod]
    public void Constructor_AddsEventToAttacker()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" }
        };
        var initialEventCount = _attacker.Events.Count;

        // Act
        var attackedSite = new AttackedSite(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _attacker.Events.Count);
    }

    [TestMethod]
    public void Print_WithBasicProperties_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };
        var attackedSite = new AttackedSite(properties, _mockWorld.Object);

        // Act
        var result = attackedSite.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Attacker Entity"));
        Assert.IsTrue(result.Contains("attacked"));
        Assert.IsTrue(result.Contains("Test Site"));
    }

    [TestMethod]
    public void Print_WithGenerals_ReturnsCorrectFormat()
    {
        // Arrange
        var attackerGeneral = new HistoricalFigure
        {
            Id = 1,
            Name = "Attacker General",
            Icon = "person"
        };
        var defenderGeneral = new HistoricalFigure
        {
            Id = 2,
            Name = "Defender General",
            Icon = "person"
        };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(attackerGeneral);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(defenderGeneral);

        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "attacker_general_hfid", Value = "1" },
            new Property { Name = "defender_general_hfid", Value = "2" }
        };
        var attackedSite = new AttackedSite(properties, _mockWorld.Object);

        // Act
        var result = attackedSite.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Leader of the attack"));
        Assert.IsTrue(result.Contains("Attacker General"));
        Assert.IsTrue(result.Contains("defenders were led"));
        Assert.IsTrue(result.Contains("Defender General"));
    }

    [TestMethod]
    public void Print_WithMercenaries_ReturnsCorrectFormat()
    {
        // Arrange
        var attackerMerc = new Entity([], _mockWorld.Object)
        {
            Id = 3,
            Name = "Attacker Mercs",
            Icon = "civilization"
        };
        attackerMerc.Honors = [];
        _mockWorld.Setup(w => w.GetEntity(3)).Returns(attackerMerc);

        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "attacker_merc_enid", Value = "3" }
        };
        var attackedSite = new AttackedSite(properties, _mockWorld.Object);

        // Act
        var result = attackedSite.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("were hired by the attackers"));
        Assert.IsTrue(result.Contains("Attacker Mercs"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };
        var attackedSite = new AttackedSite(properties, _mockWorld.Object);

        // Act
        var result = attackedSite.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("attacked"));
    }
}
