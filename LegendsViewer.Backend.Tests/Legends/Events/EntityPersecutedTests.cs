using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class EntityPersecutedTests
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
            new Property { Name = "persecutor_hfid", Value = "1" },
            new Property { Name = "persecutor_enid", Value = "2" },
            new Property { Name = "target_enid", Value = "3" },
            new Property { Name = "site_id", Value = "4" }
        };

        var evt = new EntityPersecuted(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
    }

    [TestMethod]
    public void Print_WithExpelledHfs_ReturnsFormattedString()
    {
        var persecutorHf = new HistoricalFigure { Id = 1, Name = "King", Icon = "person" };
        var persecutorEntity = new Entity([], _mockWorld.Object) { Id = 2, Name = "Kingdom", Icon = "civilization" };
        var targetEntity = new Entity([], _mockWorld.Object) { Id = 3, Name = "Elves", Icon = "civilization" };
        var site = new Site([], _mockWorld.Object) { Id = 4, Name = "Forest", Icon = "location" };
        var expelledHf1 = new HistoricalFigure { Id = 5, Name = "Elf1", Icon = "person" };
        var expelledHf2 = new HistoricalFigure { Id = 6, Name = "Elf2", Icon = "person" };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(persecutorHf);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(persecutorEntity);
        _mockWorld.Setup(w => w.GetEntity(3)).Returns(targetEntity);
        _mockWorld.Setup(w => w.GetSite(4)).Returns(site);
        _mockWorld.Setup(w => w.GetHistoricalFigure(5)).Returns(expelledHf1);
        _mockWorld.Setup(w => w.GetHistoricalFigure(6)).Returns(expelledHf2);

        var properties = new List<Property>
        {
            new Property { Name = "persecutor_hfid", Value = "1" },
            new Property { Name = "persecutor_enid", Value = "2" },
            new Property { Name = "target_enid", Value = "3" },
            new Property { Name = "site_id", Value = "4" },
            new Property { Name = "expelled_hfid", Value = "5" },
            new Property { Name = "expelled_hfid", Value = "6" }
        };

        var evt = new EntityPersecuted(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("King"));
        Assert.IsTrue(result.Contains("Elves"));
        Assert.IsTrue(result.Contains("persecuted"));
        Assert.IsTrue(result.Contains("expelled"));
    }

    [TestMethod]
    public void Print_WithShrine_ReturnsFormattedString()
    {
        var persecutorHf = new HistoricalFigure { Id = 1, Name = "King", Icon = "person" };
        var persecutorEntity = new Entity([], _mockWorld.Object) { Id = 2, Name = "Kingdom", Icon = "civilization" };
        var targetEntity = new Entity([], _mockWorld.Object) { Id = 3, Name = "Elves", Icon = "civilization" };
        var site = new Site([], _mockWorld.Object) { Id = 4, Name = "Forest", Icon = "location" };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(persecutorHf);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(persecutorEntity);
        _mockWorld.Setup(w => w.GetEntity(3)).Returns(targetEntity);
        _mockWorld.Setup(w => w.GetSite(4)).Returns(site);

        var properties = new List<Property>
        {
            new Property { Name = "persecutor_hfid", Value = "1" },
            new Property { Name = "persecutor_enid", Value = "2" },
            new Property { Name = "target_enid", Value = "3" },
            new Property { Name = "site_id", Value = "4" },
            new Property { Name = "shrine_amount_destroyed", Value = "3" }
        };

        var evt = new EntityPersecuted(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("sacred sites"));
    }
}
