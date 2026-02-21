using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class MasterpieceLostTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _historicalFigure = null!;
    private HistoricalFigure _maker = null!;
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

        _maker = new HistoricalFigure
        {
            Id = 2,
            Name = "Original Maker",
            Icon = "person"
        };

        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Destroyer",
            Icon = "person"
        };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_maker);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_entity);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site", Value = "1" },
            new Property { Name = "method", Value = "buried" }
        };

        // Act
        var masterpiece = new MasterpieceLost(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_historicalFigure, masterpiece.HistoricalFigure);
        Assert.AreEqual(_site, masterpiece.Site);
        Assert.AreEqual("buried", masterpiece.Method);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site", Value = "1" },
            new Property { Name = "method", Value = "buried" }
        };
        var masterpiece = new MasterpieceLost(properties, _mockWorld.Object);

        // Act
        var result = masterpiece.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Destroyer"));
        Assert.IsTrue(result.Contains("was destroyed by"));
        Assert.IsTrue(result.Contains("Test Fortress"));
    }

    [TestMethod]
    public void Print_WithoutCreationEvent_SaysUnknown()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "site", Value = "1" }
        };
        var masterpiece = new MasterpieceLost(properties, _mockWorld.Object);

        // Act
        var result = masterpiece.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("UNKNOWN ITEM"));
    }
}
