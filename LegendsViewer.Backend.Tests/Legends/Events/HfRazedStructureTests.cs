using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfRazedStructureTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _hf = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        _hf = new HistoricalFigure { Id = 1, Name = "Attacker", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_hf);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "attacker_hfid", Value = "1" }
        };
        var evt = new HfRazedStructure(props, _mockWorld.Object);
        Assert.IsNotNull(evt);
    }

    [TestMethod]
    public void Print_ContainsRazedText()
    {
        var props = new List<Property> { new Property { Name = "attacker_hfid", Value = "1" } };
        var evt = new HfRazedStructure(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("razed"));
    }
}
