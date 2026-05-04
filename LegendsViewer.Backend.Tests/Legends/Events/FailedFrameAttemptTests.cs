using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class FailedFrameAttemptTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var properties = new List<Property>
        {
            new Property { Name = "target_hfid", Value = "1" },
            new Property { Name = "framer_hfid", Value = "2" },
            new Property { Name = "crime", Value = "treason" }
        };

        var evt = new FailedFrameAttempt(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual("treason", evt.Crime);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        var target = new HistoricalFigure { Id = 1, Name = "King", Icon = "person" };
        var framer = new HistoricalFigure { Id = 2, Name = "Conspirator", Icon = "person" };
        var fooled = new HistoricalFigure { Id = 3, Name = "Judge", Icon = "person" };
        var plotter = new HistoricalFigure { Id = 4, Name = "Mastermind", Icon = "person" };
        var entity = new Entity([], _mockWorld.Object) { Id = 5, Name = "Kingdom", Icon = "civilization" };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(target);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(framer);
        _mockWorld.Setup(w => w.GetHistoricalFigure(3)).Returns(fooled);
        _mockWorld.Setup(w => w.GetHistoricalFigure(4)).Returns(plotter);
        _mockWorld.Setup(w => w.GetEntity(5)).Returns(entity);

        var properties = new List<Property>
        {
            new Property { Name = "target_hfid", Value = "1" },
            new Property { Name = "framer_hfid", Value = "2" },
            new Property { Name = "fooled_hfid", Value = "3" },
            new Property { Name = "plotter_hfid", Value = "4" },
            new Property { Name = "convicter_enid", Value = "5" },
            new Property { Name = "crime", Value = "treason" }
        };

        var evt = new FailedFrameAttempt(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Conspirator"));
        Assert.IsTrue(result.Contains("King"));
        Assert.IsTrue(result.Contains("treason"));
        Assert.IsTrue(result.Contains("frame"));
    }
}
