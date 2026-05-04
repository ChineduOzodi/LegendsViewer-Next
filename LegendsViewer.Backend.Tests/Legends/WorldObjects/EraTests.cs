using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.WorldObjects;

[TestClass]
public class EraTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        _mockWorld.Setup(w => w.Eras).Returns(new List<Era>());
    }

    [TestMethod]
    public void Constructor_WithNameProperty_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Age of Legends" }
        };

        var era = new Era(props, _mockWorld.Object);

        Assert.IsNotNull(era);
        Assert.AreEqual("Age of Legends", era.Name);
    }

    [TestMethod]
    public void Constructor_WithStartYear_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "start_year", Value = "100" }
        };

        var era = new Era(props, _mockWorld.Object);

        Assert.AreEqual(100, era.StartYear);
    }

    [TestMethod]
    public void Constructor_SetsDefaultIcon()
    {
        var props = new List<Property>();

        var era = new Era(props, _mockWorld.Object);

        Assert.IsTrue(era.Icon.Contains("time"));
    }

    [TestMethod]
    public void ToString_ReturnsName()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Era" }
        };

        var era = new Era(props, _mockWorld.Object);

        Assert.AreEqual("Test Era", era.ToString());
    }

    [TestMethod]
    public void ToLink_WithLink_ReturnsHtml()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Era" }
        };

        var era = new Era(props, _mockWorld.Object);

        var result = era.ToLink(link: true);

        Assert.IsTrue(result.Contains("era") || result.Contains("anchor"));
    }
}
