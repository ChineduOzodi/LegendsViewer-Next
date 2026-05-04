using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfDiedTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        var hf = new HistoricalFigure { Id = 1, Name = "Victim", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(hf);
    }

    [TestMethod]
    public void Print_ContainsDiedText()
    {
        var props = new List<Property>
        {
            new() { Name = "hfid", Value = "1" },
            new() { Name = "cause", Value = "old age" }
        };
        var evt = new HfDied(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("died"));
    }

    [TestMethod]
    public void Constructor_SetsDeathCause()
    {
        var props = new List<Property>
        {
            new() { Name = "hfid", Value = "1" },
            new() { Name = "cause", Value = "old age" }
        };
        var evt = new HfDied(props, _mockWorld.Object);
        Assert.AreEqual(DeathCause.OldAge, evt.Cause);
    }
}
