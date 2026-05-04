using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class SiteDisputeTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _entity1 = null!;
    private Entity _entity2 = null!;
    private Site _site1 = null!;
    private Site _site2 = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _entity1 = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Faction One",
            Icon = "civilization"
        };

        _entity2 = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Faction Two",
            Icon = "civilization"
        };

        _site1 = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Site Alpha",
            Type = "TOWER"
        };

        _site2 = new Site([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Site Beta",
            Type = "TOWER"
        };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_entity1);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_entity2);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site1);
        _mockWorld.Setup(w => w.GetSite(2)).Returns(_site2);
    }

    [TestMethod]
    public void Constructor_WithTerritoryDispute_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "dispute", Value = "territory" },
            new Property { Name = "entity_id_1", Value = "1" },
            new Property { Name = "entity_id_2", Value = "2" },
            new Property { Name = "site_id_1", Value = "1" },
            new Property { Name = "site_id_2", Value = "2" }
        };

        // Act
        var evt = new SiteDispute(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(Dispute.Territory, evt.Dispute);
        Assert.AreEqual(_entity1, evt.Entity1);
        Assert.AreEqual(_entity2, evt.Entity2);
        Assert.AreEqual(_site1, evt.Site1);
        Assert.AreEqual(_site2, evt.Site2);
    }

    [TestMethod]
    public void Constructor_WithWaterRights_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "dispute", Value = "water rights" },
            new Property { Name = "entity_id_1", Value = "1" },
            new Property { Name = "entity_id_2", Value = "2" },
            new Property { Name = "site_id_1", Value = "1" },
            new Property { Name = "site_id_2", Value = "2" }
        };

        // Act
        var evt = new SiteDispute(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(Dispute.WaterRights, evt.Dispute);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsDisputeString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "dispute", Value = "territory" },
            new Property { Name = "entity_id_1", Value = "1" },
            new Property { Name = "entity_id_2", Value = "2" },
            new Property { Name = "site_id_1", Value = "1" },
            new Property { Name = "site_id_2", Value = "2" }
        };

        // Act
        var evt = new SiteDispute(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("dispute"));
        Assert.IsTrue(result.Contains("territory"));
        Assert.IsTrue(result.Contains("Site Alpha"));
        Assert.IsTrue(result.Contains("Site Beta"));
    }
}
