using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.WorldObjects;

[TestClass]
public class RiverTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        _mockWorld.Setup(w => w.Rivers).Returns(new List<River>());
    }

    [TestMethod]
    public void Constructor_WithNameProperty_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "River of Gold" }
        };

        var river = new River(props, _mockWorld.Object);

        Assert.IsNotNull(river);
        Assert.AreEqual("River of Gold", river.Name);
    }

    [TestMethod]
    public void ToString_ReturnsName()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test River" }
        };

        var river = new River(props, _mockWorld.Object);

        Assert.AreEqual("Test River", river.ToString());
    }

    [TestMethod]
    public void ToLink_WithLink_ReturnsHtml()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test River" }
        };

        var river = new River(props, _mockWorld.Object);

        var result = river.ToLink(link: true);

        Assert.IsTrue(result.Contains("river") || result.Contains("anchor"));
    }
}
