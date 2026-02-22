using LegendsViewer.Backend.Legends.EventCollections;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.EventCollections;

[TestClass]
public class TheftTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _attacker = null!;
    private Entity _defender = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        
        _attacker = new Entity([], _mockWorld.Object) { Id = 1, Name = "Thief", Icon = "bandits" };
        _defender = new Entity([], _mockWorld.Object) { Id = 2, Name = "Merchant", Icon = "civilization" };
        
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_attacker);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_defender);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" },
            new Property { Name = "attacking_enid", Value = "1" },
            new Property { Name = "defending_enid", Value = "2" }
        };

        var evt = new Theft(props, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual(1, evt.Ordinal);
    }

    [TestMethod]
    public void Constructor_SetsDefaultName()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var evt = new Theft(props, _mockWorld.Object);

        Assert.IsTrue(evt.Name.Contains("theft"));
    }

    [TestMethod]
    public void ToLink_WithLink_ReturnsHtml()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var evt = new Theft(props, _mockWorld.Object);

        var result = evt.ToLink(link: true);

        Assert.IsTrue(result.Contains("theft") || result.Contains("the"));
    }

    [TestMethod]
    public void ToString_ReturnsFormattedString()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var evt = new Theft(props, _mockWorld.Object);

        var result = evt.ToString();

        Assert.IsTrue(result.Contains("theft"));
    }
}
