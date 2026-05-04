using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HFWoundedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _woundee = null!;
    private HistoricalFigure _wounder = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        
        _woundee = new HistoricalFigure { Id = 1, Name = "Victim", Icon = "person" };
        _wounder = new HistoricalFigure { Id = 2, Name = "Attacker", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_woundee);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_wounder);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "woundee_hfid", Value = "1" },
            new Property { Name = "wounder_hfid", Value = "2" }
        };

        var evt = new HfWounded(props, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual(_woundee, evt.Woundee);
        Assert.AreEqual(_wounder, evt.Wounder);
    }

    [TestMethod]
    public void Constructor_WithPartLost_SetsPartLost()
    {
        var props = new List<Property>
        {
            new Property { Name = "woundee_hfid", Value = "1" },
            new Property { Name = "part_lost", Value = "true" }
        };

        var evt = new HfWounded(props, _mockWorld.Object);

        Assert.IsTrue(evt.PartLost);
    }

    [TestMethod]
    public void Print_ContainsWoundedText()
    {
        var props = new List<Property>
        {
            new() { Name = "woundee_hfid", Value = "1" }
        };
        var evt = new HfWounded(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("wounded"));
    }
}
