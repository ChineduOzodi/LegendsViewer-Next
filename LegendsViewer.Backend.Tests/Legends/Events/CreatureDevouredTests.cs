using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class CreatureDevouredTests
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

        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithBasicProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "victim", Value = "1" },
            new Property { Name = "race", Value = "HUMAN" }
        };

        // Act
        var creatureDevoured = new CreatureDevoured(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(creatureDevoured);
        Assert.AreEqual("HUMAN", creatureDevoured.Race);
    }

    [TestMethod]
    public void Constructor_WithVictimAndEater_ParsesCorrectly()
    {
        // Arrange
        var victim = new HistoricalFigure { Id = 1, Name = "Victim", Icon = "person" };
        var eater = new HistoricalFigure { Id = 2, Name = "Eater", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(victim);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(eater);

        var properties = new List<Property>
        {
            new Property { Name = "victim", Value = "1" },
            new Property { Name = "eater", Value = "2" }
        };

        // Act
        var creatureDevoured = new CreatureDevoured(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(victim, creatureDevoured.Victim);
        Assert.AreEqual(eater, creatureDevoured.Eater);
    }

    [TestMethod]
    public void Constructor_WithRaceAndCaste_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "race", Value = "TROLL" },
            new Property { Name = "caste", Value = "MALE" }
        };

        // Act
        var creatureDevoured = new CreatureDevoured(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual("TROLL", creatureDevoured.Race);
        Assert.AreEqual("MALE", creatureDevoured.Caste);
    }

    [TestMethod]
    public void Constructor_AddsEventToSite()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" }
        };
        var initialEventCount = _site.Events.Count;

        // Act
        var creatureDevoured = new CreatureDevoured(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _site.Events.Count);
    }

    [TestMethod]
    public void Print_WithVictim_ReturnsCorrectFormat()
    {
        // Arrange
        var victim = new HistoricalFigure { Id = 1, Name = "Victim", Icon = "person" };
        var eater = new HistoricalFigure { Id = 2, Name = "Eater", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(victim);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(eater);

        var properties = new List<Property>
        {
            new Property { Name = "victim", Value = "1" },
            new Property { Name = "eater", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };
        var creatureDevoured = new CreatureDevoured(properties, _mockWorld.Object);

        // Act
        var result = creatureDevoured.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("devoured"));
        Assert.IsTrue(result.Contains("Eater"));
        Assert.IsTrue(result.Contains("Victim"));
    }

    [TestMethod]
    public void Print_WithRace_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "race", Value = "TROLL" },
            new Property { Name = "caste", Value = "MALE" },
            new Property { Name = "site_id", Value = "1" }
        };
        var creatureDevoured = new CreatureDevoured(properties, _mockWorld.Object);

        // Act
        var result = creatureDevoured.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("devoured"));
        Assert.IsTrue(result.Contains("MALE"));
        Assert.IsTrue(result.Contains("TROLL"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "victim", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var creatureDevoured = new CreatureDevoured(properties, _mockWorld.Object);

        // Act
        var result = creatureDevoured.Print(link: false);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }
}
