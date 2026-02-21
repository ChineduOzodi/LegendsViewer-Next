using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfPreachTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        var hf = new HistoricalFigure { Id = 1, Name = "Preacher", Icon = "person" };
        var e1 = new Entity([], _mockWorld.Object) { Id = 1, Name = "Group A", Icon = "civilization" };
        var e2 = new Entity([], _mockWorld.Object) { Id = 2, Name = "Group B", Icon = "civilization" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(hf);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(e1);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(e2);
    }

    [TestMethod]
    public void Print_ContainsPreachedText()
    {
        var props = new List<Property>
        {
            new() { Name = "speaker_hfid", Value = "1" },
            new() { Name = "entity_1", Value = "1" },
            new() { Name = "entity_2", Value = "2" },
            new() { Name = "topic", Value = "entity 1 should love entity 2" }
        };
        var evt = new HfPreach(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("preached"));
    }
}
