using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class MasterpieceItemImprovementTests
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
            Name = "Master Crafter",
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
            new Property { Name = "item_type", Value = "weapon" },
            new Property { Name = "improvement_type", Value = "covered" },
            new Property { Name = "mat", Value = "iron" }
        };

        // Act
        var masterpiece = new MasterpieceItemImprovement(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_historicalFigure, masterpiece.Improver);
        Assert.AreEqual(_entity, masterpiece.ImproverEntity);
        Assert.AreEqual(_site, masterpiece.Site);
        Assert.AreEqual("weapon", masterpiece.ItemType);
        Assert.AreEqual("covered", masterpiece.ImprovementType);
        Assert.AreEqual("iron", masterpiece.Material);
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
            new Property { Name = "item_type", Value = "weapon" },
            new Property { Name = "improvement_type", Value = "covered" },
            new Property { Name = "mat", Value = "iron" }
        };
        var masterpiece = new MasterpieceItemImprovement(properties, _mockWorld.Object);

        // Act
        var result = masterpiece.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Master Crafter"));
        Assert.IsTrue(result.Contains("masterful covering"));
        Assert.IsTrue(result.Contains("iron"));
        Assert.IsTrue(result.Contains("weapon"));
    }

    [TestMethod]
    public void Print_WithArtImage_ReturnsImageString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "item_type", Value = "weapon" },
            new Property { Name = "improvement_type", Value = "art image" }
        };
        var masterpiece = new MasterpieceItemImprovement(properties, _mockWorld.Object);

        // Act
        var result = masterpiece.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("masterful image"));
    }
}
