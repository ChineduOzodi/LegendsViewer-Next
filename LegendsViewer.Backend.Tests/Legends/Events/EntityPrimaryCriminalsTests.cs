using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class EntityPrimaryCriminalsTests
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
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "site_id", Value = "2" },
            new Property { Name = "action", Value = "entity_primary_criminals" }
        };

        var evt = new EntityPrimaryCriminals(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual(ActionsForEntities.PrimaryCriminals, evt.Action);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Thieves Guild", Icon = "civilization" };
        var site = new Site([], _mockWorld.Object) { Id = 2, Name = "Fort", Icon = "location" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);
        _mockWorld.Setup(w => w.GetSite(2)).Returns(site);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "site_id", Value = "2" },
            new Property { Name = "action", Value = "entity_primary_criminals" }
        };

        var evt = new EntityPrimaryCriminals(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Thieves Guild"));
        Assert.IsTrue(result.Contains("Fort"));
        Assert.IsTrue(result.Contains("primary criminal"));
    }
}
