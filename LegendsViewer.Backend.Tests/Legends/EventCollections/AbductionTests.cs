using LegendsViewer.Backend.Legends.EventCollections;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.EventCollections;

[TestClass]
public class AbductionTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _abductee = null!;
    private Entity _attacker = null!;
    private Entity _defender = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        
        _abductee = new HistoricalFigure { Id = 1, Name = "Victim", Icon = "person" };
        _attacker = new Entity([], _mockWorld.Object) { Id = 1, Name = "Attacker Group", Icon = "civilization" };
        _defender = new Entity([], _mockWorld.Object) { Id = 2, Name = "Defender Group", Icon = "civilization" };
        _site = new Site([], _mockWorld.Object) { Id = 1, Name = "Abduction Site", Icon = "location" };
        
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_abductee);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_attacker);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_defender);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "attacking_enid", Value = "1" },
            new Property { Name = "defending_enid", Value = "2" }
        };

        var evt = new Abduction(props, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual(1, evt.Ordinal);
        Assert.AreEqual(_site, evt.Site);
        Assert.AreEqual(_attacker, evt.Attacker);
        Assert.AreEqual(_defender, evt.Defender);
    }

    [TestMethod]
    public void Constructor_SetsName_WithOrdinal()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "3" }
        };

        var evt = new Abduction(props, _mockWorld.Object);

        Assert.IsTrue(evt.Name.Contains("3rd"));
        Assert.IsTrue(evt.Icon.Contains("map-marker-alert"));
    }

    [TestMethod]
    public void ToString_ReturnsFormattedString()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };

        var evt = new Abduction(props, _mockWorld.Object);

        var result = evt.ToString();
        
        Assert.IsTrue(result.Contains("abduction"));
    }

    [TestMethod]
    public void ToLink_WithLink_ReturnsHtml()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };

        var evt = new Abduction(props, _mockWorld.Object);

        var result = evt.ToLink(link: true);

        Assert.IsTrue(result.Contains("abduction") || result.Contains("the"));
    }
}
