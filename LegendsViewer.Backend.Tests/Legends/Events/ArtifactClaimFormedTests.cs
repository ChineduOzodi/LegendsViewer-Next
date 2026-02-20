using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class ArtifactClaimFormedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Artifact _artifact = null!;
    private HistoricalFigure _historicalFigure = null!;
    private Entity _entity = null!;

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

        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Test Figure",
            Icon = "person"
        };

        _entity = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Entity",
            Icon = "civilization"
        };
        _entity.EntityPositionAssignments = [];

        _mockWorld.Setup(w => w.GetArtifact(1)).Returns(_artifact);
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_entity);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "claim", Value = "symbol" }
        };

        // Act
        var artifactClaim = new ArtifactClaimFormed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_artifact, artifactClaim.Artifact);
        Assert.AreEqual(_historicalFigure, artifactClaim.HistoricalFigure);
        Assert.AreEqual(_entity, artifactClaim.Entity);
    }

    [TestMethod]
    public void Constructor_WithSymbolClaim_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "claim", Value = "symbol" }
        };

        // Act
        var artifactClaim = new ArtifactClaimFormed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(Claim.Symbol, artifactClaim.Claim);
    }

    [TestMethod]
    public void Constructor_WithHeirloomClaim_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "claim", Value = "heirloom" }
        };

        // Act
        var artifactClaim = new ArtifactClaimFormed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(Claim.Heirloom, artifactClaim.Claim);
    }

    [TestMethod]
    public void Constructor_WithTreasureClaim_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "claim", Value = "treasure" }
        };

        // Act
        var artifactClaim = new ArtifactClaimFormed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(Claim.Heirloom, artifactClaim.Claim);
    }

    [TestMethod]
    public void Constructor_WithPositionProfileId_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "position_profile_id", Value = "5" }
        };

        // Act
        var artifactClaim = new ArtifactClaimFormed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(5, artifactClaim.PositionProfileId);
    }

    [TestMethod]
    public void Constructor_WithCircumstance_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "circumstance", Value = "from afar" }
        };

        // Act
        var artifactClaim = new ArtifactClaimFormed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual("from afar", artifactClaim.Circumstance);
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
        var artifactClaim = new ArtifactClaimFormed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _artifact.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToHistoricalFigure()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hist_figure_id", Value = "1" }
        };
        var initialEventCount = _historicalFigure.Events.Count;

        // Act
        var artifactClaim = new ArtifactClaimFormed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _historicalFigure.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToEntity()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" }
        };
        var initialEventCount = _entity.Events.Count;

        // Act
        var artifactClaim = new ArtifactClaimFormed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _entity.Events.Count);
    }

    [TestMethod]
    public void Print_WithSymbolClaim_ReturnsMadeATitle()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "claim", Value = "symbol" },
            new Property { Name = "circumstance", Value = "" }
        };
        var artifactClaim = new ArtifactClaimFormed(properties, _mockWorld.Object);

        // Act
        var result = artifactClaim.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Test Artifact"));
        Assert.IsTrue(result.Contains("was made a"));
    }

    [TestMethod]
    public void Print_WithEntity_ReturnsByEntity()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "claim", Value = "symbol" }
        };
        var artifactClaim = new ArtifactClaimFormed(properties, _mockWorld.Object);

        // Act
        var result = artifactClaim.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("by"));
        Assert.IsTrue(result.Contains("Test Entity"));
    }

    [TestMethod]
    public void Print_WithHistoricalFigure_ReturnsByFigure()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "claim", Value = "symbol" }
        };
        var artifactClaim = new ArtifactClaimFormed(properties, _mockWorld.Object);

        // Act
        var result = artifactClaim.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("by"));
        Assert.IsTrue(result.Contains("Test Figure"));
    }

    [TestMethod]
    public void Print_WithCircumstance_IncludesCircumstance()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "circumstance", Value = "from afar" }
        };
        var artifactClaim = new ArtifactClaimFormed(properties, _mockWorld.Object);

        // Act
        var result = artifactClaim.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("from afar"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "claim", Value = "symbol" }
        };
        var artifactClaim = new ArtifactClaimFormed(properties, _mockWorld.Object);

        // Act
        var result = artifactClaim.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("Test Artifact"));
    }
}
