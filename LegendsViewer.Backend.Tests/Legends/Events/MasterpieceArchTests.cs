using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class MasterpieceArchTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _historicalFigure = null!;
    private Entity _entity = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Fortress",
            Icon = "fortress"
        };

        _entity = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Guild",
            Icon = "group"
        };

        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Master Builder",
            Icon = "person"
        };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_entity);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "building_type", Value = "tower" },
            new Property { Name = "building_subtype", Value = "workshop" },
            new Property { Name = "skill_at_time", Value = "Architecture" },
            new Property { Name = "construction", Value = "constructed" }
        };

        // Act
        var masterpiece = new MasterpieceArch(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_historicalFigure, masterpiece.Maker);
        Assert.AreEqual(_entity, masterpiece.MakerEntity);
        Assert.AreEqual(_site, masterpiece.Site);
        Assert.AreEqual("tower", masterpiece.BuildingType);
        Assert.AreEqual("workshop", masterpiece.BuildingSubType);
    }

    [TestMethod]
    public void Constructor_WithMissingSubtype_UsesType()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "building_type", Value = "tower" },
            new Property { Name = "building_subtype", Value = "-1" },
            new Property { Name = "construction", Value = "constructed" }
        };

        // Act
        var masterpiece = new MasterpieceArch(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual("tower", masterpiece.BuildingType);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "building_type", Value = "tower" },
            new Property { Name = "construction", Value = "constructed" }
        };
        var masterpiece = new MasterpieceArch(properties, _mockWorld.Object);

        // Act
        var result = masterpiece.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Master Builder"));
        Assert.IsTrue(result.Contains("masterful tower"));
        Assert.IsTrue(result.Contains("Test Guild"));
        Assert.IsTrue(result.Contains("Test Fortress"));
    }

    [TestMethod]
    public void Print_WithSubtype_UsesSubtype()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "building_type", Value = "tower" },
            new Property { Name = "building_subtype", Value = "workshop" },
            new Property { Name = "construction", Value = "constructed" }
        };
        var masterpiece = new MasterpieceArch(properties, _mockWorld.Object);

        // Act
        var result = masterpiece.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("masterful workshop"));
    }
}
