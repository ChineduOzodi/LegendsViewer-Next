using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class TacticalSituationTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _attackerTactician = null!;
    private HistoricalFigure _defenderTactician = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _attackerTactician = new HistoricalFigure
        {
            Id = 1,
            Name = "Tactical Commander Alpha",
            Icon = "person"
        };

        _defenderTactician = new HistoricalFigure
        {
            Id = 2,
            Name = "Tactical Commander Beta",
            Icon = "person"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Battlefield",
            Type = "HILL"
        };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_attackerTactician);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_defenderTactician);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "a_tactician_hfid", Value = "1" },
            new Property { Name = "d_tactician_hfid", Value = "2" },
            new Property { Name = "a_tactics_roll", Value = "85" },
            new Property { Name = "d_tactics_roll", Value = "45" },
            new Property { Name = "situation", Value = "a strongly favored" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var evt = new TacticalSituation(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_attackerTactician, evt.AttackerTactician);
        Assert.AreEqual(_defenderTactician, evt.DefenderTactician);
        Assert.AreEqual(85, evt.AttackerTacticsRoll);
        Assert.AreEqual(TacticalSituationType.AttackersStronglyFavored, evt.Situation);
    }

    [TestMethod]
    public void Print_WithBothTacticians_ReturnsOutmaneuveredString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "a_tactician_hfid", Value = "1" },
            new Property { Name = "d_tactician_hfid", Value = "2" },
            new Property { Name = "a_tactics_roll", Value = "85" },
            new Property { Name = "d_tactics_roll", Value = "45" },
            new Property { Name = "situation", Value = "a strongly favored" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var evt = new TacticalSituation(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("outmanuevered") || result.Contains("outwitted"));
        Assert.IsTrue(result.Contains("Battlefield"));
    }
}
