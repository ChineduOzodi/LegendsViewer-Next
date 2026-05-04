using LegendsViewer.Backend.Legends.Events.IncidentalEvents;
using LegendsViewer.Backend.Legends.EventCollections;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class BattleFoughtTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _historicalFigure = null!;
    private Battle _battle = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.Events).Returns(new List<LegendsViewer.Backend.Legends.Events.WorldEvent>());

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Fortress",
            Icon = "fortress"
        };

        _battle = new Battle([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Battle of the Valley"
        };
        _battle.Site = _site;

        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "General Ironfist",
            Icon = "person"
        };
    }

    [TestMethod]
    public void Constructor_WithValidParameters_SetsProperties()
    {
        // Act
        var battleFought = new BattleFought(_historicalFigure, _battle, _mockWorld.Object, asAttacker: true);

        // Assert
        Assert.AreEqual(_historicalFigure, battleFought.HistoricalFigure);
        Assert.AreEqual(_battle, battleFought.Battle);
        Assert.AreEqual(true, battleFought.AsAttacker);
        Assert.AreEqual(false, battleFought.WasHired);
        Assert.AreEqual(false, battleFought.AsScout);
        Assert.AreEqual(_site, battleFought.Site);
    }

    [TestMethod]
    public void Constructor_WithHiredAndScout_SetsFlags()
    {
        // Act
        var battleFought = new BattleFought(_historicalFigure, _battle, _mockWorld.Object, asAttacker: false, wasHired: true, asScout: true);

        // Assert
        Assert.AreEqual(true, battleFought.WasHired);
        Assert.AreEqual(true, battleFought.AsScout);
        Assert.AreEqual(false, battleFought.AsAttacker);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        // Arrange
        var battleFought = new BattleFought(_historicalFigure, _battle, _mockWorld.Object, asAttacker: true);

        // Act
        var result = battleFought.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("General Ironfist"));
        Assert.IsTrue(result.Contains("Battle of the Valley"));
        Assert.IsTrue(result.Contains("an assault on"));
    }

    [TestMethod]
    public void Print_WithDefender_ReturnsDefenseString()
    {
        // Arrange
        var battleFought = new BattleFought(_historicalFigure, _battle, _mockWorld.Object, asAttacker: false);

        // Act
        var result = battleFought.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("in defense of"));
    }

    [TestMethod]
    public void Print_WithHiredFighter_ReturnsHiredString()
    {
        // Arrange
        var battleFought = new BattleFought(_historicalFigure, _battle, _mockWorld.Object, asAttacker: true, wasHired: true);

        // Act
        var result = battleFought.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("was hired"));
        Assert.IsTrue(result.Contains("to fight in"));
    }

    [TestMethod]
    public void Print_WithHiredScout_ReturnsScoutString()
    {
        // Arrange
        var battleFought = new BattleFought(_historicalFigure, _battle, _mockWorld.Object, asAttacker: true, wasHired: true, asScout: true);

        // Act
        var result = battleFought.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("was hired"));
        Assert.IsTrue(result.Contains("as a scout"));
    }
}
