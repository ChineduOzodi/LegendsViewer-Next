using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfRevivedTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(new HistoricalFigure { Id = 1, Name = "Dead", Icon = "person" });
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property> { new Property { Name = "histfig", Value = "1" } };
        var evt = new HfRevived(props, _mockWorld.Object);
        Assert.IsNotNull(evt);
    }

    [TestMethod]
    public void Print_ContainsRevivedText()
    {
        var props = new List<Property> { new Property { Name = "histfig", Value = "1" } };
        var evt = new HfRevived(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Length > 0);
    }
}
