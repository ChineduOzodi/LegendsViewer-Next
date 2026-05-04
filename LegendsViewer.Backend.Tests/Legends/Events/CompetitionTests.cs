using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class CompetitionTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _civ = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _civ = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Civ",
            Icon = "civilization"
        };
        _civ.Honors = [];

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };
        _site.Structures = [];

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_civ);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_SetsOccasionTypeToCompetition()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var competition = new Competition(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(OccasionType.Competition, competition.OccasionType);
    }

    [TestMethod]
    public void Constructor_WithWinner_ParsesCorrectly()
    {
        // Arrange
        var winner = new HistoricalFigure { Id = 1, Name = "Winner", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(winner);

        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "winner_hfid", Value = "1" }
        };

        // Act
        var competition = new Competition(properties, _mockWorld.Object);

        // Assert - verify it parsed without error
        Assert.IsNotNull(competition);
    }

    [TestMethod]
    public void Constructor_WithCompetitors_ParsesCorrectly()
    {
        // Arrange
        var competitor1 = new HistoricalFigure { Id = 1, Name = "Competitor1", Icon = "person" };
        var competitor2 = new HistoricalFigure { Id = 2, Name = "Competitor2", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(competitor1);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(competitor2);

        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "competitor_hfid", Value = "1" },
            new Property { Name = "competitor_hfid", Value = "2" }
        };

        // Act
        var competition = new Competition(properties, _mockWorld.Object);

        // Assert - verify it parsed without error
        Assert.IsNotNull(competition);
    }

    [TestMethod]
    public void Constructor_AddsEventToWinner()
    {
        // Arrange
        var winner = new HistoricalFigure { Id = 1, Name = "Winner", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(winner);

        var properties = new List<Property>
        {
            new Property { Name = "winner_hfid", Value = "1" }
        };
        var initialEventCount = winner.Events.Count;

        // Act
        var competition = new Competition(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, winner.Events.Count);
    }

    [TestMethod]
    public void Print_ReturnsNonEmptyString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var competition = new Competition(properties, _mockWorld.Object);

        // Act
        var result = competition.Print(link: true);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsNonEmptyString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var competition = new Competition(properties, _mockWorld.Object);

        // Act
        var result = competition.Print(link: false);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }
}
