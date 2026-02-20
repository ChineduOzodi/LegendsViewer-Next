using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class ArtifactCopiedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Artifact _artifact = null!;
    private Site _sourceSite = null!;
    private Site _destSite = null!;
    private Entity _sourceEntity = null!;
    private Entity _destEntity = null!;

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

        _sourceEntity = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Source Entity",
            Icon = "civilization"
        };

        _destEntity = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Destination Entity",
            Icon = "civilization"
        };

        _sourceSite = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Source Site",
            Type = "TOWER"
        };
        _sourceSite.Structures = [];

        _destSite = new Site([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Destination Site",
            Type = "KEEP"
        };
        _destSite.Structures = [];

        _mockWorld.Setup(w => w.GetArtifact(1)).Returns(_artifact);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_sourceEntity);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_destEntity);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_sourceSite);
        _mockWorld.Setup(w => w.GetSite(2)).Returns(_destSite);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "dest_site_id", Value = "2" },
            new Property { Name = "dest_entity_id", Value = "2" },
            new Property { Name = "source_site_id", Value = "1" },
            new Property { Name = "source_entity_id", Value = "1" }
        };

        // Act
        var artifactCopied = new ArtifactCopied(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_artifact, artifactCopied.Artifact);
        Assert.AreEqual(_destSite, artifactCopied.DestSite);
        Assert.AreEqual(_destEntity, artifactCopied.DestEntity);
        Assert.AreEqual(_sourceSite, artifactCopied.SourceSite);
        Assert.AreEqual(_sourceEntity, artifactCopied.SourceEntity);
    }

    [TestMethod]
    public void Constructor_WithFromOriginal_SetsFlag()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "from_original", Value = "true" }
        };

        // Act
        var artifactCopied = new ArtifactCopied(properties, _mockWorld.Object);

        // Assert
        Assert.IsTrue(artifactCopied.FromOriginal);
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
        var artifactCopied = new ArtifactCopied(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _artifact.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToDestSite()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "dest_site_id", Value = "2" }
        };
        var initialEventCount = _destSite.Events.Count;

        // Act
        var artifactCopied = new ArtifactCopied(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _destSite.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToSourceEntity()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "source_entity_id", Value = "1" }
        };
        var initialEventCount = _sourceEntity.Events.Count;

        // Act
        var artifactCopied = new ArtifactCopied(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _sourceEntity.Events.Count);
    }

    [TestMethod]
    public void Print_WithAllProperties_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "dest_site_id", Value = "2" },
            new Property { Name = "dest_entity_id", Value = "2" },
            new Property { Name = "source_site_id", Value = "1" },
            new Property { Name = "source_entity_id", Value = "1" }
        };
        var artifactCopied = new ArtifactCopied(properties, _mockWorld.Object);

        // Act
        var result = artifactCopied.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Destination Entity"));
        Assert.IsTrue(result.Contains("made a copy of"));
        Assert.IsTrue(result.Contains("Test Artifact"));
    }

    [TestMethod]
    public void Print_WithFromOriginal_IncludesOriginalText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "dest_site_id", Value = "2" },
            new Property { Name = "dest_entity_id", Value = "2" },
            new Property { Name = "source_site_id", Value = "1" },
            new Property { Name = "source_entity_id", Value = "1" },
            new Property { Name = "from_original", Value = "true" }
        };
        var artifactCopied = new ArtifactCopied(properties, _mockWorld.Object);

        // Act
        var result = artifactCopied.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("the original"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "dest_site_id", Value = "2" },
            new Property { Name = "dest_entity_id", Value = "2" },
            new Property { Name = "source_site_id", Value = "1" },
            new Property { Name = "source_entity_id", Value = "1" }
        };
        var artifactCopied = new ArtifactCopied(properties, _mockWorld.Object);

        // Act
        var result = artifactCopied.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("made a copy of"));
    }
}
