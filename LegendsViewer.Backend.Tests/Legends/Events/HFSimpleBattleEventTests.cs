using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfSimpleBattleEventTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        var hf1 = new HistoricalFigure { Id = 1, Name = "Warrior A", Icon = "person" };
        var hf2 = new HistoricalFigure { Id = 2, Name = "Warrior B", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(hf1);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(hf2);
    }

    [TestMethod]
    public void Print_ContainsAttackedText()
    {
        var props = new List<Property>
        {
            new() { Name = "group_1_hfid", Value = "1" },
            new() { Name = "group_2_hfid", Value = "2" },
            new() { Name = "subtype", Value = "attacked" }
        };
        var evt = new HfSimpleBattleEvent(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("attacked"));
    }
}
