using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class DanceFormCreatedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _hf = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _hf = new HistoricalFigure
        {
            Id = 1,
            Name = "Test HF",
            Icon = "person"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };
        _site.Structures = [];

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_hf);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_SetsFormTypeToDance()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hist_figure_id", Value = "1" }
        };

        // Act
        var danceFormCreated = new DanceFormCreated(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(FormType.Dance, danceFormCreated.FormType);
    }

    [TestMethod]
    public void Constructor_WithBasicProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var danceFormCreated = new DanceFormCreated(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(danceFormCreated);
        Assert.AreEqual(_hf, danceFormCreated.HistoricalFigure);
        Assert.AreEqual(_site, danceFormCreated.Site);
    }

    [TestMethod]
    public void Constructor_AddsEventToHistoricalFigure()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hist_figure_id", Value = "1" }
        };
        var initialEventCount = _hf.Events.Count;

        // Act
        var danceFormCreated = new DanceFormCreated(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _hf.Events.Count);
    }

    [TestMethod]
    public void Print_ReturnsNonEmptyString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hist_figure_id", Value = "1" }
        };
        var danceFormCreated = new DanceFormCreated(properties, _mockWorld.Object);

        // Act
        var result = danceFormCreated.Print(link: true);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hist_figure_id", Value = "1" }
        };
        var danceFormCreated = new DanceFormCreated(properties, _mockWorld.Object);

        // Act
        var result = danceFormCreated.Print(link: false);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }
}
