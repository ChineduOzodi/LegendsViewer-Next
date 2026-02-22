using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class SquadVsSquadTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _attackerHf = null!;
    private HistoricalFigure _defenderHf = null!;
    private HistoricalFigure _attackerLeader = null!;
    private HistoricalFigure _defenderLeader = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _attackerHf = new HistoricalFigure
        {
            Id = 1,
            Name = "Attacker Warrior",
            Icon = "person"
        };

        _defenderHf = new HistoricalFigure
        {
            Id = 2,
            Name = "Defender Warrior",
            Icon = "person"
        };

        _attackerLeader = new HistoricalFigure
        {
            Id = 3,
            Name = "Attacker Captain",
            Icon = "person"
        };

        _defenderLeader = new HistoricalFigure
        {
            Id = 4,
            Name = "Defender Captain",
            Icon = "person"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Battle Site",
            Type = "TOWER"
        };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_attackerHf);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_defenderHf);
        _mockWorld.Setup(w => w.GetHistoricalFigure(3)).Returns(_attackerLeader);
        _mockWorld.Setup(w => w.GetHistoricalFigure(4)).Returns(_defenderLeader);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "a_hfid", Value = "1" },
            new Property { Name = "d_hfid", Value = "2" },
            new Property { Name = "a_squad_id", Value = "1" },
            new Property { Name = "d_squad_id", Value = "2" },
            new Property { Name = "d_race", Value = "0" },
            new Property { Name = "d_number", Value = "10" },
            new Property { Name = "d_slain", Value = "3" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure_id", Value = "1" },
            new Property { Name = "a_leader_hfid", Value = "3" },
            new Property { Name = "d_leader_hfid", Value = "4" },
            new Property { Name = "a_leadership_roll", Value = "75" },
            new Property { Name = "d_leadership_roll", Value = "50" }
        };

        // Act
        var evt = new SquadVsSquad(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_attackerHf, evt.AttackerHistoricalFigure);
        Assert.AreEqual(_defenderHf, evt.DefenderHistoricalFigure);
        Assert.AreEqual(10, evt.DefenderNumber);
        Assert.AreEqual(3, evt.DefenderSlain);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsClashString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "a_hfid", Value = "1" },
            new Property { Name = "d_hfid", Value = "2" },
            new Property { Name = "a_squad_id", Value = "1" },
            new Property { Name = "d_squad_id", Value = "2" },
            new Property { Name = "d_race", Value = "0" },
            new Property { Name = "d_number", Value = "10" },
            new Property { Name = "d_slain", Value = "3" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure_id", Value = "1" },
            new Property { Name = "a_leader_hfid", Value = "3" },
            new Property { Name = "d_leader_hfid", Value = "4" },
            new Property { Name = "a_leadership_roll", Value = "75" },
            new Property { Name = "d_leadership_roll", Value = "50" }
        };

        // Act
        var evt = new SquadVsSquad(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("clashed"));
        Assert.IsTrue(result.Contains("slaying"));
    }
}
