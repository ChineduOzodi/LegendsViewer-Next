using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfViewedArtifactTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        var hf = new HistoricalFigure { Id = 1, Name = "Viewer", Icon = "person" };
        var art = new Artifact([], _mockWorld.Object) { Id = 1, Name = "Crown" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(hf);
        _mockWorld.Setup(w => w.GetArtifact(1)).Returns(art);
    }

    [TestMethod]
    public void Print_ContainsViewedText()
    {
        var props = new List<Property>
        {
            new() { Name = "hist_fig_id", Value = "1" },
            new() { Name = "artifact_id", Value = "1" }
        };
        var evt = new HfViewedArtifact(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("viewed"));
    }
}
