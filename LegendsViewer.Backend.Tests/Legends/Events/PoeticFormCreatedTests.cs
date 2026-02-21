using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class PoeticFormCreatedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _historicalFigure = null!;
    private Site _site = null!;
    private PoeticForm _poeticForm = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Poet Bard",
            Icon = "person"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Poetry Hall",
            Type = "Library"
        };

        // Create a minimal PoeticForm
        _poeticForm = new PoeticForm(new List<Property>(), _mockWorld.Object)
        {
            Id = 1,
            Name = "Epic of Legends"
        };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
        _mockWorld.Setup(w => w.GetPoeticForm(1)).Returns(_poeticForm);
    }

    [TestMethod]
    public void Constructor_WithFormId_SetsFormTypeToPoetic()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "form_id", Value = "1" }
        };

        // Act
        var evt = new PoeticFormCreated(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(FormType.Poetic, evt.FormType);
    }

    [TestMethod]
    public void Constructor_WithFormId_SetsArtForm()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "form_id", Value = "1" }
        };

        // Act
        var evt = new PoeticFormCreated(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt.ArtForm);
        Assert.AreEqual(_poeticForm, evt.ArtForm);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "form_id", Value = "1" }
        };

        // Act
        var evt = new PoeticFormCreated(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Epic of Legends"));
        Assert.IsTrue(result.Contains("created by"));
        Assert.IsTrue(result.Contains("Poet Bard"));
    }
}
