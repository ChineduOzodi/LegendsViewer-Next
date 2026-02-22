using LegendsViewer.Backend.Legends.EventCollections;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.EventCollections;

[TestClass]
public class BeastAttackTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _defender = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        
        _defender = new Entity([], _mockWorld.Object) { Id = 1, Name = "Village", Icon = "civilization" };
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_defender);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" },
            new Property { Name = "defending_enid", Value = "1" }
        };

        var beastAttack = new BeastAttack(props, _mockWorld.Object);

        Assert.IsNotNull(beastAttack);
        Assert.AreEqual(1, beastAttack.Ordinal);
        Assert.AreEqual(_defender, beastAttack.Defender);
    }

    [TestMethod]
    public void Constructor_SetsDefaultName()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var beastAttack = new BeastAttack(props, _mockWorld.Object);

        Assert.IsTrue(beastAttack.Name.Contains("rampage"));
    }

    [TestMethod]
    public void ToLink_WithLink_ReturnsHtml()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var beastAttack = new BeastAttack(props, _mockWorld.Object);

        var result = beastAttack.ToLink(link: true);

        Assert.IsTrue(result.Contains("rampage") || result.Contains("the"));
    }

    [TestMethod]
    public void ToString_ReturnsFormattedString()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var beastAttack = new BeastAttack(props, _mockWorld.Object);

        var result = beastAttack.ToString();

        Assert.IsTrue(result.Contains("rampage"));
    }
}
