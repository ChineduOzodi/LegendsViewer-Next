using LegendsViewer.Backend.Legends.EventCollections;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.EventCollections;

[TestClass]
public class InsurrectionTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _target = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        
        _target = new Entity([], _mockWorld.Object) { Id = 1, Name = "Rebels", Icon = "civilization" };
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_target);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" },
            new Property { Name = "target_enid", Value = "1" }
        };

        var evt = new Insurrection(props, _mockWorld.Object);

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

        var evt = new Insurrection(props, _mockWorld.Object);

        Assert.IsTrue(evt.Name.Contains("insurrection"));
    }

    [TestMethod]
    public void ToLink_WithLink_ReturnsHtml()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var evt = new Insurrection(props, _mockWorld.Object);

        var result = evt.ToLink(link: true);

        Assert.IsTrue(result.Contains("insurrection") || result.Contains("the"));
    }

    [TestMethod]
    public void ToString_ReturnsFormattedString()
    {
        var props = new List<Property>
        {
            new Property { Name = "ordinal", Value = "1" }
        };

        var evt = new Insurrection(props, _mockWorld.Object);

        var result = evt.ToString();

        Assert.IsTrue(result.Contains("insurrection"));
    }
}
