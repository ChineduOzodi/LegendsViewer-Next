using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class NewSiteLeaderTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _attacker = null!;
    private Entity _defender = null!;
    private Entity _siteEntity = null!;
    private Entity _newSiteEntity = null!;
    private Site _site = null!;
    private HistoricalFigure _newLeader = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _attacker = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Attacker Kingdom",
            Icon = "civilization"
        };

        _defender = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Defender Kingdom",
            Icon = "civilization"
        };

        _siteEntity = new Entity([], _mockWorld.Object)
        {
            Id = 3,
            Name = "Site Entity",
            Icon = "civilization"
        };

        _newSiteEntity = new Entity([], _mockWorld.Object)
        {
            Id = 4,
            Name = "New Government",
            Icon = "civilization"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };

        _newLeader = new HistoricalFigure
        {
            Id = 1,
            Name = "New Leader",
            Icon = "person"
        };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_attacker);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_defender);
        _mockWorld.Setup(w => w.GetEntity(3)).Returns(_siteEntity);
        _mockWorld.Setup(w => w.GetEntity(4)).Returns(_newSiteEntity);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_newLeader);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "site_civ_id", Value = "3" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "new_site_civ_id", Value = "4" },
            new Property { Name = "new_leader_hfid", Value = "1" }
        };

        // Act
        var evt = new NewSiteLeader(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_attacker, evt.Attacker);
        Assert.AreEqual(_defender, evt.Defender);
        Assert.AreEqual(_siteEntity, evt.SiteEntity);
        Assert.AreEqual(_newSiteEntity, evt.NewSiteEntity);
        Assert.AreEqual(_site, evt.Site);
        Assert.AreEqual(_newLeader, evt.NewLeader);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "site_civ_id", Value = "3" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "new_site_civ_id", Value = "4" },
            new Property { Name = "new_leader_hfid", Value = "1" }
        };

        // Act
        var evt = new NewSiteLeader(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Attacker Kingdom"));
        Assert.IsTrue(result.Contains("defeated"));
        Assert.IsTrue(result.Contains("Defender Kingdom"));
        Assert.IsTrue(result.Contains("placed"));
        Assert.IsTrue(result.Contains("New Leader"));
        Assert.IsTrue(result.Contains("in charge of"));
        Assert.IsTrue(result.Contains("Test Site"));
    }

    [TestMethod]
    public void Print_WithNewGovernment_IncludesNewGovernmentName()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "defender_civ_id", Value = "2" },
            new Property { Name = "site_civ_id", Value = "3" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "new_site_civ_id", Value = "4" },
            new Property { Name = "new_leader_hfid", Value = "1" }
        };

        // Act
        var evt = new NewSiteLeader(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("new government"));
        Assert.IsTrue(result.Contains("New Government"));
    }

    [TestMethod]
    [ExpectedException(typeof(System.InvalidOperationException))]
    public void Constructor_WithMissingProperties_ThrowsException()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "attacker_civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act - should throw because Site.OwnerHistory.Last() is called with empty list
        var evt = new NewSiteLeader(properties, _mockWorld.Object);
    }
}
