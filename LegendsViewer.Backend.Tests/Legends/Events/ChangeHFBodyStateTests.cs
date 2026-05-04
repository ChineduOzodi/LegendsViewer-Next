using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class ChangeHFBodyStateTests
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
            new Property { Name = "body_state", Value = "entombed at site" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var changeHfBodyState = new ChangeHfBodyState(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(changeHfBodyState);
        Assert.AreEqual(_hf, changeHfBodyState.HistoricalFigure);
        Assert.AreEqual(BodyState.EntombedAtSite, changeHfBodyState.BodyState);
    }

    [TestMethod]
    public void Constructor_WithRegion_ParsesCorrectly()
    {
        // Arrange
        var region = new WorldRegion([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Region"
        };
        _mockWorld.Setup(w => w.GetRegion(1)).Returns(region);

        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "body_state", Value = "entombed at site" },
            new Property { Name = "subregion_id", Value = "1" }
        };

        // Act
        var changeHfBodyState = new ChangeHfBodyState(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(region, changeHfBodyState.Region);
    }

    [TestMethod]
    public void Constructor_AddsEventToHistoricalFigure()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "body_state", Value = "entombed at site" }
        };
        var initialEventCount = _hf.Events.Count;

        // Act
        var changeHfBodyState = new ChangeHfBodyState(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _hf.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToSite()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "body_state", Value = "entombed at site" },
            new Property { Name = "site_id", Value = "1" }
        };
        var initialEventCount = _site.Events.Count;

        // Act
        var changeHfBodyState = new ChangeHfBodyState(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _site.Events.Count);
    }

    [TestMethod]
    public void Print_WithAllProperties_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "body_state", Value = "entombed at site" },
            new Property { Name = "site_id", Value = "1" }
        };
        var changeHfBodyState = new ChangeHfBodyState(properties, _mockWorld.Object);

        // Act
        var result = changeHfBodyState.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Test HF"));
        Assert.IsTrue(result.Contains("entombed"));
        Assert.IsTrue(result.Contains("Test Site"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "body_state", Value = "entombed at site" },
            new Property { Name = "site_id", Value = "1" }
        };
        var changeHfBodyState = new ChangeHfBodyState(properties, _mockWorld.Object);

        // Act
        var result = changeHfBodyState.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("entombed"));
    }
}
