using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class MusicalFormCreatedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _historicalFigure = null!;
    private Site _site = null!;
    private MusicalForm _musicalForm = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Fortress",
            Icon = "fortress"
        };

        _musicalForm = new MusicalForm([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Song of Storms"
        };

        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Composer",
            Icon = "person"
        };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
        _mockWorld.Setup(w => w.GetMusicalForm(1)).Returns(_musicalForm);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "form_id", Value = "1" }
        };

        // Act
        var musicalForm = new MusicalFormCreated(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_historicalFigure, musicalForm.HistoricalFigure);
        Assert.AreEqual(_site, musicalForm.Site);
        Assert.AreEqual(LegendsViewer.Backend.Legends.Enums.FormType.Musical, musicalForm.FormType);
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
        var musicalForm = new MusicalFormCreated(properties, _mockWorld.Object);

        // Act
        var result = musicalForm.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Composer"));
        Assert.IsTrue(result.Contains("Song of Storms"));
        Assert.IsTrue(result.Contains("was created by"));
        Assert.IsTrue(result.Contains("Test Fortress"));
    }
}
