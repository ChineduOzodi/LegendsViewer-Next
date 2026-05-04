using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class EntityLawTests
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
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "hist_figure_id", Value = "2" },
            new Property { Name = "law_add", Value = "harsh" }
        };

        var evt = new EntityLaw(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual(EntityLawType.Harsh, evt.Law);
        Assert.IsTrue(evt.LawLaid);
    }

    [TestMethod]
    public void Print_LawLaid_ReturnsFormattedString()
    {
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Dwarven Kingdom", Icon = "civilization" };
        var hf = new HistoricalFigure { Id = 2, Name = "King", Icon = "person" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(hf);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "hist_figure_id", Value = "2" },
            new Property { Name = "law_add", Value = "harsh" }
        };

        var evt = new EntityLaw(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("King"));
        Assert.IsTrue(result.Contains("Dwarven Kingdom"));
        Assert.IsTrue(result.Contains("laid"));
        Assert.IsTrue(result.Contains("oppressive"));
    }

    [TestMethod]
    public void Print_LawRemoved_ReturnsFormattedString()
    {
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Dwarven Kingdom", Icon = "civilization" };

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);

        var properties = new List<Property>
        {
            new Property { Name = "entity_id", Value = "1" },
            new Property { Name = "law_remove", Value = "harsh" }
        };

        var evt = new EntityLaw(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Dwarven Kingdom"));
        Assert.IsTrue(result.Contains("lifted"));
    }
}
