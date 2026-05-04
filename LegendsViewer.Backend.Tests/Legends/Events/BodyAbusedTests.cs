using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class BodyAbusedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _body = null!;
    private Entity _abuser = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _body = new HistoricalFigure
        {
            Id = 1,
            Name = "Test Body",
            Icon = "person"
        };

        _abuser = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Abuser Entity",
            Icon = "civilization"
        };
        _abuser.Honors = [];

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };
        _site.Structures = [];

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_body);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_abuser);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithBasicProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "abuse_type", Value = "impaled" },
            new Property { Name = "item_type", Value = "WEAPON" },
            new Property { Name = "bodies", Value = "1" },
            new Property { Name = "civ", Value = "1" }
        };

        // Act
        var bodyAbused = new BodyAbused(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(bodyAbused);
        Assert.AreEqual(AbuseType.Impaled, bodyAbused.AbuseType);
        Assert.AreEqual("WEAPON", bodyAbused.ItemType);
    }

    [TestMethod]
    public void Constructor_WithMultipleBodies_ParsesCorrectly()
    {
        // Arrange
        var body2 = new HistoricalFigure { Id = 2, Name = "Body 2", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(body2);

        var properties = new List<Property>
        {
            new Property { Name = "abuse_type", Value = "piled" },
            new Property { Name = "bodies", Value = "1" },
            new Property { Name = "bodies", Value = "2" }
        };

        // Act
        var bodyAbused = new BodyAbused(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(2, bodyAbused.Bodies.Count);
    }

    [TestMethod]
    public void Constructor_WithSite_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "abuse_type", Value = "impaled" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var bodyAbused = new BodyAbused(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_site, bodyAbused.Site);
    }

    [TestMethod]
    public void Constructor_AddsEventToBody()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "bodies", Value = "1" }
        };
        var initialEventCount = _body.Events.Count;

        // Act
        var bodyAbused = new BodyAbused(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _body.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToAbuser()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ", Value = "1" }
        };
        var initialEventCount = _abuser.Events.Count;

        // Act
        var bodyAbused = new BodyAbused(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _abuser.Events.Count);
    }

    [TestMethod]
    public void Print_WithImpaledAbuse_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "abuse_type", Value = "impaled" },
            new Property { Name = "item_type", Value = "WEAPON" },
            new Property { Name = "bodies", Value = "1" }
        };
        var bodyAbused = new BodyAbused(properties, _mockWorld.Object);

        // Act
        var result = bodyAbused.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("the body of"));
        Assert.IsTrue(result.Contains("Test Body"));
        Assert.IsTrue(result.Contains("impaled"));
        Assert.IsTrue(result.Contains("WEAPON"));
    }

    [TestMethod]
    public void Print_WithMultipleBodies_ReturnsCorrectFormat()
    {
        // Arrange
        var body2 = new HistoricalFigure { Id = 2, Name = "Body 2", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(body2);

        var properties = new List<Property>
        {
            new Property { Name = "abuse_type", Value = "piled" },
            new Property { Name = "bodies", Value = "1" },
            new Property { Name = "bodies", Value = "2" }
        };
        var bodyAbused = new BodyAbused(properties, _mockWorld.Object);

        // Act
        var result = bodyAbused.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("the bodies of"));
        Assert.IsTrue(result.Contains("and"));
    }

    [TestMethod]
    public void Print_WithAbuser_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "abuse_type", Value = "flayed" },
            new Property { Name = "bodies", Value = "1" },
            new Property { Name = "civ", Value = "1" }
        };
        var bodyAbused = new BodyAbused(properties, _mockWorld.Object);

        // Act
        var result = bodyAbused.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("flayed"));
        Assert.IsTrue(result.Contains("by"));
        Assert.IsTrue(result.Contains("Abuser Entity"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "abuse_type", Value = "mutilated" },
            new Property { Name = "bodies", Value = "1" }
        };
        var bodyAbused = new BodyAbused(properties, _mockWorld.Object);

        // Act
        var result = bodyAbused.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("mutilated"));
    }
}
