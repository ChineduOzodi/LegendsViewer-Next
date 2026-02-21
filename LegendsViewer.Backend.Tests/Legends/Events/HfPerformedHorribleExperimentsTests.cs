using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfPerformedHorribleExperimentsTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        var hf = new HistoricalFigure { Id = 1, Name = "Mad Scientist", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(hf);
    }

    [TestMethod]
    public void Print_ContainsExperimentsText()
    {
        var props = new List<Property>
        {
            new() { Name = "group_hfid", Value = "1" }
        };
        var evt = new HfPerformedHorribleExperiments(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("experiments"));
    }
}
