using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class RemoveHfEntityLinkTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _entity = null!;
    private HistoricalFigure _historicalFigure = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _entity = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Kingdom",
            Icon = "civilization"
        };

        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Test Figure",
            Icon = "person"
        };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_entity);
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
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
        var evt = new RemoveHfEntityLink(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_entity, evt.Entity);
        Assert.AreEqual(_historicalFigure, evt.HistoricalFigure);
        Assert.AreEqual(HfEntityLinkType.Member, evt.LinkType);
    }

    [TestMethod]
    public void Constructor_WithPrisonerLinkType_SetsPrisoner()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "prisoner" }
        };

        // Act
        var evt = new RemoveHfEntityLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(HfEntityLinkType.Prisoner, evt.LinkType);
    }

    [TestMethod]
    public void Print_WithMemberLink_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "member" }
        };

        // Act
        var evt = new RemoveHfEntityLink(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Test Figure"));
        Assert.IsTrue(result.Contains("left"));
        Assert.IsTrue(result.Contains("Test Kingdom"));
    }

    [TestMethod]
    public void Print_WithPrisonerLink_ReturnsEscapedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "prisoner" }
        };

        // Act
        var evt = new RemoveHfEntityLink(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("escaped"));
    }
}
