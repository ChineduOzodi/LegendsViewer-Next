using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class ChangedCreatureTypeTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _changee = null!;
    private HistoricalFigure _changer = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _changee = new HistoricalFigure
        {
            Id = 1,
            Name = "Changee",
            Icon = "person"
        };

        _changer = new HistoricalFigure
        {
            Id = 2,
            Name = "Changer",
            Icon = "person"
        };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_changee);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_changer);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "changee_hfid", Value = "1" },
            new Property { Name = "changer_hfid", Value = "2" },
            new Property { Name = "old_race", Value = "HUMAN" },
            new Property { Name = "new_race", Value = "ELF" }
        };

        // Act
        var changedCreatureType = new ChangedCreatureType(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(changedCreatureType);
        Assert.AreEqual(_changee, changedCreatureType.Changee);
        Assert.AreEqual(_changer, changedCreatureType.Changer);
    }

    [TestMethod]
    public void Constructor_UpdatesChangeeCreatureTypes()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "changee_hfid", Value = "1" },
            new Property { Name = "old_race", Value = "HUMAN" },
            new Property { Name = "new_race", Value = "ELF" }
        };

        // Act
        var changedCreatureType = new ChangedCreatureType(properties, _mockWorld.Object);

        // Assert - creature types should be added
        Assert.IsTrue(_changee.CreatureTypes.Count > 0);
    }

    [TestMethod]
    public void Constructor_AddsEventToChangee()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "changee_hfid", Value = "1" },
            new Property { Name = "new_race", Value = "ELF" }
        };
        var initialEventCount = _changee.Events.Count;

        // Act
        var changedCreatureType = new ChangedCreatureType(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _changee.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToChanger()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "changee_hfid", Value = "1" },
            new Property { Name = "changer_hfid", Value = "2" }
        };
        var initialEventCount = _changer.Events.Count;

        // Act
        var changedCreatureType = new ChangedCreatureType(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _changer.Events.Count);
    }

    [TestMethod]
    public void Print_WithAllProperties_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "changee_hfid", Value = "1" },
            new Property { Name = "changer_hfid", Value = "2" },
            new Property { Name = "old_race", Value = "HUMAN" },
            new Property { Name = "new_race", Value = "ELF" }
        };
        var changedCreatureType = new ChangedCreatureType(properties, _mockWorld.Object);

        // Act
        var result = changedCreatureType.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Changer"));
        Assert.IsTrue(result.Contains("changed"));
        Assert.IsTrue(result.Contains("Changee"));
    }

    [TestMethod]
    public void Print_WithoutChanger_ReturnsUnknownCreature()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "changee_hfid", Value = "1" },
            new Property { Name = "old_race", Value = "HUMAN" },
            new Property { Name = "new_race", Value = "ELF" }
        };
        var changedCreatureType = new ChangedCreatureType(properties, _mockWorld.Object);

        // Act
        var result = changedCreatureType.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("An unknown creature"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "changee_hfid", Value = "1" },
            new Property { Name = "changer_hfid", Value = "2" },
            new Property { Name = "old_race", Value = "HUMAN" },
            new Property { Name = "new_race", Value = "ELF" }
        };
        var changedCreatureType = new ChangedCreatureType(properties, _mockWorld.Object);

        // Act
        var result = changedCreatureType.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("changed"));
    }
}
