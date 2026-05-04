using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class ArtifactPossessedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Artifact _artifact = null!;
    private HistoricalFigure _historicalFigure = null!;
    private Site _site = null!;

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

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };
        _site.Structures = [];

        _mockWorld.Setup(w => w.GetArtifact(1)).Returns(_artifact);
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var artifactPossessed = new ArtifactPossessed(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(artifactPossessed);
        Assert.AreEqual(_artifact, artifactPossessed.Artifact);
        Assert.AreEqual(_historicalFigure, artifactPossessed.HistoricalFigure);
        Assert.AreEqual(_site, artifactPossessed.Site);
    }

    [TestMethod]
    public void Constructor_WithUnitId_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "unit_id", Value = "42" }
        };

        // Act
        var artifactPossessed = new ArtifactPossessed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(42, artifactPossessed.UnitId);
    }

    [TestMethod]
    public void Constructor_WithHeirloomReason_SetsFamilyFigure()
    {
        // Arrange
        var familyFigure = new HistoricalFigure
        {
            Id = 2,
            Name = "Family Figure",
            Icon = "person"
        };
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(familyFigure);

        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "reason", Value = "artifact is heirloom of family hfid" },
            new Property { Name = "reason_id", Value = "2" }
        };

        // Act
        var artifactPossessed = new ArtifactPossessed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(ArtifactReason.ArtifactIsHeirloomOfFamilyHfid, artifactPossessed.ArtifactReason);
        Assert.AreEqual(familyFigure, artifactPossessed.FamilyFigure);
    }

    [TestMethod]
    public void Constructor_WithSymbolReason_SetsSymbolEntity()
    {
        // Arrange
        var symbolEntity = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Symbol Entity",
            Icon = "civilization"
        };
        symbolEntity.Honors = [];
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(symbolEntity);

        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "reason", Value = "artifact is symbol of entity position" },
            new Property { Name = "reason_id", Value = "2" }
        };

        // Act
        var artifactPossessed = new ArtifactPossessed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(ArtifactReason.ArtifactIsSymbolOfEntityPosition, artifactPossessed.ArtifactReason);
        Assert.AreEqual(symbolEntity, artifactPossessed.SymbolEntity);
    }

    [TestMethod]
    public void Constructor_WithRegion_ParsesRegion()
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
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "subregion_id", Value = "1" }
        };

        // Act
        var artifactPossessed = new ArtifactPossessed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(region, artifactPossessed.Region);
    }

    [TestMethod]
    public void Constructor_WithUndergroundRegion_ParsesCorrectly()
    {
        // Arrange
        var undergroundRegion = new UndergroundRegion([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Underground"
        };
        _mockWorld.Setup(w => w.GetUndergroundRegion(1)).Returns(undergroundRegion);

        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "feature_layer_id", Value = "1" }
        };

        // Act
        var artifactPossessed = new ArtifactPossessed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(undergroundRegion, artifactPossessed.UndergroundRegion);
    }

    [TestMethod]
    public void Constructor_WithCircumstanceHfIsDead_SetsFormerHolder()
    {
        // Arrange
        var formerHolder = new HistoricalFigure
        {
            Id = 3,
            Name = "Former Holder",
            Icon = "person"
        };
        _mockWorld.Setup(w => w.GetHistoricalFigure(3)).Returns(formerHolder);

        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "circumstance", Value = "hf is dead" },
            new Property { Name = "circumstance_id", Value = "3" }
        };

        // Act
        var artifactPossessed = new ArtifactPossessed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(Circumstance.HfIsDead, artifactPossessed.Circumstance);
        Assert.AreEqual(formerHolder, artifactPossessed.FormerHolder);
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
        var artifactPossessed = new ArtifactPossessed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _artifact.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToHistoricalFigure()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "hist_figure_id", Value = "1" }
        };
        var initialEventCount = _historicalFigure.Events.Count;

        // Act
        var artifactPossessed = new ArtifactPossessed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _historicalFigure.Events.Count);
    }

    [TestMethod]
    public void Print_WithSite_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var artifactPossessed = new ArtifactPossessed(properties, _mockWorld.Object);

        // Act
        var result = artifactPossessed.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Test Artifact"));
        Assert.IsTrue(result.Contains("Test Figure"));
        Assert.IsTrue(result.Contains("Test Site"));
        Assert.IsTrue(result.Contains("was claimed"));
        Assert.IsTrue(result.Contains("by"));
    }

    [TestMethod]
    public void Print_WithHeirloomReason_ReturnsCorrectFormat()
    {
        // Arrange
        var familyFigure = new HistoricalFigure
        {
            Id = 2,
            Name = "Family Figure",
            Icon = "person"
        };
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(familyFigure);

        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "reason", Value = "artifact is heirloom of family hfid" },
            new Property { Name = "reason_id", Value = "2" }
        };
        var artifactPossessed = new ArtifactPossessed(properties, _mockWorld.Object);

        // Act
        var result = artifactPossessed.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("acquired"));
        Assert.IsTrue(result.Contains("heirloom"));
        Assert.IsTrue(result.Contains("Family Figure"));
    }

    [TestMethod]
    public void Print_WithSymbolReason_ReturnsCorrectFormat()
    {
        // Arrange
        var symbolEntity = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Symbol Entity",
            Icon = "civilization"
        };
        symbolEntity.Honors = [];
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(symbolEntity);

        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "reason", Value = "artifact is symbol of entity position" },
            new Property { Name = "reason_id", Value = "2" }
        };
        var artifactPossessed = new ArtifactPossessed(properties, _mockWorld.Object);

        // Act
        var result = artifactPossessed.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("acquired"));
        Assert.IsTrue(result.Contains("symbol of authority"));
        Assert.IsTrue(result.Contains("Symbol Entity"));
    }

    [TestMethod]
    public void Print_WithCircumstanceHfIsDead_ReturnsCorrectFormat()
    {
        // Arrange
        var formerHolder = new HistoricalFigure
        {
            Id = 3,
            Name = "Former Holder",
            Icon = "person"
        };
        _mockWorld.Setup(w => w.GetHistoricalFigure(3)).Returns(formerHolder);

        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "circumstance", Value = "hf is dead" },
            new Property { Name = "circumstance_id", Value = "3" }
        };
        var artifactPossessed = new ArtifactPossessed(properties, _mockWorld.Object);

        // Act
        var result = artifactPossessed.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("after the death of"));
        Assert.IsTrue(result.Contains("Former Holder"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var artifactPossessed = new ArtifactPossessed(properties, _mockWorld.Object);

        // Act
        var result = artifactPossessed.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("was claimed"));
        Assert.IsTrue(result.Contains("Test Figure"));
    }
}
