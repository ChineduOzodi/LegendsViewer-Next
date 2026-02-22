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
    private HistoricalFigure _hf = null!;
    private Artifact _artifact = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        
        _hf = new HistoricalFigure { Id = 1, Name = "Viewer", Icon = "person" };
        _artifact = new Artifact([], _mockWorld.Object) { Id = 1, Name = "Crown", Icon = "artifact" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_hf);
        _mockWorld.Setup(w => w.GetArtifact(1)).Returns(_artifact);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "hist_fig_id", Value = "1" },
            new Property { Name = "artifact_id", Value = "1" }
        };

        var evt = new HfViewedArtifact(props, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual(_hf, evt.HistoricalFigure);
        Assert.AreEqual(_artifact, evt.Artifact);
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
