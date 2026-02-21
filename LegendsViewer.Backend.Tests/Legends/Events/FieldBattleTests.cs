using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class FieldBattleTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _attacker = null!;
    private Entity _defender = null!;
    private HistoricalFigure _attackerGeneral = null!;
    private HistoricalFigure _defenderGeneral = null!;
    private WorldRegion _region = null!;
    private UndergroundRegion _undergroundRegion = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        // Create attacker entity
        _attacker = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Attackers",
            Icon = "civilization"
        };

        // Create defender entity
        _defender = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Defenders",
            Icon = "civilization"
        };

        // Create attacker general
        _attackerGeneral = new HistoricalFigure
        {
            Id = 1,
            Name = "General Ironhand",
            Icon = "person"
        };

        // Create defender general
        _defenderGeneral = new HistoricalFigure
        {
            Id = 2,
            Name = "General Stonewall",
            Icon = "person"
        };

        // Create region
        _region = new WorldRegion([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Valley of War"
        };

        // Create underground region
        _undergroundRegion = new UndergroundRegion([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Deep Caverns"
        };

        // Setup mock world
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_attacker);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_defender);
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_attackerGeneral);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_defenderGeneral);
        _mockWorld.Setup(w => w.GetRegion(1)).Returns(_region);
        _mockWorld.Setup(w => w.GetUndergroundRegion(1)).Returns(_undergroundRegion);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "attacker_general_hfid", Value = "1" },
            new Property { Name = "defender_general_hfid", Value = "2" },
            new Property { Name = "subregion_id", Value = "1" },
            new Property { Name = "feature_layer_id", Value = "1" },
            new Property { Name = "coords", Value = "0,0" }
        };

        // Act
        var fieldBattle = new FieldBattle(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_attacker, fieldBattle.Attacker);
        Assert.AreEqual(_defender, fieldBattle.Defender);
        Assert.AreEqual(_attackerGeneral, fieldBattle.AttackerGeneral);
        Assert.AreEqual(_defenderGeneral, fieldBattle.DefenderGeneral);
        Assert.AreEqual(_region, fieldBattle.Region);
        Assert.AreEqual(_undergroundRegion, fieldBattle.UndergroundRegion);
    }

    [TestMethod]
    public void Print_WithLinkTrue_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "attacker_general_hfid", Value = "1" },
            new Property { Name = "defender_general_hfid", Value = "2" },
            new Property { Name = "subregion_id", Value = "1" },
            new Property { Name = "feature_layer_id", Value = "1" },
            new Property { Name = "coords", Value = "0,0" }
        };
        var fieldBattle = new FieldBattle(properties, _mockWorld.Object);

        // Act
        var result = fieldBattle.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Attackers"));
        Assert.IsTrue(result.Contains("Defenders"));
        Assert.IsTrue(result.Contains("General Ironhand"));
        Assert.IsTrue(result.Contains("General Stonewall"));
        Assert.IsTrue(result.Contains("Valley of War"));
    }

    [TestMethod]
    public void Print_WithLinkFalse_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "subregion_id", Value = "1" }
        };
        var fieldBattle = new FieldBattle(properties, _mockWorld.Object);

        // Act
        var result = fieldBattle.Print(link: false);

        // Assert - basic content should be present
        Assert.IsTrue(result.Contains("Attackers"));
        Assert.IsTrue(result.Contains("Defenders"));
    }

    [TestMethod]
    public void Constructor_WithoutGenerals_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "subregion_id", Value = "1" }
        };

        // Act
        var fieldBattle = new FieldBattle(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(fieldBattle.Attacker);
        Assert.IsNotNull(fieldBattle.Defender);
        Assert.IsNull(fieldBattle.AttackerGeneral);
        Assert.IsNull(fieldBattle.DefenderGeneral);
    }

    [TestMethod]
    public void Print_WithoutGenerals_DoesNotIncludeGeneralText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "subregion_id", Value = "1" }
        };
        var fieldBattle = new FieldBattle(properties, _mockWorld.Object);

        // Act
        var result = fieldBattle.Print(link: false);

        // Assert - without generals, these texts should NOT appear
        Assert.IsFalse(result.Contains("Leader of the attack was"));
        Assert.IsFalse(result.Contains("defenders were led by"));
    }

    [TestMethod]
    public void Constructor_WithMercenaries_ParsesCorrectly()
    {
        // Arrange
        var attackerMercenaries = new Entity([], _mockWorld.Object)
        {
            Id = 3,
            Name = "Mercenary Company",
            Icon = "civilization"
        };

        var defenderMercenaries = new Entity([], _mockWorld.Object)
        {
            Id = 4,
            Name = "Hired Blades",
            Icon = "civilization"
        };

        _mockWorld.Setup(w => w.GetEntity(3)).Returns(attackerMercenaries);
        _mockWorld.Setup(w => w.GetEntity(4)).Returns(defenderMercenaries);

        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "attacker_merc_enid", Value = "3" },
            new Property { Name = "defender_merc_enid", Value = "4" },
            new Property { Name = "subregion_id", Value = "1" }
        };

        // Act
        var fieldBattle = new FieldBattle(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(attackerMercenaries, fieldBattle.AttackerMercenaries);
        Assert.AreEqual(defenderMercenaries, fieldBattle.DefenderMercenaries);
    }

    [TestMethod]
    public void Print_WithMercenaries_IncludesMercenaryText()
    {
        // Arrange
        var attackerMercenaries = new Entity([], _mockWorld.Object)
        {
            Id = 3,
            Name = "Mercenary Company",
            Icon = "civilization"
        };

        _mockWorld.Setup(w => w.GetEntity(3)).Returns(attackerMercenaries);

        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "attacker_merc_enid", Value = "3" },
            new Property { Name = "subregion_id", Value = "1" }
        };
        var fieldBattle = new FieldBattle(properties, _mockWorld.Object);

        // Act
        var result = fieldBattle.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("Mercenary Company"));
        Assert.IsTrue(result.Contains("were hired by the attackers"));
    }

    [TestMethod]
    public void Constructor_WithSupportMercenaries_ParsesCorrectly()
    {
        // Arrange
        var attackerSupportMercenaries = new Entity([], _mockWorld.Object)
        {
            Id = 5,
            Name = "Scout Company",
            Icon = "civilization"
        };

        var defenderSupportMercenaries = new Entity([], _mockWorld.Object)
        {
            Id = 6,
            Name = "Forest Watchers",
            Icon = "civilization"
        };

        _mockWorld.Setup(w => w.GetEntity(5)).Returns(attackerSupportMercenaries);
        _mockWorld.Setup(w => w.GetEntity(6)).Returns(defenderSupportMercenaries);

        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "a_support_merc_enid", Value = "5" },
            new Property { Name = "d_support_merc_enid", Value = "6" },
            new Property { Name = "subregion_id", Value = "1" }
        };

        // Act
        var fieldBattle = new FieldBattle(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(attackerSupportMercenaries, fieldBattle.AttackerSupportMercenaries);
        Assert.AreEqual(defenderSupportMercenaries, fieldBattle.DefenderSupportMercenaries);
    }

    [TestMethod]
    public void Print_WithSupportMercenaries_IncludesScoutText()
    {
        // Arrange
        var attackerSupportMercenaries = new Entity([], _mockWorld.Object)
        {
            Id = 5,
            Name = "Scout Company",
            Icon = "civilization"
        };

        _mockWorld.Setup(w => w.GetEntity(5)).Returns(attackerSupportMercenaries);

        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "a_support_merc_enid", Value = "5" },
            new Property { Name = "subregion_id", Value = "1" }
        };
        var fieldBattle = new FieldBattle(properties, _mockWorld.Object);

        // Act
        var result = fieldBattle.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("Scout Company"));
        Assert.IsTrue(result.Contains("were hired as scouts by the attackers"));
    }

    [TestMethod]
    public void Constructor_AddsEventToEntities()
    {
        // Arrange
        var initialAttackerEventCount = _attacker.Events.Count;
        var initialDefenderEventCount = _defender.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "subregion_id", Value = "1" }
        };

        // Act
        var fieldBattle = new FieldBattle(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialAttackerEventCount + 1, _attacker.Events.Count);
        Assert.AreEqual(initialDefenderEventCount + 1, _defender.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToGenerals()
    {
        // Arrange
        var initialAttackerGeneralEventCount = _attackerGeneral.Events.Count;
        var initialDefenderGeneralEventCount = _defenderGeneral.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "attacker_general_hfid", Value = "1" },
            new Property { Name = "defender_general_hfid", Value = "2" },
            new Property { Name = "subregion_id", Value = "1" },
            new Property { Name = "feature_layer_id", Value = "1" }
        };

        // Act
        var fieldBattle = new FieldBattle(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialAttackerGeneralEventCount + 1, _attackerGeneral.Events.Count);
        Assert.AreEqual(initialDefenderGeneralEventCount + 1, _defenderGeneral.Events.Count);
    }

    [TestMethod]
    public void Constructor_WithNullEntities_DoesNotThrow()
    {
        // Arrange
        _mockWorld.Setup(w => w.GetEntity(It.IsAny<int>())).Returns((Entity?)null);
        _mockWorld.Setup(w => w.GetHistoricalFigure(It.IsAny<int>())).Returns((HistoricalFigure?)null);
        _mockWorld.Setup(w => w.GetRegion(It.IsAny<int>())).Returns((WorldRegion?)null);
        _mockWorld.Setup(w => w.GetUndergroundRegion(It.IsAny<int>())).Returns((UndergroundRegion?)null);

        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "999" },
            new Property { Name = "defender_civ_id", Value = "998" },
            new Property { Name = "attacker_general_hfid", Value = "997" },
            new Property { Name = "defender_general_hfid", Value = "996" },
            new Property { Name = "subregion_id", Value = "995" },
            new Property { Name = "feature_layer_id", Value = "994" }
        };

        // Act & Assert
        var fieldBattle = new FieldBattle(properties, _mockWorld.Object);
        Assert.IsNull(fieldBattle.Attacker);
        Assert.IsNull(fieldBattle.Defender);
        Assert.IsNull(fieldBattle.AttackerGeneral);
        Assert.IsNull(fieldBattle.DefenderGeneral);
        Assert.IsNull(fieldBattle.Region);
        Assert.IsNull(fieldBattle.UndergroundRegion);
    }
}
