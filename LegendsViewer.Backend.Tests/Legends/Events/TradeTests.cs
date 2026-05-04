using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class TradeTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _trader = null!;
    private Entity _traderEntity = null!;
    private Site _sourceSite = null!;
    private Site _destSite = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _trader = new HistoricalFigure
        {
            Id = 1,
            Name = "Merchant",
            Icon = "person"
        };

        _traderEntity = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Trading Company",
            Icon = "civilization"
        };

        _sourceSite = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Origin City",
            Type = "CITY"
        };

        _destSite = new Site([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Destination City",
            Type = "TOWER"
        };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_trader);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_traderEntity);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_sourceSite);
        _mockWorld.Setup(w => w.GetSite(2)).Returns(_destSite);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "trader_hfid", Value = "1" },
            new Property { Name = "trader_entity_id", Value = "2" },
            new Property { Name = "source_site_id", Value = "1" },
            new Property { Name = "dest_site_id", Value = "2" },
            new Property { Name = "account_shift", Value = "1500" }
        };

        // Act
        var evt = new Trade(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_trader, evt.Trader);
        Assert.AreEqual(_traderEntity, evt.TraderEntity);
        Assert.AreEqual(_sourceSite, evt.SourceSite);
        Assert.AreEqual(_destSite, evt.DestSite);
        Assert.AreEqual(1500, evt.AccountShift);
    }

    [TestMethod]
    public void Print_WithPositiveBalance_ReturnsDidWell()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "trader_hfid", Value = "1" },
            new Property { Name = "trader_entity_id", Value = "2" },
            new Property { Name = "source_site_id", Value = "1" },
            new Property { Name = "dest_site_id", Value = "2" },
            new Property { Name = "account_shift", Value = "1500" }
        };

        // Act
        var evt = new Trade(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("trading"));
        Assert.IsTrue(result.Contains("did well") || result.Contains("fortune"));
    }

    [TestMethod]
    public void Print_WithNegativeBalance_ReturnsDidPoorly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "trader_hfid", Value = "1" },
            new Property { Name = "trader_entity_id", Value = "2" },
            new Property { Name = "source_site_id", Value = "1" },
            new Property { Name = "dest_site_id", Value = "2" },
            new Property { Name = "account_shift", Value = "-1500" }
        };

        // Act
        var evt = new Trade(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("trading"));
        Assert.IsTrue(result.Contains("did poorly") || result.Contains("lost"));
    }
}
