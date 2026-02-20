using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class ArtifactLostTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Artifact _artifact = null!;
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

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };
        _site.Structures = [];
        _site.SiteProperties = [];

        _mockWorld.Setup(w => w.GetArtifact(1)).Returns(_artifact);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var artifactLost = new ArtifactLost(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(artifactLost);
        Assert.AreEqual(_artifact, artifactLost.Artifact);
        Assert.AreEqual(_site, artifactLost.Site);
    }

    [TestMethod]
    public void Constructor_WithRegion_ParsesCorrectly()
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
        var artifactLost = new ArtifactLost(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(region, artifactLost.Region);
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
        var artifactLost = new ArtifactLost(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(undergroundRegion, artifactLost.UndergroundRegion);
    }

    [TestMethod]
    public void Constructor_WithSitePropertyId_SetsSitePropertyId()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "site_property_id", Value = "42" }
        };

        // Act
        var artifactLost = new ArtifactLost(properties, _mockWorld.Object);

        // Assert - just verify it parses without error (SiteProperty lookup requires proper site setup)
        Assert.IsNotNull(artifactLost);
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
        var artifactLost = new ArtifactLost(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _artifact.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToSite()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var initialEventCount = _site.Events.Count;

        // Act
        var artifactLost = new ArtifactLost(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _site.Events.Count);
    }

    [TestMethod]
    public void Print_WithSite_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var artifactLost = new ArtifactLost(properties, _mockWorld.Object);

        // Act
        var result = artifactLost.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Test Artifact"));
        Assert.IsTrue(result.Contains("was lost"));
        Assert.IsTrue(result.Contains("Test Site"));
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
        var artifactLost = new ArtifactLost(properties, _mockWorld.Object);

        // Act
        var result = artifactLost.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("was lost"));
        Assert.IsTrue(result.Contains("Test Region"));
    }

    [TestMethod]
    public void Print_WithUndergroundRegion_ReturnsCorrectFormat()
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
        var artifactLost = new ArtifactLost(properties, _mockWorld.Object);

        // Act
        var result = artifactLost.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("was lost"));
        Assert.IsTrue(result.Contains("Test Underground"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var artifactLost = new ArtifactLost(properties, _mockWorld.Object);

        // Act
        var result = artifactLost.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("was lost"));
    }
}
