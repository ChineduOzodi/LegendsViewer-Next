using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class BuildingProfileAcquiredTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Site _site = null!;
    private HistoricalFigure _acquirerHf = null!;
    private Entity _acquirerEntity = null!;
    private HistoricalFigure _lastOwnerHf = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };
        _site.Structures = [];
        _site.SiteProperties = [];

        _acquirerHf = new HistoricalFigure
        {
            Id = 1,
            Name = "Acquirer",
            Icon = "person"
        };

        _acquirerEntity = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Acquirer Entity",
            Icon = "civilization"
        };
        _acquirerEntity.Honors = [];

        _lastOwnerHf = new HistoricalFigure
        {
            Id = 2,
            Name = "Last Owner",
            Icon = "person"
        };

        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_acquirerHf);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_lastOwnerHf);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_acquirerEntity);
    }

    [TestMethod]
    public void Constructor_WithBasicProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "acquirer_hfid", Value = "1" },
            new Property { Name = "building_profile_id", Value = "1" }
        };

        // Act
        var buildingProfileAcquired = new BuildingProfileAcquired(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(buildingProfileAcquired);
        Assert.AreEqual(_site, buildingProfileAcquired.Site);
        Assert.AreEqual(_acquirerHf, buildingProfileAcquired.AcquirerHf);
        Assert.AreEqual(1, buildingProfileAcquired.BuildingProfileId);
    }

    [TestMethod]
    public void Constructor_WithInherited_SetsFlag()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "acquirer_hfid", Value = "1" },
            new Property { Name = "inherited", Value = "true" }
        };

        // Act
        var buildingProfileAcquired = new BuildingProfileAcquired(properties, _mockWorld.Object);

        // Assert
        Assert.IsTrue(buildingProfileAcquired.Inherited);
    }

    [TestMethod]
    public void Constructor_WithPurchasedUnowned_SetsFlag()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "acquirer_hfid", Value = "1" },
            new Property { Name = "purchased_unowned", Value = "true" }
        };

        // Act
        var buildingProfileAcquired = new BuildingProfileAcquired(properties, _mockWorld.Object);

        // Assert
        Assert.IsTrue(buildingProfileAcquired.PurchasedUnowned);
    }

    [TestMethod]
    public void Constructor_WithLastOwner_SetsLastOwner()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "acquirer_hfid", Value = "1" },
            new Property { Name = "last_owner_hfid", Value = "2" }
        };

        // Act
        var buildingProfileAcquired = new BuildingProfileAcquired(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_lastOwnerHf, buildingProfileAcquired.LastOwnerHf);
    }

    [TestMethod]
    public void Constructor_AddsEventToSite()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" }
        };
        var initialEventCount = _site.Events.Count;

        // Act
        var buildingProfileAcquired = new BuildingProfileAcquired(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _site.Events.Count);
    }

    [TestMethod]
    public void Print_WithAcquirerHf_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "acquirer_hfid", Value = "1" }
        };
        var buildingProfileAcquired = new BuildingProfileAcquired(properties, _mockWorld.Object);

        // Act
        var result = buildingProfileAcquired.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Acquirer"));
        Assert.IsTrue(result.Contains("acquired"));
    }

    [TestMethod]
    public void Print_WithInherited_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "acquirer_hfid", Value = "1" },
            new Property { Name = "inherited", Value = "true" }
        };
        var buildingProfileAcquired = new BuildingProfileAcquired(properties, _mockWorld.Object);

        // Act
        var result = buildingProfileAcquired.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("inherited"));
    }

    [TestMethod]
    public void Print_WithPurchasedUnowned_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "acquirer_hfid", Value = "1" },
            new Property { Name = "purchased_unowned", Value = "true" }
        };
        var buildingProfileAcquired = new BuildingProfileAcquired(properties, _mockWorld.Object);

        // Act
        var result = buildingProfileAcquired.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("purchased"));
    }

    [TestMethod]
    public void Print_WithLastOwner_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "acquirer_hfid", Value = "1" },
            new Property { Name = "last_owner_hfid", Value = "2" }
        };
        var buildingProfileAcquired = new BuildingProfileAcquired(properties, _mockWorld.Object);

        // Act
        var result = buildingProfileAcquired.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("formerly owned"));
        Assert.IsTrue(result.Contains("Last Owner"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "acquirer_hfid", Value = "1" }
        };
        var buildingProfileAcquired = new BuildingProfileAcquired(properties, _mockWorld.Object);

        // Act
        var result = buildingProfileAcquired.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("acquired"));
    }
}
