using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfTravelTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _hf = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        
        _hf = new HistoricalFigure { Id = 1, Name = "Traveler", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_hf);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(new Site([], _mockWorld.Object) { Id = 1, Name = "Town", Icon = "location" });
        _mockWorld.Setup(w => w.GetRegion(1)).Returns(new WorldRegion([], _mockWorld.Object) { Id = 1, Name = "Forest", Icon = "region" });
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "group_hfid", Value = "1" },
            new Property { Name = "site_id", Value = "1" }
        };

        var evt = new HfTravel(props, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual(_hf, evt.HistoricalFigure);
    }

    [TestMethod]
    public void Constructor_WithEscaped_SetsEscaped()
    {
        var props = new List<Property>
        {
            new Property { Name = "group_hfid", Value = "1" },
            new Property { Name = "escape", Value = "true" }
        };

        var evt = new HfTravel(props, _mockWorld.Object);

        Assert.IsTrue(evt.Escaped);
    }

    [TestMethod]
    public void Constructor_WithReturned_SetsReturned()
    {
        var props = new List<Property>
        {
            new Property { Name = "group_hfid", Value = "1" },
            new Property { Name = "return", Value = "true" }
        };

        var evt = new HfTravel(props, _mockWorld.Object);

        Assert.IsTrue(evt.Returned);
    }

    [TestMethod]
    public void Print_ContainsJourneyText()
    {
        var props = new List<Property>
        {
            new() { Name = "group_hfid", Value = "1" }
        };
        var evt = new HfTravel(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("journey") || result.Contains("Traveler"));
    }
}
