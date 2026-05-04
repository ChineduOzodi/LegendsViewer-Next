using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.WorldObjects;

[TestClass]
public class PoeticFormTests
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
            new Property { Name = "name", Value = "Epic Saga" }
        };

        var form = new PoeticForm(props, _mockWorld.Object);

        Assert.IsNotNull(form);
        Assert.AreEqual("Epic Saga", form.Name);
    }

    [TestMethod]
    public void Constructor_SetsDefaultIcon()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Form" }
        };

        var form = new PoeticForm(props, _mockWorld.Object);

        Assert.IsTrue(form.Icon.Contains("voice"));
    }

    [TestMethod]
    public void ToString_ReturnsName()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Form" }
        };

        var form = new PoeticForm(props, _mockWorld.Object);

        Assert.AreEqual("Test Form", form.ToString());
    }

    [TestMethod]
    public void ToLink_WithLink_ReturnsHtml()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Form" }
        };

        var form = new PoeticForm(props, _mockWorld.Object);

        var result = form.ToLink(link: true);

        Assert.IsTrue(result.Contains("poetic") || result.Contains("anchor"));
    }
}
