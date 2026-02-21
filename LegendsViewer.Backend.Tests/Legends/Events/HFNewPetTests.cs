using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfNewPetTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _historicalFigure = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Fortress",
            Icon = "fortress"
        };

        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Test Hero",
            Icon = "person"
        };
        _historicalFigure.JourneyPets = [];

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
        _mockWorld.Setup(w => w.GetCreatureInfo(It.IsAny<string>())).Returns(LegendsViewer.Backend.Legends.Various.CreatureInfo.Unknown);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "group", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "pets", Value = "tiger" }
        };

        // Act
        var hfNewPet = new HfNewPet(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual("Tiger", hfNewPet.Pet);
        Assert.AreEqual(_historicalFigure, hfNewPet.HistoricalFigure);
        Assert.AreEqual(_site, hfNewPet.Site);
    }

    [TestMethod]
    public void Constructor_WithUnknownPet_UsesFormattedRace()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "group", Value = "1" },
            new Property { Name = "pets", Value = "cave_dragon" }
        };

        // Act
        var hfNewPet = new HfNewPet(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual("Cave Dragon", hfNewPet.Pet);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "group", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "pets", Value = "tiger" }
        };
        var hfNewPet = new HfNewPet(properties, _mockWorld.Object);

        // Act
        var result = hfNewPet.Print(link: true);

        // Debug
        Console.WriteLine($"Result: {result}");

        // Assert
        Assert.IsTrue(result.Contains("Test Hero"));
        Assert.IsTrue(result.Contains("tamed the creatures named"));
        Assert.IsTrue(result.Contains("Test Fortress"));
    }

    [TestMethod]
    public void Print_WithRegion_UsesRegion()
    {
        // Arrange
        var region = new WorldRegion([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Region",
            Icon = "region"
        };
        _mockWorld.Setup(w => w.GetRegion(1)).Returns(region);

        var properties = new List<Property>
        {
            new Property { Name = "group", Value = "1" },
            new Property { Name = "subregion_id", Value = "1" },
            new Property { Name = "pets", Value = "wolf" }
        };
        var hfNewPet = new HfNewPet(properties, _mockWorld.Object);

        // Act
        var result = hfNewPet.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Test Region"));
    }

    [TestMethod]
    public void Print_WithoutPet_SaysUnknown()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "group", Value = "1" }
        };
        var hfNewPet = new HfNewPet(properties, _mockWorld.Object);

        // Act
        var result = hfNewPet.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("UNKNOWN"));
    }
}
