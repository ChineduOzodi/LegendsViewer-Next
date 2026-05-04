using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class CreatedWorldConstructionTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _civ = null!;
    private Site _site1 = null!;
    private Site _site2 = null!;

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

        _site1 = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Site 1",
            Type = "TOWER"
        };
        _site1.Structures = [];

        _site2 = new Site([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Site 2",
            Type = "TOWER"
        };
        _site2.Structures = [];

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_civ);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site1);
        _mockWorld.Setup(w => w.GetSite(2)).Returns(_site2);
    }

    [TestMethod]
    public void Constructor_WithBasicProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id1", Value = "1" },
            new Property { Name = "site_id2", Value = "2" }
        };

        // Act
        var createdWorldConstruction = new CreatedWorldConstruction(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(createdWorldConstruction);
        Assert.AreEqual(_civ, createdWorldConstruction.Civ);
    }

    [TestMethod]
    public void Constructor_WithWorldConstruction_ParsesCorrectly()
    {
        // Arrange
        var wc = new WorldConstruction([], _mockWorld.Object) { Id = 1, Name = "Road" };
        _mockWorld.Setup(w => w.GetWorldConstruction(1)).Returns(wc);

        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "wcid", Value = "1" }
        };

        // Act
        var createdWorldConstruction = new CreatedWorldConstruction(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(wc, createdWorldConstruction.WorldConstruction);
    }

    [TestMethod]
    public void Constructor_AddsEventToCiv()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" }
        };
        var initialEventCount = _civ.Events.Count;

        // Act
        var createdWorldConstruction = new CreatedWorldConstruction(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _civ.Events.Count);
    }

    [TestMethod]
    public void Print_WithAllProperties_ReturnsCorrectFormat()
    {
        // Arrange
        var wc = new WorldConstruction([], _mockWorld.Object) { Id = 1, Name = "Road" };
        _mockWorld.Setup(w => w.GetWorldConstruction(1)).Returns(wc);

        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id1", Value = "1" },
            new Property { Name = "site_id2", Value = "2" },
            new Property { Name = "wcid", Value = "1" }
        };
        var createdWorldConstruction = new CreatedWorldConstruction(properties, _mockWorld.Object);

        // Act
        var result = createdWorldConstruction.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("constructed"));
        Assert.IsTrue(result.Contains("connecting"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id1", Value = "1" },
            new Property { Name = "site_id2", Value = "2" }
        };
        var createdWorldConstruction = new CreatedWorldConstruction(properties, _mockWorld.Object);

        // Act
        var result = createdWorldConstruction.Print(link: false);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }
}
