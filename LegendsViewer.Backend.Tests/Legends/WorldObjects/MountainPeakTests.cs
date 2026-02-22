using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.WorldObjects;

[TestClass]
public class MountainPeakTests
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
            new Property { Name = "name", Value = "Mount Doom" }
        };

        var mountain = new MountainPeak(props, _mockWorld.Object);

        Assert.IsNotNull(mountain);
        Assert.AreEqual("Mount Doom", mountain.Name);
    }

    [TestMethod]
    public void Constructor_IsVolcano_SetsIcon()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Volcano" },
            new Property { Name = "is_volcano", Value = "true" }
        };

        var mountain = new MountainPeak(props, _mockWorld.Object);

        Assert.IsTrue(mountain.IsVolcano);
        Assert.IsTrue(mountain.Icon.Contains("volcano"));
    }

    [TestMethod]
    public void ToString_ReturnsName()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Mountain" }
        };

        var mountain = new MountainPeak(props, _mockWorld.Object);

        Assert.AreEqual("Test Mountain", mountain.ToString());
    }

    [TestMethod]
    public void ToLink_WithLink_ReturnsHtml()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Mountain" }
        };

        var mountain = new MountainPeak(props, _mockWorld.Object);

        var result = mountain.ToLink(link: true);

        Assert.IsTrue(result.Contains("mountain") || result.Contains("anchor"));
    }
}
