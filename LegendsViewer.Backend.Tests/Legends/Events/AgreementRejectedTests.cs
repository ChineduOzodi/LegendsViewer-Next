using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class AgreementRejectedTests
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
            Type = "TOWER"
        };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_source);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_destination);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "source", Value = "1" },
            new Property { Name = "destination", Value = "2" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "topic", Value = "treequota" }
        };

        // Act
        var agreement = new AgreementRejected(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_source, agreement.Source);
        Assert.AreEqual(_destination, agreement.Destination);
        Assert.AreEqual(_site, agreement.Site);
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
        var agreement = new AgreementRejected(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _site.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToSource()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "source", Value = "1" }
        };
        var initialEventCount = _source.Events.Count;

        // Act
        var agreement = new AgreementRejected(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _source.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToDestination()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "destination", Value = "2" }
        };
        var initialEventCount = _destination.Events.Count;

        // Act
        var agreement = new AgreementRejected(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _destination.Events.Count);
    }

    [TestMethod]
    public void Print_TreeQuota_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "source", Value = "1" },
            new Property { Name = "destination", Value = "2" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "topic", Value = "treequota" }
        };
        var agreement = new AgreementRejected(properties, _mockWorld.Object);

        // Act
        var result = agreement.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("lumber agreement"));
        Assert.IsTrue(result.Contains("Source Kingdom"));
        Assert.IsTrue(result.Contains("Destination Kingdom"));
        Assert.IsTrue(result.Contains("proposed by"));
        Assert.IsTrue(result.Contains("was rejected"));
    }

    [TestMethod]
    public void Print_BecomeLandHolder_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "source", Value = "1" },
            new Property { Name = "destination", Value = "2" },
            new Property { Name = "topic", Value = "becomelandholder" }
        };
        var agreement = new AgreementRejected(properties, _mockWorld.Object);

        // Act
        var result = agreement.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("establishment of landed nobility"));
    }

    [TestMethod]
    public void Print_PromoteLandHolder_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "source", Value = "1" },
            new Property { Name = "destination", Value = "2" },
            new Property { Name = "topic", Value = "promotelandholder" }
        };
        var agreement = new AgreementRejected(properties, _mockWorld.Object);

        // Act
        var result = agreement.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("elevation of the landed nobility"));
    }

    [TestMethod]
    public void Print_Tribute_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "source", Value = "1" },
            new Property { Name = "destination", Value = "2" },
            new Property { Name = "topic", Value = "tributeagreement" }
        };
        var agreement = new AgreementRejected(properties, _mockWorld.Object);

        // Act
        var result = agreement.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("tribute agreement"));
    }

    [TestMethod]
    public void Print_WithNullSource_ReturnsUnknownEntity()
    {
        // Arrange
        _mockWorld.Setup(w => w.GetEntity(1)).Returns((Entity?)null);

        var properties = new List<Property>
        {
            new Property { Name = "source", Value = "1" },
            new Property { Name = "destination", Value = "2" },
            new Property { Name = "topic", Value = "treequota" }
        };
        var agreement = new AgreementRejected(properties, _mockWorld.Object);

        // Act
        var result = agreement.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("UNKNOWN ENTITY"));
    }

    [TestMethod]
    public void Print_WithNullSite_ReturnsUnknownSite()
    {
        // Arrange
        _mockWorld.Setup(w => w.GetSite(1)).Returns((Site?)null);

        var properties = new List<Property>
        {
            new Property { Name = "source", Value = "1" },
            new Property { Name = "destination", Value = "2" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "topic", Value = "treequota" }
        };
        var agreement = new AgreementRejected(properties, _mockWorld.Object);

        // Act
        var result = agreement.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("UNKNOWN SITE"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "source", Value = "1" },
            new Property { Name = "destination", Value = "2" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "topic", Value = "treequota" }
        };
        var agreement = new AgreementRejected(properties, _mockWorld.Object);

        // Act
        var result = agreement.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("lumber agreement"));
        Assert.IsTrue(result.Contains("was rejected"));
    }

    [TestMethod]
    public void Print_UnknownTopic_ReturnsUnknownFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "source", Value = "1" },
            new Property { Name = "destination", Value = "2" },
            new Property { Name = "topic", Value = "unknown_topic" }
        };
        var agreement = new AgreementRejected(properties, _mockWorld.Object);

        // Act
        var result = agreement.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("UNKNOWN AGREEMENT"));
    }
}
