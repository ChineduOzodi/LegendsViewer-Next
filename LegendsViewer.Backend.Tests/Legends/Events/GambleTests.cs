using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class GambleTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _gambler = null!;
    private Site _site = null!;
    private Structure _structure = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        var structuresList = new List<Structure>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        _mockWorld.Setup(w => w.Structures).Returns(structuresList);

        // Create gambler
        _gambler = new HistoricalFigure
        {
            Id = 1,
            Name = "Lucky Gambler",
            Icon = "person"
        };

        // Create site
        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Casino",
            Type = "TOWER"
        };
        _site.Coordinates.Add(new Location(0, 0));
        _site.Structures = [];

        // Create structure
        _structure = new Structure([], _mockWorld.Object, _site)
        {
            LocalId = 1,
            Name = "Gambling Den"
        };
        _site.Structures = [_structure];
        _site.Structures.Add(_structure);

        // Setup mock world
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_gambler);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "gambler_hfid", Value = "1" },
            new Property { Name = "old_account", Value = "100" },
            new Property { Name = "new_account", Value = "200" },
            new Property { Name = "structure_id", Value = "1" }
        };

        // Act
        var gamble = new Gamble(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_gambler, gamble.Gambler);
        Assert.AreEqual(_site, gamble.Site);
        Assert.AreEqual(100, gamble.OldAccount);
        Assert.AreEqual(200, gamble.NewAccount);
        Assert.AreEqual(_structure, gamble.Structure);
    }

    [TestMethod]
    public void Print_WithPositiveBalance_ReturnsFortuneText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "gambler_hfid", Value = "1" },
            new Property { Name = "old_account", Value = "100" },
            new Property { Name = "new_account", Value = "6000" }
        };
        var gamble = new Gamble(properties, _mockWorld.Object);

        // Act
        var result = gamble.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("made a fortune"));
    }

    [TestMethod]
    public void Print_WithSmallPositiveBalance_ReturnsDidWellText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "gambler_hfid", Value = "1" },
            new Property { Name = "old_account", Value = "100" },
            new Property { Name = "new_account", Value = "1500" }
        };
        var gamble = new Gamble(properties, _mockWorld.Object);

        // Act
        var result = gamble.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("did well"));
    }

    [TestMethod]
    public void Print_WithNegativeBalance_ReturnsDidPoorlyText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "gambler_hfid", Value = "1" },
            new Property { Name = "old_account", Value = "6000" },
            new Property { Name = "new_account", Value = "4000" }
        };
        var gamble = new Gamble(properties, _mockWorld.Object);

        // Act
        var result = gamble.Print(link: false);

        // Assert - -2000 <= -1000 is true, so it returns "did poorly"
        Assert.IsTrue(result.Contains("did poorly"));
    }

    [TestMethod]
    public void Print_WithEqualBalance_ReturnsBrokeEvenText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "gambler_hfid", Value = "1" },
            new Property { Name = "old_account", Value = "1000" },
            new Property { Name = "new_account", Value = "1000" }
        };
        var gamble = new Gamble(properties, _mockWorld.Object);

        // Act
        var result = gamble.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("broke even"));
    }

    [TestMethod]
    public void Print_WithSite_IncludesSiteName()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "gambler_hfid", Value = "1" },
            new Property { Name = "old_account", Value = "100" },
            new Property { Name = "new_account", Value = "100" }
        };
        var gamble = new Gamble(properties, _mockWorld.Object);

        // Act
        var result = gamble.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("Casino"));
        Assert.IsTrue(result.Contains("gambling"));
    }

    [TestMethod]
    public void Constructor_AddsEventToEntities()
    {
        // Arrange
        var initialGamblerEventCount = _gambler.Events.Count;
        var initialSiteEventCount = _site.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "gambler_hfid", Value = "1" },
            new Property { Name = "old_account", Value = "100" },
            new Property { Name = "new_account", Value = "100" }
        };

        // Act
        var gamble = new Gamble(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialGamblerEventCount + 1, _gambler.Events.Count);
        Assert.AreEqual(initialSiteEventCount + 1, _site.Events.Count);
    }
}
