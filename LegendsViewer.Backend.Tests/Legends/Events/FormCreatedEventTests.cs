using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class FormCreatedEventTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _historicalFigure = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        // Create historical figure
        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Bard Smith",
            Icon = "person"
        };

        // Create site
        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "The Great Hall"
        };

        // Setup mock world
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "form_id", Value = "1" },
            new Property { Name = "reason", Value = "test" }
        };

        // Act
        var formCreatedEvent = new FormCreatedEvent(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_historicalFigure, formCreatedEvent.HistoricalFigure);
        Assert.AreEqual(_site, formCreatedEvent.Site);
        Assert.AreEqual("1", formCreatedEvent.FormId);
        Assert.AreEqual("test", formCreatedEvent.Reason);
    }

    [TestMethod]
    public void Print_WithLinkTrue_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "form_id", Value = "1" }
        };
        var formCreatedEvent = new FormCreatedEvent(properties, _mockWorld.Object);

        // Act
        var result = formCreatedEvent.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Bard Smith"));
        Assert.IsTrue(result.Contains("was created by"));
    }

    [TestMethod]
    public void Print_WithSite_IncludesSiteInOutput()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };
        var formCreatedEvent = new FormCreatedEvent(properties, _mockWorld.Object);

        // Act
        var result = formCreatedEvent.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("The Great Hall"));
    }

    [TestMethod]
    public void Constructor_AddsEventToHistoricalFigure()
    {
        // Arrange
        var initialEventCount = _historicalFigure.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "hist_figure_id", Value = "1" }
        };

        // Act
        var formCreatedEvent = new FormCreatedEvent(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _historicalFigure.Events.Count);
    }

    [TestMethod]
    public void Constructor_WithGlorifyHf_AddsEventToGlorifiedHf()
    {
        // Arrange
        var glorifiedHf = new HistoricalFigure
        {
            Id = 2,
            Name = "Hero of Ages",
            Icon = "person"
        };
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(glorifiedHf);

        var initialEventCount = glorifiedHf.Events.Count;

        var properties = new List<Property>
        {
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "reason", Value = "glorify hf" },
            new Property { Name = "reason_id", Value = "2" }
        };

        // Act
        var formCreatedEvent = new FormCreatedEvent(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(glorifiedHf, formCreatedEvent.GlorifiedHf);
        Assert.AreEqual(initialEventCount + 1, glorifiedHf.Events.Count);
    }
}
