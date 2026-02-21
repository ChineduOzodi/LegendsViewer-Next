using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class ModifiedBuildingTests
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
        _site.Coordinates = [];

        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Builder",
            Icon = "person"
        };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "modifier_hfid", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure_id", Value = "100" },
            new Property { Name = "modification", Value = "furnace" }
        };

        // Act
        var modified = new ModifiedBuilding(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_historicalFigure, modified.ModifierHf);
        Assert.AreEqual(_site, modified.Site);
        Assert.AreEqual("furnace", modified.Modification);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "modifier_hfid", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure_id", Value = "100" },
            new Property { Name = "modification", Value = "furnace" }
        };
        var modified = new ModifiedBuilding(properties, _mockWorld.Object);

        // Act
        var result = modified.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Builder"));
        Assert.IsTrue(result.Contains("furnace"));
        Assert.IsTrue(result.Contains("Test Fortress"));
    }
}
