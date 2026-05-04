using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.WorldObjects;

[TestClass]
public class ArtifactTests
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
            new Property { Name = "name", Value = "The Crown of Kings" }
        };

        var artifact = new Artifact(props, _mockWorld.Object);

        Assert.IsNotNull(artifact);
        Assert.AreEqual("The Crown of Kings", artifact.Name);
    }

    [TestMethod]
    public void Constructor_WithItem_ParsesItemName()
    {
        var props = new List<Property>
        {
            new Property { Name = "item", Value = "Golden Sword" }
        };

        var artifact = new Artifact(props, _mockWorld.Object);

        Assert.AreEqual("Golden Sword", artifact.Item);
    }

    [TestMethod]
    public void Constructor_WithoutName_SetsDefaultName()
    {
        var props = new List<Property>();

        var artifact = new Artifact(props, _mockWorld.Object);

        Assert.AreEqual("Untitled", artifact.Name);
    }

    [TestMethod]
    public void ToString_ReturnsName()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Artifact" }
        };

        var artifact = new Artifact(props, _mockWorld.Object);

        Assert.AreEqual("Test Artifact", artifact.ToString());
    }

    [TestMethod]
    public void ToLink_WithLink_ReturnsHtml()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Artifact" },
            new Property { Name = "item_type", Value = "Weapon" }
        };

        var artifact = new Artifact(props, _mockWorld.Object);

        var result = artifact.ToLink(link: true);

        Assert.IsTrue(result.Contains("artifact") || result.Contains("anchor"));
    }
}
