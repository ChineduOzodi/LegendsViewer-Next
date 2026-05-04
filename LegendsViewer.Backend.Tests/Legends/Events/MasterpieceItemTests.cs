using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class MasterpieceItemTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _hf = null!;
    private Entity _entity = null!;
    private Site _site = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        _hf = new HistoricalFigure { Id = 1, Name = "Artisan", Icon = "person" };
        _entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Guild", Icon = "civilization" };
        _site = new Site([], _mockWorld.Object) { Id = 1, Name = "Workshop", Icon = "location" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_hf);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_entity);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "item_type", Value = "weapon" }
        };
        var evt = new MasterpieceItem(props, _mockWorld.Object);
        Assert.IsNotNull(evt);
        Assert.AreEqual(_hf, evt.Maker);
        Assert.AreEqual(_entity, evt.MakerEntity);
        Assert.AreEqual(_site, evt.Site);
    }

    [TestMethod]
    public void Print_ContainsMasterpieceText()
    {
        var props = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "item_type", Value = "weapon" }
        };
        var evt = new MasterpieceItem(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("masterful") || result.Contains("created"));
    }
}
