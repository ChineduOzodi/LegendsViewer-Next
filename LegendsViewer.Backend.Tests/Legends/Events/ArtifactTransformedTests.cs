using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class ArtifactTransformedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Artifact _newArtifact = null!;
    private Artifact _oldArtifact = null!;
    private HistoricalFigure _historicalFigure = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _newArtifact = new Artifact([], _mockWorld.Object)
        {
            Id = 1,
            Name = "New Artifact",
            Icon = "artifact",
            Material = "Steel",
            Subtype = "Weapon"
        };

        _oldArtifact = new Artifact([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Old Artifact",
            Icon = "artifact",
            Material = "Iron",
            Type = "Weapon"
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

        _mockWorld.Setup(w => w.GetArtifact(1)).Returns(_newArtifact);
        _mockWorld.Setup(w => w.GetArtifact(2)).Returns(_oldArtifact);
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "new_artifact_id", Value = "1" },
            new Property { Name = "old_artifact_id", Value = "2" },
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var artifactTransformed = new ArtifactTransformed(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(artifactTransformed);
        Assert.AreEqual(_newArtifact, artifactTransformed.NewArtifact);
        Assert.AreEqual(_oldArtifact, artifactTransformed.OldArtifact);
        Assert.AreEqual(_historicalFigure, artifactTransformed.HistoricalFigure);
        Assert.AreEqual(_site, artifactTransformed.Site);
    }

    [TestMethod]
    public void Constructor_WithUnitId_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "new_artifact_id", Value = "1" },
            new Property { Name = "old_artifact_id", Value = "2" },
            new Property { Name = "unit_id", Value = "42" }
        };

        // Act
        var artifactTransformed = new ArtifactTransformed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(42, artifactTransformed.UnitId);
    }

    [TestMethod]
    public void Constructor_AddsEventToNewArtifact()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "new_artifact_id", Value = "1" }
        };
        var initialEventCount = _newArtifact.Events.Count;

        // Act
        var artifactTransformed = new ArtifactTransformed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _newArtifact.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToOldArtifact()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "new_artifact_id", Value = "1" },
            new Property { Name = "old_artifact_id", Value = "2" }
        };
        var initialEventCount = _oldArtifact.Events.Count;

        // Act
        var artifactTransformed = new ArtifactTransformed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _oldArtifact.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToHistoricalFigure()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "new_artifact_id", Value = "1" },
            new Property { Name = "hist_figure_id", Value = "1" }
        };
        var initialEventCount = _historicalFigure.Events.Count;

        // Act
        var artifactTransformed = new ArtifactTransformed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _historicalFigure.Events.Count);
    }

    [TestMethod]
    public void Print_WithAllProperties_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "new_artifact_id", Value = "1" },
            new Property { Name = "old_artifact_id", Value = "2" },
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var artifactTransformed = new ArtifactTransformed(properties, _mockWorld.Object);

        // Act
        var result = artifactTransformed.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("New Artifact"));
        Assert.IsTrue(result.Contains("was made from"));
        Assert.IsTrue(result.Contains("Old Artifact"));
        Assert.IsTrue(result.Contains("Test Figure"));
        Assert.IsTrue(result.Contains("Test Site"));
    }

    [TestMethod]
    public void Print_WithoutHistoricalFigure_ReturnsUnknownFigure()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "new_artifact_id", Value = "1" },
            new Property { Name = "old_artifact_id", Value = "2" }
        };
        var artifactTransformed = new ArtifactTransformed(properties, _mockWorld.Object);

        // Act
        var result = artifactTransformed.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("UNKNOWN HISTORICAL FIGURE"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "new_artifact_id", Value = "1" },
            new Property { Name = "old_artifact_id", Value = "2" },
            new Property { Name = "hist_figure_id", Value = "1" }
        };
        var artifactTransformed = new ArtifactTransformed(properties, _mockWorld.Object);

        // Act
        var result = artifactTransformed.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("was made from"));
        Assert.IsTrue(result.Contains("Test Figure"));
    }
}
