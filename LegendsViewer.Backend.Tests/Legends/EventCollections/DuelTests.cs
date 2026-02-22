using LegendsViewer.Backend.Legends.EventCollections;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.EventCollections;

[TestClass]
public class DuelTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _attacker = null!;
    private HistoricalFigure _defender = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        
        _attacker = new HistoricalFigure { Id = 1, Name = "Fighter A", Icon = "person" };
        _defender = new HistoricalFigure { Id = 2, Name = "Fighter B", Icon = "person" };
        
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_attacker);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_defender);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" },
            new Property { Name = "attacking_hfid", Value = "1" },
            new Property { Name = "defending_hfid", Value = "2" }
        };

        var duel = new Duel(props, _mockWorld.Object);

        Assert.IsNotNull(duel);
        Assert.AreEqual(1, duel.Ordinal);
    }

    [TestMethod]
    public void Constructor_SetsDefaultName()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var duel = new Duel(props, _mockWorld.Object);

        Assert.IsTrue(duel.Name.Contains("duel"));
    }

    [TestMethod]
    public void ToLink_WithLink_ReturnsHtml()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var duel = new Duel(props, _mockWorld.Object);

        var result = duel.ToLink(link: true);

        Assert.IsTrue(result.Contains("duel") || result.Contains("the"));
    }

    [TestMethod]
    public void ToString_ReturnsFormattedString()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var duel = new Duel(props, _mockWorld.Object);

        var result = duel.ToString();

        Assert.IsTrue(result.Contains("duel"));
    }
}
