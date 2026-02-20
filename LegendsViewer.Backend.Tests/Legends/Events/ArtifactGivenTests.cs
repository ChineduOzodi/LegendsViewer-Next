using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class ArtifactGivenTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Artifact _artifact = null!;
    private HistoricalFigure _giver = null!;
    private HistoricalFigure _receiver = null!;
    private Entity _giverEntity = null!;
    private Entity _receiverEntity = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _artifact = new Artifact([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Artifact",
            Icon = "artifact"
        };

        _giver = new HistoricalFigure
        {
            Id = 1,
            Name = "Test Giver",
            Icon = "person"
        };

        _receiver = new HistoricalFigure
        {
            Id = 2,
            Name = "Test Receiver",
            Icon = "person"
        };

        _giverEntity = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Giver Entity",
            Icon = "civilization"
        };
        _giverEntity.Honors = [];

        _receiverEntity = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Receiver Entity",
            Icon = "civilization"
        };
        _receiverEntity.Honors = [];

        _mockWorld.Setup(w => w.GetArtifact(1)).Returns(_artifact);
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_giver);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_receiver);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_giverEntity);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_receiverEntity);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "giver_hist_figure_id", Value = "1" },
            new Property { Name = "receiver_hist_figure_id", Value = "2" }
        };

        // Act
        var artifactGiven = new ArtifactGiven(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(artifactGiven);
        Assert.AreEqual(_artifact, artifactGiven.Artifact);
        Assert.AreEqual(_giver, artifactGiven.HistoricalFigureGiver);
        Assert.AreEqual(_receiver, artifactGiven.HistoricalFigureReceiver);
    }

    [TestMethod]
    public void Constructor_WithEntities_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "giver_hist_figure_id", Value = "1" },
            new Property { Name = "receiver_hist_figure_id", Value = "2" },
            new Property { Name = "giver_entity_id", Value = "1" },
            new Property { Name = "receiver_entity_id", Value = "2" }
        };

        // Act
        var artifactGiven = new ArtifactGiven(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_giverEntity, artifactGiven.EntityGiver);
        Assert.AreEqual(_receiverEntity, artifactGiven.EntityReceiver);
    }

    [TestMethod]
    public void Constructor_WithCementBondsReason_ParsesReason()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "reason", Value = "cement bonds of friendship" }
        };

        // Act
        var artifactGiven = new ArtifactGiven(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(ArtifactReason.CementBondsOfFriendship, artifactGiven.ArtifactReason);
    }

    [TestMethod]
    public void Constructor_WithTradeNegotiationReason_ParsesReason()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "reason", Value = "part of trade negotiation" }
        };

        // Act
        var artifactGiven = new ArtifactGiven(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(ArtifactReason.PartOfTradeNegotiation, artifactGiven.ArtifactReason);
    }

    [TestMethod]
    public void Constructor_AddsEventToArtifact()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" }
        };
        var initialEventCount = _artifact.Events.Count;

        // Act
        var artifactGiven = new ArtifactGiven(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _artifact.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToGiver()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "giver_hist_figure_id", Value = "1" }
        };
        var initialEventCount = _giver.Events.Count;

        // Act
        var artifactGiven = new ArtifactGiven(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _giver.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToReceiver_WhenDifferentFromGiver()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "giver_hist_figure_id", Value = "1" },
            new Property { Name = "receiver_hist_figure_id", Value = "2" }
        };
        var initialEventCount = _receiver.Events.Count;

        // Act
        var artifactGiven = new ArtifactGiven(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _receiver.Events.Count);
    }

    [TestMethod]
    public void Constructor_DoesNotAddEventToReceiver_WhenSameAsGiver()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "giver_hist_figure_id", Value = "1" },
            new Property { Name = "receiver_hist_figure_id", Value = "1" }
        };
        var initialEventCount = _giver.Events.Count;

        // Act
        var artifactGiven = new ArtifactGiven(properties, _mockWorld.Object);

        // Assert - should only add once, not twice
        Assert.AreEqual(initialEventCount + 1, _giver.Events.Count);
    }

    [TestMethod]
    public void Print_WithAllProperties_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "giver_hist_figure_id", Value = "1" },
            new Property { Name = "receiver_hist_figure_id", Value = "2" }
        };
        var artifactGiven = new ArtifactGiven(properties, _mockWorld.Object);

        // Act
        var result = artifactGiven.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Test Artifact"));
        Assert.IsTrue(result.Contains("was offered to"));
        Assert.IsTrue(result.Contains("Test Receiver"));
        Assert.IsTrue(result.Contains("Test Giver"));
        Assert.IsTrue(result.Contains("by"));
    }

    [TestMethod]
    public void Print_WithEntities_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "giver_hist_figure_id", Value = "1" },
            new Property { Name = "receiver_hist_figure_id", Value = "2" },
            new Property { Name = "giver_entity_id", Value = "1" },
            new Property { Name = "receiver_entity_id", Value = "2" }
        };
        var artifactGiven = new ArtifactGiven(properties, _mockWorld.Object);

        // Act
        var result = artifactGiven.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("of"));
        Assert.IsTrue(result.Contains("Giver Entity"));
        Assert.IsTrue(result.Contains("Receiver Entity"));
    }

    [TestMethod]
    public void Print_WithCementBondsReason_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "reason", Value = "cement bonds of friendship" }
        };
        var artifactGiven = new ArtifactGiven(properties, _mockWorld.Object);

        // Act
        var result = artifactGiven.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("cement the bonds of friendship"));
    }

    [TestMethod]
    public void Print_WithTradeNegotiationReason_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "reason", Value = "part of trade negotiation" }
        };
        var artifactGiven = new ArtifactGiven(properties, _mockWorld.Object);

        // Act
        var result = artifactGiven.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("trade negotiation"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "giver_hist_figure_id", Value = "1" },
            new Property { Name = "receiver_hist_figure_id", Value = "2" }
        };
        var artifactGiven = new ArtifactGiven(properties, _mockWorld.Object);

        // Act
        var result = artifactGiven.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("was offered to"));
        Assert.IsTrue(result.Contains("Test Giver"));
        Assert.IsTrue(result.Contains("Test Receiver"));
    }
}
