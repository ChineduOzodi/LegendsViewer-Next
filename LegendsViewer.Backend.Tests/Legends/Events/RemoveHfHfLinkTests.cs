using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class RemoveHfHfLinkTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _historicalFigure = null!;
    private HistoricalFigure _targetFigure = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Test Figure",
            Icon = "person"
        };

        _targetFigure = new HistoricalFigure
        {
            Id = 2,
            Name = "Target Figure",
            Icon = "person"
        };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_targetFigure);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "former_spouse" }
        };

        // Act
        var evt = new RemoveHfHfLink(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_historicalFigure, evt.HistoricalFigure);
        Assert.AreEqual(_targetFigure, evt.HistoricalFigureTarget);
        Assert.AreEqual(HistoricalFigureLinkType.FormerSpouse, evt.LinkType);
    }

    [TestMethod]
    public void Print_WithFormerSpouse_ReturnsDivorcedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "former_spouse" }
        };

        // Act
        var evt = new RemoveHfHfLink(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Test Figure"));
        Assert.IsTrue(result.Contains("divorced"));
        Assert.IsTrue(result.Contains("Target Figure"));
    }

    [TestMethod]
    public void Print_WithFormerApprentice_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "former_apprentice" }
        };

        // Act
        var evt = new RemoveHfHfLink(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert - output contains "ceased being"
        Assert.IsTrue(result.Contains("ceased"));
    }
}
