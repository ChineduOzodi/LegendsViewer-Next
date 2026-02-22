using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfRecruitedUnitTypeForEntityTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _hf = null!;
    private Entity _entity = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        
        _hf = new HistoricalFigure { Id = 1, Name = "Recruiter", Icon = "person" };
        _entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Temple", Icon = "civilization" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_hf);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_entity);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "unit_type", Value = "monk" }
        };

        var evt = new HfRecruitedUnitTypeForEntity(props, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual(_hf, evt.HistoricalFigure);
        Assert.AreEqual(_entity, evt.Entity);
        Assert.AreEqual(UnitType.Monk, evt.UnitType);
    }

    [TestMethod]
    public void Print_ContainsRecruitedText()
    {
        var props = new List<Property>
        {
            new() { Name = "hfid", Value = "1" },
            new() { Name = "unit_type", Value = "monk" }
        };
        var evt = new HfRecruitedUnitTypeForEntity(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("recruited"));
    }
}
