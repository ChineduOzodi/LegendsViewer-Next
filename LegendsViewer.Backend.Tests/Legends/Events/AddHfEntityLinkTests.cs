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

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        // Create test entity
        _entity = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Entity",
            Icon = "civilization"
        };
        _entity.EntityPositions = [];

        // Create test historical figure
        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Test Figure",
            Icon = "person"
        };

        // Setup mock world to return the entity and hf
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_entity);
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "member" }
        };

        var evt = new AddHfEntityLink(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual(_entity, evt.Entity);
        Assert.AreEqual(_historicalFigure, evt.HistoricalFigure);
        Assert.AreEqual(HfEntityLinkType.Member, evt.LinkType);
    }

    [TestMethod]
    public void Constructor_WithPosition_LinkType()
    {
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "position" },
            new Property { Name = "position", Value = "King" },
            new Property { Name = "position_id", Value = "5" }
        };

        var evt = new AddHfEntityLink(properties, _mockWorld.Object);

        Assert.AreEqual(HfEntityLinkType.Position, evt.LinkType);
        Assert.AreEqual("King", evt.Position);
        Assert.AreEqual(5, evt.PositionId);
    }

    [TestMethod]
    public void Constructor_WithPrisoner_LinkType()
    {
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "prisoner" }
        };

        var evt = new AddHfEntityLink(properties, _mockWorld.Object);

        Assert.AreEqual(HfEntityLinkType.Prisoner, evt.LinkType);
    }

    [TestMethod]
    public void Constructor_WithSlave_LinkType()
    {
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "slave" }
        };

        var evt = new AddHfEntityLink(properties, _mockWorld.Object);

        Assert.AreEqual(HfEntityLinkType.Slave, evt.LinkType);
    }

    [TestMethod]
    public void Print_WithMemberLink_ReturnsFormattedString()
    {
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "member" }
        };

        var evt = new AddHfEntityLink(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Test Figure"));
        Assert.IsTrue(result.Contains("became a member of"));
        Assert.IsTrue(result.Contains("Test Entity"));
    }

    [TestMethod]
    public void Print_WithPrisonerLink_ReturnsImprisonedText()
    {
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "prisoner" }
        };

        var evt = new AddHfEntityLink(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("was imprisoned by"));
    }

    [TestMethod]
    public void Print_WithSlaveLink_ReturnsEnslavedText()
    {
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "slave" }
        };

        var evt = new AddHfEntityLink(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("was enslaved by"));
    }

    [TestMethod]
    public void PrintFeature_WithPosition_ReturnsAscensionText()
    {
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "position" },
            new Property { Name = "position", Value = "King" }
        };

        var evt = new AddHfEntityLink(properties, _mockWorld.Object);

        var result = evt.PrintFeature(link: true);

        Assert.IsTrue(result.Contains("ascension of"));
        Assert.IsTrue(result.Contains("King"));
    }

    [TestMethod]
    public void Print_WithAppointer_IncludesAppointerText()
    {
        var appointer = new HistoricalFigure
        {
            Id = 2,
            Name = "Appointer Figure",
            Icon = "person"
        };
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(appointer);

        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "link_type", Value = "position" },
            new Property { Name = "position", Value = "King" },
            new Property { Name = "appointer_hfid", Value = "2" }
        };

        var evt = new AddHfEntityLink(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("appointed by"));
        Assert.IsTrue(result.Contains("Appointer Figure"));
    }
}
