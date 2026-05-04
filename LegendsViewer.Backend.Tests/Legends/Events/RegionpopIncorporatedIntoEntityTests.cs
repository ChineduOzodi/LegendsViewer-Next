using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class RegionpopIncorporatedIntoEntityTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _joinEntity = null!;
    private Site _site = null!;
    private WorldRegion _popSourceRegion = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _joinEntity = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Joining Kingdom",
            Icon = "civilization"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "CITY"
        };

        _popSourceRegion = new WorldRegion([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Source Region"
        };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_joinEntity);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
        _mockWorld.Setup(w => w.GetRegion(1)).Returns(_popSourceRegion);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "join_entity_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "pop_race", Value = "DWARF" },
            new Property { Name = "pop_number_moved", Value = "50" },
            new Property { Name = "pop_srid", Value = "1" }
        };

        // Act
        var evt = new RegionpopIncorporatedIntoEntity(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_joinEntity, evt.JoinEntity);
        Assert.AreEqual(_site, evt.Site);
        Assert.AreEqual("DWARF", evt.PopRace);
        Assert.AreEqual(50, evt.PopNumberMoved);
    }

    [TestMethod]
    public void Print_WithLargePop_ReturnsHundredsString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "join_entity_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "pop_number_moved", Value = "250" },
            new Property { Name = "pop_srid", Value = "1" }
        };

        // Act
        var evt = new RegionpopIncorporatedIntoEntity(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("hundreds of"));
        Assert.IsTrue(result.Contains("joined with"));
        Assert.IsTrue(result.Contains("Test Site"));
    }

    [TestMethod]
    public void Print_WithMediumPop_ReturnsDozensString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "join_entity_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "pop_number_moved", Value = "50" },
            new Property { Name = "pop_srid", Value = "1" }
        };

        // Act
        var evt = new RegionpopIncorporatedIntoEntity(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("dozens of"));
    }

    [TestMethod]
    public void Print_WithSmallPop_ReturnsSeveralString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "join_entity_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "pop_number_moved", Value = "10" },
            new Property { Name = "pop_srid", Value = "1" }
        };

        // Act
        var evt = new RegionpopIncorporatedIntoEntity(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("several"));
    }
}
