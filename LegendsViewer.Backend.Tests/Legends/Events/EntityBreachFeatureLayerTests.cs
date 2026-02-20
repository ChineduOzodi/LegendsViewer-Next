using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class EntityBreachFeatureLayerTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var properties = new List<Property>
        {
            new Property { Name = "site_entity_id", Value = "1" },
            new Property { Name = "civ_entity_id", Value = "2" },
            new Property { Name = "site_id", Value = "3" },
            new Property { Name = "feature_layer_id", Value = "4" }
        };

        var evt = new EntityBreachFeatureLayer(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        var siteEntity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Dwarves", Icon = "civilization" };
        var civEntity = new Entity([], _mockWorld.Object) { Id = 2, Name = "Mountain Home", Icon = "civilization" };
        var site = new Site([], _mockWorld.Object) { Id = 3, Name = "Fort", Icon = "location" };
        var undergroundRegion = new UndergroundRegion([], _mockWorld.Object) { Id = 4, Name = "Cavern", Icon = "cave" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(siteEntity);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(civEntity);
        _mockWorld.Setup(w => w.GetSite(3)).Returns(site);
        _mockWorld.Setup(w => w.GetUndergroundRegion(4)).Returns(undergroundRegion);

        var properties = new List<Property>
        {
            new Property { Name = "site_entity_id", Value = "1" },
            new Property { Name = "civ_entity_id", Value = "2" },
            new Property { Name = "site_id", Value = "3" },
            new Property { Name = "feature_layer_id", Value = "4" }
        };

        var evt = new EntityBreachFeatureLayer(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Dwarves"));
        Assert.IsTrue(result.Contains("Mountain Home"));
        Assert.IsTrue(result.Contains("Fort"));
        Assert.IsTrue(result.Contains("Cavern"));
        Assert.IsTrue(result.Contains("breached"));
    }

    [TestMethod]
    public void Print_WithoutSite_ReturnsFormattedString()
    {
        var siteEntity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Dwarves", Icon = "civilization" };
        var civEntity = new Entity([], _mockWorld.Object) { Id = 2, Name = "Mountain Home", Icon = "civilization" };
        var undergroundRegion = new UndergroundRegion([], _mockWorld.Object) { Id = 4, Name = "Cavern", Icon = "cave" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(siteEntity);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(civEntity);
        _mockWorld.Setup(w => w.GetUndergroundRegion(4)).Returns(undergroundRegion);

        var properties = new List<Property>
        {
            new Property { Name = "site_entity_id", Value = "1" },
            new Property { Name = "civ_entity_id", Value = "2" },
            new Property { Name = "feature_layer_id", Value = "4" }
        };

        var evt = new EntityBreachFeatureLayer(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Dwarves"));
        Assert.IsTrue(result.Contains("Cavern"));
        Assert.IsFalse(result.Contains(" at "));
    }
}
