using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class AddHfEntityLinkTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _entity = null!;
    private HistoricalFigure _historicalFigure = null!;
    private HistoricalFigure _appointerHf = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        // Create test entity
        _entity = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Kingdom",
            Icon = "civilization"
        };
        _entity.EntityPositions = [];

        // Create test historical figure
        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Baron Urist",
            Icon = "human"
        };

        // Create appointer historical figure
        _appointerHf = new HistoricalFigure
        {
            Id = 2,
            Name = "King Thiel",
            Icon = "human"
        };

        // Setup mock world to return the entity and hf
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_entity);
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_appointerHf);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "member" }
        };

        // Act
        var addHfEntityLink = new AddHfEntityLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_entity, addHfEntityLink.Entity);
        Assert.AreEqual(_historicalFigure, addHfEntityLink.HistoricalFigure);
        Assert.AreEqual(HfEntityLinkType.Member, addHfEntityLink.LinkType);
    }

    [TestMethod]
    public void Constructor_WithPositionLinkType_ParsesPosition()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "position" },
            new Property { Name = "position", Value = "High Marshal" },
            new Property { Name = "position_id", Value = "5" }
        };

        // Act
        var addHfEntityLink = new AddHfEntityLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(HfEntityLinkType.Position, addHfEntityLink.LinkType);
        Assert.AreEqual("High Marshal", addHfEntityLink.Position);
        Assert.AreEqual(5, addHfEntityLink.PositionId);
    }

    [TestMethod]
    public void Constructor_WithAppointer_AssignsAppointer()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "position" },
            new Property { Name = "appointer_hfid", Value = "2" }
        };

        // Act
        var addHfEntityLink = new AddHfEntityLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_appointerHf, addHfEntityLink.AppointerHf);
    }

    [TestMethod]
    public void Constructor_WithPromiseToHf_AssignsPromiseTo()
    {
        // Arrange
        var promiseToHf = new HistoricalFigure
        {
            Id = 3,
            Name = "Lord Promise",
            Icon = "human"
        };
        _mockWorld.Setup(w => w.GetHistoricalFigure(3)).Returns(promiseToHf);

        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "position" },
            new Property { Name = "promise_to_hfid", Value = "3" }
        };

        // Act
        var addHfEntityLink = new AddHfEntityLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(promiseToHf, addHfEntityLink.PromiseToHf);
    }

    [TestMethod]
    public void Constructor_AddsEventToHistoricalFigure()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "member" }
        };
        var initialEventCount = _historicalFigure.Events.Count;

        // Act
        var addHfEntityLink = new AddHfEntityLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _historicalFigure.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToEntity()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "member" }
        };
        var initialEventCount = _entity.Events.Count;

        // Act
        var addHfEntityLink = new AddHfEntityLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _entity.Events.Count);
    }

    [TestMethod]
    public void Print_MemberLink_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "member" }
        };
        var addHfEntityLink = new AddHfEntityLink(properties, _mockWorld.Object);

        // Act
        var result = addHfEntityLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Baron Urist"));
        Assert.IsTrue(result.Contains("became a member of"));
        Assert.IsTrue(result.Contains("Test Kingdom"));
    }

    [TestMethod]
    public void Print_PrisonerLink_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "prisoner" }
        };
        var addHfEntityLink = new AddHfEntityLink(properties, _mockWorld.Object);

        // Act
        var result = addHfEntityLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("was imprisoned by"));
    }

    [TestMethod]
    public void Print_SlaveLink_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "slave" }
        };
        var addHfEntityLink = new AddHfEntityLink(properties, _mockWorld.Object);

        // Act
        var result = addHfEntityLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("was enslaved by"));
    }

    [TestMethod]
    public void Print_WithAppointer_IncludesAppointer()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "position" },
            new Property { Name = "appointer_hfid", Value = "2" }
        };
        var addHfEntityLink = new AddHfEntityLink(properties, _mockWorld.Object);

        // Act
        var result = addHfEntityLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("appointed by"));
        Assert.IsTrue(result.Contains("King Thiel"));
    }

    [TestMethod]
    public void PrintFeature_WithPosition_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "position" },
            new Property { Name = "position", Value = "High Marshal" }
        };
        var addHfEntityLink = new AddHfEntityLink(properties, _mockWorld.Object);

        // Act
        var result = addHfEntityLink.PrintFeature(link: true);

        // Assert
        Assert.IsTrue(result.Contains("the ascension of"));
        Assert.IsTrue(result.Contains("High Marshal"));
    }

    [TestMethod]
    public void Constructor_WithNullEntity_DoesNotThrow()
    {
        // Arrange
        _mockWorld.Setup(w => w.GetEntity(It.IsAny<int>())).Returns((Entity?)null);

        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "999" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "member" }
        };

        // Act & Assert
        var addHfEntityLink = new AddHfEntityLink(properties, _mockWorld.Object);
        Assert.IsNull(addHfEntityLink.Entity);
    }

    [TestMethod]
    public void Constructor_WithNullHistoricalFigure_DoesNotThrow()
    {
        // Arrange
        _mockWorld.Setup(w => w.GetHistoricalFigure(It.IsAny<int>())).Returns((HistoricalFigure?)null);

        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "999" },
            new Property { Name = "link_type", Value = "member" }
        };

        // Act & Assert
        var addHfEntityLink = new AddHfEntityLink(properties, _mockWorld.Object);
        Assert.IsNull(addHfEntityLink.HistoricalFigure);
    }
}
