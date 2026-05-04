using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class EntitySearchedSiteTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var properties = new List<Property>
        {
            new Property { Name = "searcher_civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "2" },
            new Property { Name = "result", Value = "found treasure" }
        };

        var evt = new EntitySearchedSite(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual("found treasure", evt.Result);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        var searcherCiv = new Entity([], _mockWorld.Object) { Id = 1, Name = "Adventurers", Icon = "civilization" };
        var site = new Site([], _mockWorld.Object) { Id = 2, Name = "Ancient Ruins", Icon = "location" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(searcherCiv);
        _mockWorld.Setup(w => w.GetSite(2)).Returns(site);

        var properties = new List<Property>
        {
            new Property { Name = "searcher_civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "2" },
            new Property { Name = "result", Value = "found treasure" }
        };

        var evt = new EntitySearchedSite(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Adventurers"));
        Assert.IsTrue(result.Contains("Ancient Ruins"));
        Assert.IsTrue(result.Contains("searched"));
        Assert.IsTrue(result.Contains("found treasure"));
    }
}
