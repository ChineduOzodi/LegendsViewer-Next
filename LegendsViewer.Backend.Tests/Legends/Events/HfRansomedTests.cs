using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfRansomedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _ransomedHf = null!;
    private HistoricalFigure _ransomerHf = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        _ransomedHf = new HistoricalFigure { Id = 1, Name = "Victim", Icon = "person" };
        _ransomerHf = new HistoricalFigure { Id = 2, Name = "Kidnapper", Icon = "person" };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_ransomedHf);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_ransomerHf);
    }

    [TestMethod]
    public void Constructor_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new() { Name = "ransomed_hfid", Value = "1" },
            new() { Name = "ransomer_hfid", Value = "2" }
        };
        var evt = new HfRansomed(props, _mockWorld.Object);
        Assert.AreEqual(_ransomedHf, evt.RansomedHf);
        Assert.AreEqual(_ransomerHf, evt.RansomerHf);
    }

    [TestMethod]
    public void Print_ContainsRansomedText()
    {
        var props = new List<Property>
        {
            new() { Name = "ransomed_hfid", Value = "1" },
            new() { Name = "ransomer_hfid", Value = "2" }
        };
        var evt = new HfRansomed(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("ransomed"));
    }
}
