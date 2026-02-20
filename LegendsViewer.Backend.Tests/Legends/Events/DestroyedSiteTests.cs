using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class DestroyedSiteTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };
        _site.Structures = [];
        _site.OwnerHistory = [];

        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithBasicProperties_ParsesCorrectly()
    {
        // Arrange
        var attacker = new Entity([], _mockWorld.Object) { Id = 1, Name = "Attacker", Icon = "civilization" };
        attacker.Honors = [];
        var defender = new Entity([], _mockWorld.Object) { Id = 2, Name = "Defender", Icon = "civilization" };
        defender.Honors = [];
        
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(attacker);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(defender);

        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" }
        };

        // Act
        var destroyedSite = new DestroyedSite(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(destroyedSite);
        Assert.AreEqual(attacker, destroyedSite.Attacker);
    }

    [TestMethod]
    public void Constructor_WithNoDefeat_SetsFlag()
    {
        // Arrange
        var attacker = new Entity([], _mockWorld.Object) { Id = 1, Name = "Attacker", Icon = "civilization" };
        attacker.Honors = [];
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(attacker);

        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "no_defeat_mention", Value = "true" }
        };

        // Act
        var destroyedSite = new DestroyedSite(properties, _mockWorld.Object);

        // Assert
        Assert.IsTrue(destroyedSite.NoDefeatMention);
    }

    [TestMethod]
    public void Constructor_AddsEventToSite()
    {
        // Arrange
        var attacker = new Entity([], _mockWorld.Object) { Id = 1, Name = "Attacker", Icon = "civilization" };
        attacker.Honors = [];
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(attacker);

        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "attacker_civ_id", Value = "1" }
        };
        var initialEventCount = _site.Events.Count;

        // Act
        var destroyedSite = new DestroyedSite(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _site.Events.Count);
    }

    [TestMethod]
    public void Print_WithAttacker_ReturnsCorrectFormat()
    {
        // Arrange
        var attacker = new Entity([], _mockWorld.Object) { Id = 1, Name = "Attacker", Icon = "civilization" };
        attacker.Honors = [];
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(attacker);

        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "attacker_civ_id", Value = "1" }
        };
        var destroyedSite = new DestroyedSite(properties, _mockWorld.Object);

        // Act
        var result = destroyedSite.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("destroyed"));
        Assert.IsTrue(result.Contains("Attacker"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var attacker = new Entity([], _mockWorld.Object) { Id = 1, Name = "Attacker", Icon = "civilization" };
        attacker.Honors = [];
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(attacker);

        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "attacker_civ_id", Value = "1" }
        };
        var destroyedSite = new DestroyedSite(properties, _mockWorld.Object);

        // Act
        var result = destroyedSite.Print(link: false);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }
}
