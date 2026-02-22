using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfsFormedIntrigueRelationshipTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _corruptor = null!;
    private HistoricalFigure _target = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        _corruptor = new HistoricalFigure { Id = 1, Name = "Corruptor", Icon = "person" };
        _target = new HistoricalFigure { Id = 2, Name = "Target", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_corruptor);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_target);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "corruptor_hfid", Value = "1" },
            new Property { Name = "target_hfid", Value = "2" }
        };
        var evt = new HfsFormedIntrigueRelationship(props, _mockWorld.Object);
        Assert.IsNotNull(evt);
        Assert.AreEqual(_corruptor, evt.CorruptorHf);
        Assert.AreEqual(_target, evt.TargetHf);
    }

    [TestMethod]
    public void Print_ContainsIntrigueText()
    {
        var props = new List<Property>
        {
            new Property { Name = "corruptor_hfid", Value = "1" },
            new Property { Name = "target_hfid", Value = "2" },
            new Property { Name = "action", Value = "corrupt_in_place" }
        };
        var evt = new HfsFormedIntrigueRelationship(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("corrupted") || result.Contains("began"));
    }
}
