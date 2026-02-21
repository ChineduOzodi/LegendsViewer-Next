using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class OccasionEventTests
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
            Name = "Test Civilization",
            Icon = "civilization"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "CITY"
        };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_civ);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "occasion_id", Value = "0" },
            new Property { Name = "schedule_id", Value = "0" }
        };

        // Act
        var evt = new OccasionEvent(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_civ, evt.Civ);
        Assert.AreEqual(_site, evt.Site);
        Assert.AreEqual(0, evt.OccasionId);
        Assert.AreEqual(0, evt.ScheduleId);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "occasion_id", Value = "0" },
            new Property { Name = "schedule_id", Value = "0" }
        };

        // Act
        var evt = new OccasionEvent(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Test Civilization"));
        Assert.IsTrue(result.Contains("held a"));
        Assert.IsTrue(result.Contains("Test Site"));
    }

    [TestMethod]
    public void Constructor_WithMissingProperties_HandlesNulls()
    {
        // Arrange
        var properties = new List<Property>();

        // Act
        var evt = new OccasionEvent(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.IsNull(evt.Civ);
        Assert.IsNull(evt.Site);
    }
}
