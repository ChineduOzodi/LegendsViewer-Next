using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfConvictedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _hf = null!;
    private Entity _entity = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        
        _hf = new HistoricalFigure { Id = 1, Name = "Prisoner", Icon = "person" };
        _entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Court", Icon = "civilization" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_hf);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(new HistoricalFigure { Id = 2, Name = "Framer", Icon = "person" });
        _mockWorld.Setup(w => w.GetHistoricalFigure(3)).Returns(new HistoricalFigure { Id = 3, Name = "Fooled", Icon = "person" });
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_entity);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "convicted_hfid", Value = "1" },
            new Property { Name = "convicter_enid", Value = "1" },
            new Property { Name = "crime", Value = "theft" }
        };

        var evt = new HfConvicted(props, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual(_hf, evt.ConvictedHf);
        Assert.AreEqual(_entity, evt.ConvicterEntity);
        Assert.AreEqual("theft", evt.Crime);
    }

    [TestMethod]
    public void Constructor_WithDeathPenalty_SetsDeathPenalty()
    {
        var props = new List<Property>
        {
            new Property { Name = "convicted_hfid", Value = "1" },
            new Property { Name = "convicter_enid", Value = "1" },
            new Property { Name = "crime", Value = "murder" },
            new Property { Name = "death_penalty", Value = "true" }
        };

        var evt = new HfConvicted(props, _mockWorld.Object);

        Assert.IsTrue(evt.DeathPenalty);
    }

    [TestMethod]
    public void Constructor_WithExiled_SetsExiled()
    {
        var props = new List<Property>
        {
            new Property { Name = "convicted_hfid", Value = "1" },
            new Property { Name = "convicter_enid", Value = "1" },
            new Property { Name = "crime", Value = "theft" },
            new Property { Name = "exiled", Value = "true" }
        };

        var evt = new HfConvicted(props, _mockWorld.Object);

        Assert.IsTrue(evt.Exiled);
    }

    [TestMethod]
    public void Constructor_WithWrongfulConviction_SetsWrongful()
    {
        var props = new List<Property>
        {
            new Property { Name = "convicted_hfid", Value = "1" },
            new Property { Name = "convicter_enid", Value = "1" },
            new Property { Name = "crime", Value = "murder" },
            new Property { Name = "wrongful_conviction", Value = "true" }
        };

        var evt = new HfConvicted(props, _mockWorld.Object);

        Assert.IsTrue(evt.WrongfulConviction);
    }

    [TestMethod]
    public void Print_ContainsConvictedText()
    {
        var props = new List<Property>
        {
            new() { Name = "convicted_hfid", Value = "1" },
            new() { Name = "convicter_enid", Value = "1" },
            new() { Name = "crime", Value = "theft" }
        };
        var evt = new HfConvicted(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("convicted"));
    }

    [TestMethod]
    public void Print_WithDeathPenalty_ContainsDeathSentence()
    {
        var props = new List<Property>
        {
            new Property { Name = "convicted_hfid", Value = "1" },
            new Property { Name = "convicter_enid", Value = "1" },
            new Property { Name = "crime", Value = "murder" },
            new Property { Name = "death_penalty", Value = "true" }
        };
        var evt = new HfConvicted(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("death"));
    }

    [TestMethod]
    public void Print_WithWrongfulConviction_ContainsWrongfully()
    {
        var props = new List<Property>
        {
            new Property { Name = "convicted_hfid", Value = "1" },
            new Property { Name = "convicter_enid", Value = "1" },
            new Property { Name = "crime", Value = "murder" },
            new Property { Name = "wrongful_conviction", Value = "true" }
        };
        var evt = new HfConvicted(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.ToLower().Contains("wrongfully"));
    }
}
