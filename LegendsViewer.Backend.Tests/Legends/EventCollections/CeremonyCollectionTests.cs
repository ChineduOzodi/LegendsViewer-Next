using LegendsViewer.Backend.Legends.EventCollections;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.EventCollections;

[TestClass]
public class CeremonyCollectionTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "5" }
        };

        var ceremony = new CeremonyCollection(props, _mockWorld.Object);

        Assert.IsNotNull(ceremony);
        Assert.AreEqual(5, ceremony.Ordinal);
    }

    [TestMethod]
    public void Constructor_SetsDefaultName()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var ceremony = new CeremonyCollection(props, _mockWorld.Object);

        Assert.IsTrue(ceremony.Name.Contains("ceremony"));
        Assert.IsTrue(ceremony.Icon.Contains("human-greeting"));
    }

    [TestMethod]
    public void ToLink_WithLink_ReturnsHtml()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var ceremony = new CeremonyCollection(props, _mockWorld.Object);

        var result = ceremony.ToLink(link: true);

        Assert.IsTrue(result.Contains("ceremony") || result.Contains("the"));
    }

    [TestMethod]
    public void ToString_ReturnsFormattedString()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var ceremony = new CeremonyCollection(props, _mockWorld.Object);

        var result = ceremony.ToString();

        Assert.IsTrue(result.Contains("ceremony"));
    }
}
