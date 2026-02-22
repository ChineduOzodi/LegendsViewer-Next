using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.WorldObjects;

[TestClass]
public class ArtFormTests
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
            new Property { Name = "name", Value = "Masterpiece Sculpture" }
        };

        var artForm = new ArtForm(props, _mockWorld.Object);

        Assert.IsNotNull(artForm);
        Assert.AreEqual("Masterpiece Sculpture", artForm.Name);
    }

    [TestMethod]
    public void Constructor_WithDescription_ParsesNameFromDescription()
    {
        var props = new List<Property>
        {
            new Property { Name = "description", Value = "The Golden Statue is a beautiful sculpture" }
        };

        var artForm = new ArtForm(props, _mockWorld.Object);

        Assert.IsNotNull(artForm);
        Assert.AreEqual("The Golden Statue", artForm.Name);
    }

    [TestMethod]
    public void Constructor_WithoutName_SetsDefaultName()
    {
        var props = new List<Property>();

        var artForm = new ArtForm(props, _mockWorld.Object);

        Assert.AreEqual("Untitled", artForm.Name);
    }

    [TestMethod]
    public void ToString_ReturnsName()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Art" }
        };

        var artForm = new ArtForm(props, _mockWorld.Object);

        Assert.AreEqual("Test Art", artForm.ToString());
    }

    [TestMethod]
    public void ToLink_ReturnsName()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test Art" }
        };

        var artForm = new ArtForm(props, _mockWorld.Object);

        Assert.AreEqual("Test Art", artForm.ToLink());
    }
}
