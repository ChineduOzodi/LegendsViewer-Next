using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class WrittenContentComposedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _author = null!;
    private Entity _civ = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _author = new HistoricalFigure
        {
            Id = 1,
            Name = "Famous Author",
            Icon = "person"
        };

        _civ = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Kingdom",
            Icon = "civilization"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Library",
            Type = "TOWER"
        };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_author);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_civ);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "2" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "wc_id", Value = "1" }
        };

        // Act
        var evt = new WrittenContentComposed(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_author, evt.HistoricalFigure);
        Assert.AreEqual(_civ, evt.Civ);
        Assert.AreEqual(_site, evt.Site);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsAuthoredString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "2" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "hist_figure_id", Value = "1" },
            new Property { Name = "wc_id", Value = "1" }
        };

        // Act - this event doesn't require WrittenContent to exist for basic Print
        var evt = new WrittenContentComposed(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("authored"));
    }
}
