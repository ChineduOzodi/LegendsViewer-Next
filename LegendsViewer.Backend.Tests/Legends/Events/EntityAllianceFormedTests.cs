using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class EntityAllianceFormedTests
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
            new Property { Name = "initiating_enid", Value = "1" },
            new Property { Name = "joining_enid", Value = "2" }
        };

        var evt = new EntityAllianceFormed(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        var initiatingEntity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Kingdom", Icon = "civilization" };
        var joiningEntity = new Entity([], _mockWorld.Object) { Id = 2, Name = "Dwarf Fortress", Icon = "civilization" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(initiatingEntity);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(joiningEntity);

        var properties = new List<Property>
        {
            new Property { Name = "initiating_enid", Value = "1" },
            new Property { Name = "joining_enid", Value = "2" }
        };

        var evt = new EntityAllianceFormed(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Kingdom"));
        Assert.IsTrue(result.Contains("Dwarf Fortress"));
        Assert.IsTrue(result.Contains("swore to support"));
        Assert.IsTrue(result.Contains("in war if the latter did likewise"));
    }

    [TestMethod]
    public void Print_WithoutLinks_ReturnsFormattedString()
    {
        var initiatingEntity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Kingdom", Icon = "civilization" };
        var joiningEntity = new Entity([], _mockWorld.Object) { Id = 2, Name = "Dwarf Fortress", Icon = "civilization" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(initiatingEntity);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(joiningEntity);

        var properties = new List<Property>
        {
            new Property { Name = "initiating_enid", Value = "1" },
            new Property { Name = "joining_enid", Value = "2" }
        };

        var evt = new EntityAllianceFormed(properties, _mockWorld.Object);

        var result = evt.Print(link: false);

        Assert.IsTrue(result.Contains("Kingdom"));
        Assert.IsTrue(result.Contains("Dwarf Fortress"));
    }
}
