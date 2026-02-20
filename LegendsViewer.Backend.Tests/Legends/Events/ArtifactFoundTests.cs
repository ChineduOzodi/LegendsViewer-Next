using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class ArtifactFoundTests
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
            Name = "Test Finder",
            Icon = "person"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };
        _site.Structures = [];
        _site.SiteProperties = [];

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
        var artifactFound = new ArtifactFound(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(artifactFound);
        Assert.AreEqual(_artifact, artifactFound.Artifact);
        Assert.AreEqual(_historicalFigure, artifactFound.HistoricalFigure);
        Assert.AreEqual(_site, artifactFound.Site);
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
        var artifactFound = new ArtifactFound(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(42, artifactFound.UnitId);
    }

    [TestMethod]
    public void Constructor_WithStructureId_SetsStructureId()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure_id", Value = "42" }
        };

        // Act
        var artifactFound = new ArtifactFound(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(42, artifactFound.StructureId);
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
        var artifactFound = new ArtifactFound(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(region, artifactFound.Region);
    }

    [TestMethod]
    public void Constructor_WithUndergroundRegion_ParsesCorrectly()
    {
        // Arrange
        var undergroundRegion = new UndergroundRegion([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Underground Region"
        };
        _mockWorld.Setup(w => w.GetUndergroundRegion(1)).Returns(undergroundRegion);

        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "feature_layer_id", Value = "1" }
        };

        // Act
        var artifactFound = new ArtifactFound(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(undergroundRegion, artifactFound.UndergroundRegion);
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
        var artifactFound = new ArtifactFound(properties, _mockWorld.Object);

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
        var artifactFound = new ArtifactFound(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _historicalFigure.Events.Count);
    }

    [TestMethod]
    public void Print_WithAllProperties_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var artifactFound = new ArtifactFound(properties, _mockWorld.Object);

        // Act
        var result = artifactFound.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Test Artifact"));
        Assert.IsTrue(result.Contains("was found"));
        Assert.IsTrue(result.Contains("Test Finder"));
        Assert.IsTrue(result.Contains("by"));
        Assert.IsTrue(result.Contains("Test Site"));
        Assert.IsTrue(result.Contains("in"));
    }

    [TestMethod]
    public void Print_WithoutHistoricalFigure_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" }
        };
        var artifactFound = new ArtifactFound(properties, _mockWorld.Object);

        // Act
        var result = artifactFound.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("was found"));
        Assert.IsFalse(result.Contains("by"));
    }

    [TestMethod]
    public void Print_WithRegion_ReturnsCorrectFormat()
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
        var artifactFound = new ArtifactFound(properties, _mockWorld.Object);

        // Act
        var result = artifactFound.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("was found"));
        Assert.IsTrue(result.Contains("Test Region"));
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
        var artifactFound = new ArtifactFound(properties, _mockWorld.Object);

        // Act
        var result = artifactFound.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("was found"));
    }
}
