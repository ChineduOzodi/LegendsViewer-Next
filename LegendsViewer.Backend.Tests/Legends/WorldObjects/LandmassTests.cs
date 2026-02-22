using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.WorldObjects;

[TestClass]
public class LandmassTests
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
            new Property { Name = "name", Value = "Great Island" }
        };

        var landmass = new Landmass(props, _mockWorld.Object);

        Assert.IsNotNull(landmass);
        Assert.AreEqual("Great Island", landmass.Name);
    }

    [TestMethod]
    public void Constructor_SetsDefaultName()
    {
        var props = new List<Property>();

        var landmass = new Landmass(props, _mockWorld.Object);

        Assert.AreEqual("Untitled", landmass.Name);
    }

    [TestMethod]
    public void ToString_ReturnsName()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Landmass" }
        };

        var landmass = new Landmass(props, _mockWorld.Object);

        Assert.AreEqual("Test Landmass", landmass.ToString());
    }

    [TestMethod]
    public void ToLink_WithLink_ReturnsHtml()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Landmass" }
        };

        var landmass = new Landmass(props, _mockWorld.Object);

        var result = landmass.ToLink(link: true);

        Assert.IsTrue(result.Contains("landmass") || result.Contains("anchor"));
    }
}
