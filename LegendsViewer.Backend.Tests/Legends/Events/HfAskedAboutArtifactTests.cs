using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfAskedAboutArtifactTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _historicalFigure = null!;
    private Artifact _artifact = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        // Create historical figure
        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Inquisitor",
            Icon = "person"
        };

        // Create artifact
        _artifact = new Artifact([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Crown of Ages"
        };

        // Create site
        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Royal Palace"
        };

        // Setup mock world
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetArtifact(1)).Returns(_artifact);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hist_fig_id", Value = "1" },
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var hfAskedAboutArtifact = new HfAskedAboutArtifact(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_historicalFigure, hfAskedAboutArtifact.HistoricalFigure);
        Assert.AreEqual(_artifact, hfAskedAboutArtifact.Artifact);
        Assert.AreEqual(_site, hfAskedAboutArtifact.Site);
    }

    [TestMethod]
    public void Print_WithLinkTrue_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hist_fig_id", Value = "1" },
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var hfAskedAboutArtifact = new HfAskedAboutArtifact(properties, _mockWorld.Object);

        // Act
        var result = hfAskedAboutArtifact.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Inquisitor"));
        Assert.IsTrue(result.Contains("asked about"));
        Assert.IsTrue(result.Contains("Crown of Ages"));
    }

    [TestMethod]
    public void Print_WithSite_IncludesSiteName()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hist_fig_id", Value = "1" },
            new Property { Name = "artifact_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var hfAskedAboutArtifact = new HfAskedAboutArtifact(properties, _mockWorld.Object);

        // Act
        var result = hfAskedAboutArtifact.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("Royal Palace"));
    }

    [TestMethod]
    public void Constructor_AddsEventToArtifact()
    {
        // Arrange
        var initialEventCount = _artifact.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "hist_fig_id", Value = "1" },
            new Property { Name = "artifact_id", Value = "1" }
        };

        // Act
        var hfAskedAboutArtifact = new HfAskedAboutArtifact(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _artifact.Events.Count);
    }

    [TestMethod]
    public void Constructor_WithNullArtifact_DoesNotThrow()
    {
        // Arrange
        _mockWorld.Setup(w => w.GetArtifact(It.IsAny<int>())).Returns((Artifact?)null);

        var properties = new List<Property>
        {
            new Property { Name = "hist_fig_id", Value = "1" },
            new Property { Name = "artifact_id", Value = "999" }
        };

        // Act & Assert
        var hfAskedAboutArtifact = new HfAskedAboutArtifact(properties, _mockWorld.Object);
        Assert.IsNull(hfAskedAboutArtifact.Artifact);
    }
}
