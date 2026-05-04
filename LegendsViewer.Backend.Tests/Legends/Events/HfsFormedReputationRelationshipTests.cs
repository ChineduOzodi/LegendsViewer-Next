using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfsFormedReputationRelationshipTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _hf1 = null!;
    private HistoricalFigure _hf2 = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        _hf1 = new HistoricalFigure { Id = 1, Name = "HF1", Icon = "person" };
        _hf2 = new HistoricalFigure { Id = 2, Name = "HF2", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_hf1);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_hf2);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "hfid1", Value = "1" },
            new Property { Name = "hfid2", Value = "2" }
        };
        var evt = new HfsFormedReputationRelationship(props, _mockWorld.Object);
        Assert.IsNotNull(evt);
        Assert.AreEqual(_hf1, evt.HistoricalFigure1);
        Assert.AreEqual(_hf2, evt.HistoricalFigure2);
    }

    [TestMethod]
    public void Print_ContainsRelationshipText()
    {
        var props = new List<Property>
        {
            new Property { Name = "hfid1", Value = "1" },
            new Property { Name = "hfid2", Value = "2" },
            new Property { Name = "hf_rep_2_of_1", Value = "buddy" }
        };
        var evt = new HfsFormedReputationRelationship(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("friendship"));
    }
}
