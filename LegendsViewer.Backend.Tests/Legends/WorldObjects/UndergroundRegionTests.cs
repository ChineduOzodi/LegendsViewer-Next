using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.WorldObjects;

[TestClass]
public class UndergroundRegionTests
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
            new Property { Name = "name", Value = "Deep Caverns" }
        };

        var region = new UndergroundRegion(props, _mockWorld.Object);

        Assert.IsNotNull(region);
    }

    [TestMethod]
    public void ToString_ReturnsDescription()
    {
        var props = new List<Property>();

        var region = new UndergroundRegion(props, _mockWorld.Object);

        Assert.IsNotNull(region.ToString());
    }
}
