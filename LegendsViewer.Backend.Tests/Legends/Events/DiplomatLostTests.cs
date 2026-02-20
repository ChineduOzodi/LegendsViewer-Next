using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class DiplomatLostTests
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
            new Property { Name = "entity", Value = "1" },
            new Property { Name = "site", Value = "2" },
            new Property { Name = "involved", Value = "3" }
        };

        var evt = new DiplomatLost(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Test Entity", Icon = "civilization" };
        var site = new Site([], _mockWorld.Object) { Id = 2, Name = "Test Site", Icon = "location" };
        var involvedEntity = new Entity([], _mockWorld.Object) { Id = 3, Name = "Involved Entity", Icon = "civilization" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);
        _mockWorld.Setup(w => w.GetSite(2)).Returns(site);
        _mockWorld.Setup(w => w.GetEntity(3)).Returns(involvedEntity);

        var properties = new List<Property>
        {
            new Property { Name = "entity", Value = "1" },
            new Property { Name = "site", Value = "2" },
            new Property { Name = "involved", Value = "3" }
        };

        var evt = new DiplomatLost(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Test Entity"));
        Assert.IsTrue(result.Contains("Test Site"));
        Assert.IsTrue(result.Contains("Involved Entity"));
        Assert.IsTrue(result.Contains("lost a diplomat at"));
    }

    [TestMethod]
    public void Print_WithoutLinks_ReturnsFormattedString()
    {
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Test Entity", Icon = "civilization" };
        var site = new Site([], _mockWorld.Object) { Id = 2, Name = "Test Site", Icon = "location" };
        var involvedEntity = new Entity([], _mockWorld.Object) { Id = 3, Name = "Involved Entity", Icon = "civilization" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);
        _mockWorld.Setup(w => w.GetSite(2)).Returns(site);
        _mockWorld.Setup(w => w.GetEntity(3)).Returns(involvedEntity);

        var properties = new List<Property>
        {
            new Property { Name = "entity", Value = "1" },
            new Property { Name = "site", Value = "2" },
            new Property { Name = "involved", Value = "3" }
        };

        var evt = new DiplomatLost(properties, _mockWorld.Object);

        var result = evt.Print(link: false);

        Assert.IsTrue(result.Contains("Test Entity"));
        Assert.IsTrue(result.Contains("Test Site"));
        Assert.IsTrue(result.Contains("Involved Entity"));
    }
}
