using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class AddHfEntityHonorTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _entity = null!;
    private HistoricalFigure _historicalFigure = null!;
    private Honor _honor = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        // Create test entity with empty properties
        _entity = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Entity",
            Icon = "test"
        };
        _entity.Honors = [];

        // Create test historical figure
        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Test Historical Figure",
            Icon = "person"
        };

        // Create test honor
        _honor = new Honor([], _mockWorld.Object, _entity)
        {
            Id = 42,
            Name = "Knight of the Deep"
        };
        _entity.Honors.Add(_honor);

        // Setup mock world to return the entity and hf
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_entity);
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "honor_id", Value = "42" }
        };

        // Act
        var addHfEntityHonor = new AddHfEntityHonor(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_entity, addHfEntityHonor.Entity);
        Assert.AreEqual(_historicalFigure, addHfEntityHonor.HistoricalFigure);
        Assert.AreEqual(42, addHfEntityHonor.HonorId);
        Assert.AreEqual(_honor, addHfEntityHonor.Honor);
    }

    [TestMethod]
    public void Constructor_WithInvalidHonorId_SetsHonorToNull()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "honor_id", Value = "999" } // Non-existent honor
        };

        // Act
        var addHfEntityHonor = new AddHfEntityHonor(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_entity, addHfEntityHonor.Entity);
        Assert.AreEqual(_historicalFigure, addHfEntityHonor.HistoricalFigure);
        Assert.AreEqual(999, addHfEntityHonor.HonorId);
        Assert.IsNull(addHfEntityHonor.Honor);
    }

    [TestMethod]
    public void Constructor_WithMissingProperties_HandlesGracefully()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" }
            // hfid and honor_id are missing
        };

        // Act
        var addHfEntityHonor = new AddHfEntityHonor(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_entity, addHfEntityHonor.Entity);
        Assert.IsNull(addHfEntityHonor.HistoricalFigure);
        Assert.AreEqual(-1, addHfEntityHonor.HonorId);
        Assert.IsNull(addHfEntityHonor.Honor);
    }

    [TestMethod]
    public void Print_WithLinkTrue_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "honor_id", Value = "42" }
        };
        var addHfEntityHonor = new AddHfEntityHonor(properties, _mockWorld.Object);

        // Act
        var result = addHfEntityHonor.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Test Historical Figure"));
        Assert.IsTrue(result.Contains("Knight of the Deep"));
        Assert.IsTrue(result.Contains("Test Entity"));
    }

    [TestMethod]
    public void Print_WithRequirements_IncludesRequirementsInOutput()
    {
        // Arrange
        var honorWithRequirements = new Honor([], _mockWorld.Object, _entity)
        {
            Id = 42,
            Name = "Battle Veteran",
            RequiredBattles = 5,
            RequiredKills = 10
        };
        _entity.Honors.Clear();
        _entity.Honors.Add(honorWithRequirements);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "honor_id", Value = "42" }
        };
        var addHfEntityHonor = new AddHfEntityHonor(properties, _mockWorld.Object);

        // Act
        var result = addHfEntityHonor.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Battle Veteran"));
        Assert.IsTrue(result.Contains("after"));
    }

    [TestMethod]
    public void Print_WithoutRequirements_DoesNotIncludeAfterClause()
    {
        // Arrange
        var honorWithoutRequirements = new Honor([], _mockWorld.Object, _entity)
        {
            Id = 42,
            Name = "Simple Title"
            // No requirements set
        };
        _entity.Honors.Clear();
        _entity.Honors.Add(honorWithoutRequirements);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "honor_id", Value = "42" }
        };
        var addHfEntityHonor = new AddHfEntityHonor(properties, _mockWorld.Object);

        // Act
        var result = addHfEntityHonor.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("Simple Title"));
        Assert.IsFalse(result.Contains("after"));
    }

    [TestMethod]
    public void Constructor_AddsEventToEntity()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "honor_id", Value = "42" }
        };
        var initialEventCount = _entity.Events.Count;

        // Act
        var addHfEntityHonor = new AddHfEntityHonor(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _entity.Events.Count);
        Assert.IsTrue(_entity.Events.Contains(addHfEntityHonor));
    }

    [TestMethod]
    public void Constructor_AddsEventToHistoricalFigure()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "honor_id", Value = "42" }
        };
        var initialEventCount = _historicalFigure.Events.Count;

        // Act
        var addHfEntityHonor = new AddHfEntityHonor(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _historicalFigure.Events.Count);
        Assert.IsTrue(_historicalFigure.Events.Contains(addHfEntityHonor));
    }

    [TestMethod]
    public void Constructor_WithNullEntity_DoesNotThrow()
    {
        // Arrange
        _mockWorld.Setup(w => w.GetEntity(It.IsAny<int>())).Returns((Entity?)null);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "999" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "honor_id", Value = "42" }
        };

        // Act & Assert
        var addHfEntityHonor = new AddHfEntityHonor(properties, _mockWorld.Object);
        Assert.IsNull(addHfEntityHonor.Entity);
    }

    [TestMethod]
    public void Constructor_WithNullHistoricalFigure_DoesNotThrow()
    {
        // Arrange
        _mockWorld.Setup(w => w.GetHistoricalFigure(It.IsAny<int>())).Returns((HistoricalFigure?)null);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "hfid", Value = "999" },
            new Property { Name = "honor_id", Value = "42" }
        };

        // Act & Assert
        var addHfEntityHonor = new AddHfEntityHonor(properties, _mockWorld.Object);
        Assert.IsNull(addHfEntityHonor.HistoricalFigure);
    }
}
