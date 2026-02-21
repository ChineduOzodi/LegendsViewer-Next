using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfProfanedStructureTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        var hf = new HistoricalFigure { Id = 1, Name = "Desecrator", Icon = "person" };
        var site = new Site([], _mockWorld.Object) { Id = 1, Name = "Temple" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(hf);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(site);
    }

    [TestMethod]
    public void Print_ContainsProfanedText()
    {
        var props = new List<Property>
        {
            new() { Name = "hist_fig_id", Value = "1" },
            new() { Name = "site_id", Value = "1" }
        };
        var evt = new HfProfanedStructure(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("profaned"));
    }
}
