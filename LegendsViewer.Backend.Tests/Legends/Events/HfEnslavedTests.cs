using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfEnslavedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _enslavedHf = null!;
    private HistoricalFigure _sellerHf = null!;
    private HistoricalFigure _payerHf = null!;
    private Entity _payerEntity = null!;
    private Site _movedToSite = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        // Create enslaved figure
        _enslavedHf = new HistoricalFigure
        {
            Id = 1,
            Name = "Captive",
            Icon = "person"
        };

        // Create seller
        _sellerHf = new HistoricalFigure
        {
            Id = 2,
            Name = "Slave Trader",
            Icon = "person"
        };

        // Create payer
        _payerHf = new HistoricalFigure
        {
            Id = 3,
            Name = "Buyer",
            Icon = "person"
        };

        // Create payer entity
        _payerEntity = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Merchant Guild",
            Icon = "civilization"
        };

        // Create site
        _movedToSite = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Plantation"
        };

        // Setup mock world
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_enslavedHf);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_sellerHf);
        _mockWorld.Setup(w => w.GetHistoricalFigure(3)).Returns(_payerHf);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_payerEntity);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_movedToSite);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "enslaved_hfid", Value = "1" },
            new Property { Name = "seller_hfid", Value = "2" },
            new Property { Name = "payer_hfid", Value = "3" },
            new Property { Name = "payer_entity_id", Value = "1" },
            new Property { Name = "moved_to_site_id", Value = "1" }
        };

        // Act
        var hfEnslaved = new HfEnslaved(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_enslavedHf, hfEnslaved.EnslavedHf);
        Assert.AreEqual(_sellerHf, hfEnslaved.SellerHf);
        Assert.AreEqual(_payerHf, hfEnslaved.PayerHf);
    }

    [TestMethod]
    public void Print_WithLinkTrue_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "enslaved_hfid", Value = "1" },
            new Property { Name = "seller_hfid", Value = "2" },
            new Property { Name = "payer_hfid", Value = "3" },
            new Property { Name = "payer_entity_id", Value = "1" },
            new Property { Name = "moved_to_site_id", Value = "1" }
        };
        var hfEnslaved = new HfEnslaved(properties, _mockWorld.Object);

        // Act
        var result = hfEnslaved.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Slave Trader"));
        Assert.IsTrue(result.Contains("sold"));
        Assert.IsTrue(result.Contains("Captive"));
    }

    [TestMethod]
    public void Constructor_AddsEventToEnslaved()
    {
        // Arrange
        var initialEventCount = _enslavedHf.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "enslaved_hfid", Value = "1" },
            new Property { Name = "seller_hfid", Value = "2" }
        };

        // Act
        var hfEnslaved = new HfEnslaved(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _enslavedHf.Events.Count);
    }
}
