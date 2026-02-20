using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class CreatedSiteTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _civ = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _civ = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Civ",
            Icon = "civilization"
        };
        _civ.Honors = [];

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };
        _site.Structures = [];
        _site.OwnerHistory = [];

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_civ);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithBasicProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var createdSite = new CreatedSite(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(createdSite);
        Assert.AreEqual(_civ, createdSite.Civ);
        Assert.AreEqual(_site, createdSite.Site);
    }

    [TestMethod]
    public void Constructor_WithBuilder_ParsesCorrectly()
    {
        // Arrange
        var builder = new HistoricalFigure { Id = 1, Name = "Builder", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(builder);

        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "builder_hfid", Value = "1" }
        };

        // Act
        var createdSite = new CreatedSite(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(builder, createdSite.Builder);
    }

    [TestMethod]
    public void Constructor_WithSiteEntity_ParsesCorrectly()
    {
        // Arrange
        var siteEntity = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Site Entity",
            Icon = "civilization"
        };
        siteEntity.Honors = [];
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(siteEntity);

        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_civ_id", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var createdSite = new CreatedSite(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(siteEntity, createdSite.SiteEntity);
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
        var createdSite = new CreatedSite(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _site.Events.Count);
    }

    [TestMethod]
    public void Print_WithBuilder_ReturnsCorrectFormat()
    {
        // Arrange
        var builder = new HistoricalFigure { Id = 1, Name = "Builder", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(builder);

        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "builder_hfid", Value = "1" }
        };
        var createdSite = new CreatedSite(properties, _mockWorld.Object);

        // Act
        var result = createdSite.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Builder"));
        Assert.IsTrue(result.Contains("constructed"));
    }

    [TestMethod]
    public void Print_WithoutBuilder_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var createdSite = new CreatedSite(properties, _mockWorld.Object);

        // Act
        var result = createdSite.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("founded"));
        Assert.IsTrue(result.Contains("Test Civ"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var createdSite = new CreatedSite(properties, _mockWorld.Object);

        // Act
        var result = createdSite.Print(link: false);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }
}
