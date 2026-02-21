using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfConvictedTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        var hf = new HistoricalFigure { Id = 1, Name = "Prisoner", Icon = "person" };
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Court", Icon = "civilization" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(hf);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);
    }

    [TestMethod]
    public void Print_ContainsConvictedText()
    {
        var props = new List<Property>
        {
            new() { Name = "convicted_hfid", Value = "1" },
            new() { Name = "convicter_enid", Value = "1" },
            new() { Name = "crime", Value = "theft" }
        };
        var evt = new HfConvicted(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("convicted"));
    }
}
