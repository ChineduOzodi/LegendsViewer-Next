using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfsFormedIntrigueRelationshipTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        var hf1 = new HistoricalFigure { Id = 1, Name = "Corruptor", Icon = "person" };
        var hf2 = new HistoricalFigure { Id = 2, Name = "Target", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(hf1);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(hf2);
    }

    [TestMethod]
    public void Print_ContainsRelationshipText()
    {
        var props = new List<Property>
        {
            new() { Name = "corruptor_hfid", Value = "1" },
            new() { Name = "target_hfid", Value = "2" },
            new() { Name = "action", Value = "bribe_official" }
        };
        var evt = new HfsFormedIntrigueRelationship(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("bribe"));
    }
}
