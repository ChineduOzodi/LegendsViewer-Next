using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.WorldObjects;

[TestClass]
public class HistoricalFigureTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
    }

    [TestMethod]
    public void Constructor_WithNameProperty_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Thorin Oakenshield" }
        };

        var hf = new HistoricalFigure(props, _mockWorld.Object);

        Assert.IsNotNull(hf);
        Assert.AreEqual("Thorin Oakenshield", hf.Name);
    }

    [TestMethod]
    public void Constructor_SetsDefaultValues()
    {
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Test HF" },
            new Property { Name = "race", Value = "DWARF" }
        };

        var hf = new HistoricalFigure(props, _mockWorld.Object);

        Assert.IsNotNull(hf.Name);
    }
}
