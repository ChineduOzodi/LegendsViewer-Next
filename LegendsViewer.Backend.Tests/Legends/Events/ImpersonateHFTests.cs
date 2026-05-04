using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class ImpersonateHfTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _trickster = null!;
    private HistoricalFigure _cover = null!;
    private Entity _target = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        _trickster = new HistoricalFigure { Id = 1, Name = "Impersonator", Icon = "person" };
        _cover = new HistoricalFigure { Id = 2, Name = "Deity", Icon = "person" };
        _target = new Entity([], _mockWorld.Object) { Id = 1, Name = "Village", Icon = "civilization" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_trickster);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_cover);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_target);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "trickster_hfid", Value = "1" },
            new Property { Name = "cover_hfid", Value = "2" },
            new Property { Name = "target_enid", Value = "1" }
        };
        var evt = new ImpersonateHf(props, _mockWorld.Object);
        Assert.IsNotNull(evt);
        Assert.AreEqual(_trickster, evt.Trickster);
        Assert.AreEqual(_cover, evt.Cover);
        Assert.AreEqual(_target, evt.Target);
    }

    [TestMethod]
    public void Print_ContainsImpersonatedText()
    {
        var props = new List<Property>
        {
            new Property { Name = "trickster_hfid", Value = "1" },
            new Property { Name = "cover_hfid", Value = "2" },
            new Property { Name = "target_enid", Value = "1" }
        };
        var evt = new ImpersonateHf(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("fooled"));
    }
}
