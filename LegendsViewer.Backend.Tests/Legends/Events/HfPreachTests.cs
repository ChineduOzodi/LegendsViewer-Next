using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfPreachTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _hf = null!;
    private Entity _entity1 = null!;
    private Entity _entity2 = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        
        _hf = new HistoricalFigure { Id = 1, Name = "Preacher", Icon = "person" };
        _entity1 = new Entity([], _mockWorld.Object) { Id = 1, Name = "Group A", Icon = "civilization" };
        _entity2 = new Entity([], _mockWorld.Object) { Id = 2, Name = "Group B", Icon = "civilization" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_hf);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_entity1);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_entity2);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "speaker_hfid", Value = "1" },
            new Property { Name = "entity_1", Value = "1" },
            new Property { Name = "entity_2", Value = "2" },
            new Property { Name = "topic", Value = "entity 1 should love entity 2" }
        };

        var evt = new HfPreach(props, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual(_hf, evt.SpeakerHf);
        Assert.AreEqual(_entity1, evt.Entity1);
        Assert.AreEqual(_entity2, evt.Entity2);
        Assert.AreEqual(PreachTopic.Entity1ShouldLoveEntity2, evt.Topic);
    }

    [TestMethod]
    public void Constructor_WithSetEntityAgainstEntity2_SetsTopic()
    {
        var props = new List<Property>
        {
            new Property { Name = "speaker_hfid", Value = "1" },
            new Property { Name = "entity_1", Value = "1" },
            new Property { Name = "entity_2", Value = "2" },
            new Property { Name = "topic", Value = "set entity 1 against entity 2" }
        };

        var evt = new HfPreach(props, _mockWorld.Object);

        Assert.AreEqual(PreachTopic.SetEntity1AgainstEntity2, evt.Topic);
    }

    [TestMethod]
    public void Print_ContainsPreachedText()
    {
        var props = new List<Property>
        {
            new() { Name = "speaker_hfid", Value = "1" },
            new() { Name = "entity_1", Value = "1" },
            new() { Name = "entity_2", Value = "2" },
            new() { Name = "topic", Value = "entity 1 should love entity 2" }
        };
        var evt = new HfPreach(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("preached"));
    }
}
