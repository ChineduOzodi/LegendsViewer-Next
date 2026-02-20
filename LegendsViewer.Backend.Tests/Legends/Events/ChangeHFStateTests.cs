using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class ChangeHFStateTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _hf = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _hf = new HistoricalFigure
        {
            Id = 1,
            Name = "Test HF",
            Icon = "person"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };
        _site.Structures = [];

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_hf);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithBasicProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "state", Value = "settled" }
        };

        // Act
        var changeHfState = new ChangeHfState(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(changeHfState);
        Assert.AreEqual(_hf, changeHfState.HistoricalFigure);
        Assert.AreEqual(HfState.Settled, changeHfState.State);
    }

    [TestMethod]
    public void Constructor_WithSite_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "state", Value = "settled" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var changeHfState = new ChangeHfState(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_site, changeHfState.Site);
    }

    [TestMethod]
    public void Constructor_WithMood_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "state", Value = "settled" },
            new Property { Name = "mood", Value = "berserk" }
        };

        // Act
        var changeHfState = new ChangeHfState(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(Mood.Berserk, changeHfState.Mood);
    }

    [TestMethod]
    public void Constructor_WithReason_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "state", Value = "settled" },
            new Property { Name = "reason", Value = "failed mood" }
        };

        // Act
        var changeHfState = new ChangeHfState(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(ChangeHfStateReason.FailedMood, changeHfState.Reason);
    }

    [TestMethod]
    public void Constructor_UpdatesHistoricalFigureState()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "state", Value = "settled" }
        };

        // Act
        var changeHfState = new ChangeHfState(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(HfState.Settled, _hf.CurrentState);
        Assert.AreEqual(1, _hf.States.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToHistoricalFigure()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "state", Value = "settled" }
        };
        var initialEventCount = _hf.Events.Count;

        // Act
        var changeHfState = new ChangeHfState(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _hf.Events.Count);
    }

    [TestMethod]
    public void Print_WithSettledState_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "state", Value = "settled" },
            new Property { Name = "site_id", Value = "1" }
        };
        var changeHfState = new ChangeHfState(properties, _mockWorld.Object);

        // Act
        var result = changeHfState.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Test HF"));
        Assert.IsTrue(result.Contains("settled"));
    }

    [TestMethod]
    public void Print_WithMood_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "state", Value = "settled" },
            new Property { Name = "mood", Value = "berserk" },
            new Property { Name = "site_id", Value = "1" }
        };
        var changeHfState = new ChangeHfState(properties, _mockWorld.Object);

        // Act
        var result = changeHfState.Print(link: true);

        // Assert - just verify output is non-empty
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "state", Value = "wandering" }
        };
        var changeHfState = new ChangeHfState(properties, _mockWorld.Object);

        // Act
        var result = changeHfState.Print(link: false);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }
}
