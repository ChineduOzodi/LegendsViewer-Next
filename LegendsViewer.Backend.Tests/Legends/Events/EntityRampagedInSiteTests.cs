using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class EntityRampagedInSiteTests
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
            new Property { Name = "rampage_civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "2" }
        };

        var evt = new EntityRampagedInSite(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        var rampageCiv = new Entity([], _mockWorld.Object) { Id = 1, Name = "Orcs", Icon = "civilization" };
        var site = new Site([], _mockWorld.Object) { Id = 2, Name = "Fort", Icon = "location" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(rampageCiv);
        _mockWorld.Setup(w => w.GetSite(2)).Returns(site);

        var properties = new List<Property>
        {
            new Property { Name = "rampage_civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "2" }
        };

        var evt = new EntityRampagedInSite(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Orcs"));
        Assert.IsTrue(result.Contains("Fort"));
        Assert.IsTrue(result.Contains("rampaged"));
    }
}
