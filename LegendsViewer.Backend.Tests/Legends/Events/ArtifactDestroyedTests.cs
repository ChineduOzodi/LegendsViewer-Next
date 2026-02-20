using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class ArtifactDestroyedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Artifact _artifact = null!;
    private Site _site = null!;
    private HistoricalFigure _destroyer = null!;

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

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };
        _site.Structures = [];

        _destroyer = new HistoricalFigure
        {
            Id = 1,
            Name = "Test Destroyer",
            Icon = "person"
        };

        _mockWorld.Setup(w => w.GetArtifact(1)).Returns(_artifact);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_destroyer);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "destroyer_enid", Value = "1" }
        };

        // Act
        var artifactDestroyed = new ArtifactDestroyed(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(artifactDestroyed);
        Assert.AreEqual(_artifact, artifactDestroyed.Artifact);
        Assert.AreEqual(_site, artifactDestroyed.Site);
        Assert.AreEqual(_destroyer, artifactDestroyed.Destroyer);
    }

    [TestMethod]
    public void Constructor_WithOnlyArtifact_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" }
        };

        // Act
        var artifactDestroyed = new ArtifactDestroyed(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(artifactDestroyed);
        Assert.AreEqual(_artifact, artifactDestroyed.Artifact);
        Assert.IsNull(artifactDestroyed.Site);
        Assert.IsNull(artifactDestroyed.Destroyer);
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
        var artifactDestroyed = new ArtifactDestroyed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _artifact.Events.Count);
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
        var artifactDestroyed = new ArtifactDestroyed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _site.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToDestroyer()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "destroyer_enid", Value = "1" }
        };
        var initialEventCount = _destroyer.Events.Count;

        // Act
        var artifactDestroyed = new ArtifactDestroyed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _destroyer.Events.Count);
    }

    [TestMethod]
    public void Print_WithAllProperties_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "destroyer_enid", Value = "1" }
        };
        var artifactDestroyed = new ArtifactDestroyed(properties, _mockWorld.Object);

        // Act
        var result = artifactDestroyed.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Test Artifact"));
        Assert.IsTrue(result.Contains("was destroyed"));
        Assert.IsTrue(result.Contains("Test Destroyer"));
        Assert.IsTrue(result.Contains("by"));
        Assert.IsTrue(result.Contains("Test Site"));
        Assert.IsTrue(result.Contains("in"));
    }

    [TestMethod]
    public void Print_WithoutDestroyer_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var artifactDestroyed = new ArtifactDestroyed(properties, _mockWorld.Object);

        // Act
        var result = artifactDestroyed.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("was destroyed"));
        Assert.IsFalse(result.Contains("by"));
    }

    [TestMethod]
    public void Print_WithoutSite_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "destroyer_enid", Value = "1" }
        };
        var artifactDestroyed = new ArtifactDestroyed(properties, _mockWorld.Object);

        // Act
        var result = artifactDestroyed.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("was destroyed"));
        Assert.IsTrue(result.Contains("Test Destroyer"));
        Assert.IsTrue(result.Contains("by"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "destroyer_enid", Value = "1" }
        };
        var artifactDestroyed = new ArtifactDestroyed(properties, _mockWorld.Object);

        // Act
        var result = artifactDestroyed.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("was destroyed"));
    }
}
