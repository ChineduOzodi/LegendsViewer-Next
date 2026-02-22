using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.WorldObjects;

[TestClass]
public class EntityTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
    }

    [TestMethod]
    public void Constructor_WithNameProperty_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Mountainhome" }
        };

        var entity = new Entity(props, _mockWorld.Object);

        Assert.IsNotNull(entity);
        Assert.AreEqual("Mountainhome", entity.Name);
    }

    [TestMethod]
    public void Constructor_WithoutName_SetsDefaultName()
    {
        var props = new List<Property>();

        var entity = new Entity(props, _mockWorld.Object);

        Assert.IsNotNull(entity.Name);
    }

    [TestMethod]
    public void GetTitle_ReturnsType()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Entity" }
        };

        var entity = new Entity(props, _mockWorld.Object);

        var result = entity.GetTitle();

        Assert.IsNotNull(result);
    }
}
