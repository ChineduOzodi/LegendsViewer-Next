using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class PeaceEffortsTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _source = null!;
    private Entity _destination = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _source = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Source Kingdom",
            Icon = "civilization"
        };

        _destination = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Destination Kingdom",
            Icon = "civilization"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "CITY"
        };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_source);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_destination);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void PeaceEfforts_Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "source", Value = "1" },
            new Property { Name = "destination", Value = "2" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "topic", Value = "war" }
        };

        // Act
        var evt = new PeaceEfforts(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_source, evt.Source);
        Assert.AreEqual(_destination, evt.Destination);
        Assert.AreEqual(_site, evt.Site);
        Assert.AreEqual("war", evt.Topic);
    }

    [TestMethod]
    public void PeaceAccepted_Constructor_SetsDecisionToAccepted()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "source", Value = "1" },
            new Property { Name = "destination", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var evt = new PeaceAccepted(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual("accepted", evt.Decision);
    }

    [TestMethod]
    public void PeaceRejected_Constructor_SetsDecisionToRejected()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "source", Value = "1" },
            new Property { Name = "destination", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var evt = new PeaceRejected(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual("rejected", evt.Decision);
    }

    [TestMethod]
    public void Print_WithSourceAndDestination_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "source", Value = "1" },
            new Property { Name = "destination", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var evt = new PeaceAccepted(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Destination Kingdom"));
        Assert.IsTrue(result.Contains("accepted"));
        Assert.IsTrue(result.Contains("offer of peace"));
        Assert.IsTrue(result.Contains("Source Kingdom"));
    }

    [TestMethod]
    public void Print_WithoutSourceAndDestination_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var evt = new PeaceAccepted(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Peace"));
        Assert.IsTrue(result.Contains("accepted"));
    }
}
