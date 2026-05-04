using LegendsViewer.Backend.Legends.EventCollections;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.EventCollections;

[TestClass]
public class CompetitionCollectionTests
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
            new Property { Name = "ordinal", Value = "3" }
        };

        var competition = new CompetitionCollection(props, _mockWorld.Object);

        Assert.IsNotNull(competition);
        Assert.AreEqual(3, competition.Ordinal);
    }

    [TestMethod]
    public void Constructor_SetsDefaultName()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var competition = new CompetitionCollection(props, _mockWorld.Object);

        Assert.IsTrue(competition.Name.Contains("competition"));
        Assert.IsTrue(competition.Icon.Contains("trophy"));
    }

    [TestMethod]
    public void ToLink_WithLink_ReturnsHtml()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var competition = new CompetitionCollection(props, _mockWorld.Object);

        var result = competition.ToLink(link: true);

        Assert.IsTrue(result.Contains("competition") || result.Contains("the"));
    }

    [TestMethod]
    public void ToString_ReturnsFormattedString()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var competition = new CompetitionCollection(props, _mockWorld.Object);

        var result = competition.ToString();

        Assert.IsTrue(result.Contains("competition"));
    }
}
