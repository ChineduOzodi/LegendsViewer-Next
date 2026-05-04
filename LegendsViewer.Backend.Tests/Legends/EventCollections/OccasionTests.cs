using LegendsViewer.Backend.Legends.EventCollections;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.EventCollections;

[TestClass]
public class OccasionTests
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
            new Property { Name = "ordinal", Value = "1" },
            new Property { Name = "civ_id", Value = "1" }
        };

        var occasion = new Occasion(props, _mockWorld.Object);

        Assert.IsNotNull(occasion);
        Assert.AreEqual(1, occasion.Ordinal);
    }

    [TestMethod]
    public void Constructor_SetsDefaultName()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var occasion = new Occasion(props, _mockWorld.Object);

        Assert.IsTrue(occasion.Name.Contains("occasion"));
    }

    [TestMethod]
    public void ToLink_WithLink_ReturnsHtml()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var occasion = new Occasion(props, _mockWorld.Object);

        var result = occasion.ToLink(link: true);

        Assert.IsTrue(result.Contains("occasion") || result.Contains("the"));
    }

    [TestMethod]
    public void ToString_ReturnsFormattedString()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var occasion = new Occasion(props, _mockWorld.Object);

        var result = occasion.ToString();

        Assert.IsTrue(result.Contains("occasion"));
    }
}
