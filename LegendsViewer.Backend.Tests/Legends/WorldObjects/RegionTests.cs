using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.WorldObjects;

[TestClass]
public class RegionTests
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
            new Property { Name = "name", Value = "The Great Forest" }
        };

        var region = new WorldRegion(props, _mockWorld.Object);

        Assert.IsNotNull(region);
        Assert.AreEqual("The Great Forest", region.Name);
    }

    [TestMethod]
    public void ToString_ReturnsName()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Region" }
        };

        var region = new WorldRegion(props, _mockWorld.Object);

        Assert.AreEqual("Test Region", region.ToString());
    }

    [TestMethod]
    public void ToLink_WithLink_ReturnsHtml()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Region" }
        };

        var region = new WorldRegion(props, _mockWorld.Object);

        var result = region.ToLink(link: true);

        Assert.IsTrue(result.Contains("region") || result.Contains("anchor"));
    }
}
