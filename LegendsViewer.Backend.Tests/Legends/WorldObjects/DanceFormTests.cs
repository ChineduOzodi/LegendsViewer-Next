using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.WorldObjects;

[TestClass]
public class DanceFormTests
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
            new Property { Name = "name", Value = "Festival Dance" }
        };

        var danceForm = new DanceForm(props, _mockWorld.Object);

        Assert.IsNotNull(danceForm);
        Assert.AreEqual("Festival Dance", danceForm.Name);
    }

    [TestMethod]
    public void Constructor_SetsDefaultIcon()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Dance" }
        };

        var danceForm = new DanceForm(props, _mockWorld.Object);

        Assert.IsTrue(danceForm.Icon.Contains("dance"));
    }

    [TestMethod]
    public void ToString_ReturnsName()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Dance" }
        };

        var danceForm = new DanceForm(props, _mockWorld.Object);

        Assert.AreEqual("Test Dance", danceForm.ToString());
    }

    [TestMethod]
    public void ToLink_WithLink_ReturnsHtml()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Dance" }
        };

        var danceForm = new DanceForm(props, _mockWorld.Object);

        var result = danceForm.ToLink(link: true);

        Assert.IsTrue(result.Contains("danceform") || result.Contains("anchor"));
    }
}
