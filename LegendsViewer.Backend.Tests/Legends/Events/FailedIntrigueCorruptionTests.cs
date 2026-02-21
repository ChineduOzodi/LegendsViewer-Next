using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class FailedIntrigueCorruptionTests
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
            new Property { Name = "corruptor_hfid", Value = "1" },
            new Property { Name = "target_hfid", Value = "2" },
            new Property { Name = "action", Value = "bribe_official" },
            new Property { Name = "method", Value = "bribe" }
        };

        var evt = new FailedIntrigueCorruption(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual(IntrigueAction.BribeOfficial, evt.Action);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        var corruptor = new HistoricalFigure { Id = 1, Name = "Schemer", Icon = "person" };
        var target = new HistoricalFigure { Id = 2, Name = "Official", Icon = "person" };
        var site = new Site([], _mockWorld.Object) { Id = 3, Name = "Fort", Icon = "location" };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(corruptor);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(target);
        _mockWorld.Setup(w => w.GetSite(3)).Returns(site);

        var properties = new List<Property>
        {
            new Property { Name = "corruptor_hfid", Value = "1" },
            new Property { Name = "target_hfid", Value = "2" },
            new Property { Name = "site_id", Value = "3" },
            new Property { Name = "action", Value = "bribe_official" },
            new Property { Name = "method", Value = "bribe" }
        };

        var evt = new FailedIntrigueCorruption(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Schemer"));
        Assert.IsTrue(result.Contains("Official"));
        Assert.IsTrue(result.Contains("corrupt"));
        Assert.IsTrue(result.Contains("failed"));
    }
}
