using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class SabotageTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _saboteur = null!;
    private HistoricalFigure _target = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _saboteur = new HistoricalFigure
        {
            Id = 1,
            Name = "Saboteur",
            Icon = "person"
        };

        _target = new HistoricalFigure
        {
            Id = 2,
            Name = "Target Figure",
            Icon = "person"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_saboteur);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_target);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "saboteur_hfid", Value = "1" },
            new Property { Name = "target_hfid", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var evt = new Sabotage(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_saboteur, evt.SaboteurHf);
        Assert.AreEqual(_target, evt.TargetHf);
        Assert.AreEqual(_site, evt.Site);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "saboteur_hfid", Value = "1" },
            new Property { Name = "target_hfid", Value = "2" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var evt = new Sabotage(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Saboteur"));
        Assert.IsTrue(result.Contains("sabotaged"));
        Assert.IsTrue(result.Contains("Target Figure"));
    }
}
