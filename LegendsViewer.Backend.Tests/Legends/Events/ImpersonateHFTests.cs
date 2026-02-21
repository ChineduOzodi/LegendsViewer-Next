using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class ImpersonateHFTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        var hf1 = new HistoricalFigure { Id = 1, Name = "Trickster", Icon = "person" };
        var hf2 = new HistoricalFigure { Id = 2, Name = "Deity", Icon = "person" };
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Temple", Icon = "civilization" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(hf1);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(hf2);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);
    }

    [TestMethod]
    public void Print_ContainsFooledText()
    {
        var props = new List<Property>
        {
            new() { Name = "trickster_hfid", Value = "1" },
            new() { Name = "cover_hfid", Value = "2" },
            new() { Name = "target_enid", Value = "1" }
        };
        var evt = new ImpersonateHf(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("fooled"));
    }
}
