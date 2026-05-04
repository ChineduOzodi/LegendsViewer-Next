using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HolyCityDeclarationTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Site _site = null!;
    private Entity _entity = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        
        _site = new Site([], _mockWorld.Object) { Id = 1, Name = "Holy City", Icon = "location" };
        _entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Sun Temple", Icon = "civilization" };
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_entity);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var props = new List<Property>
        {
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "religion_id", Value = "1" }
        };

        var evt = new HolyCityDeclaration(props, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual(_site, evt.Site);
        Assert.AreEqual(_entity, evt.ReligionEntity);
    }

    [TestMethod]
    public void Print_ContainsHolyCityText()
    {
        var props = new List<Property>
        {
            new() { Name = "site_id", Value = "1" },
            new() { Name = "religion_id", Value = "1" }
        };
        var evt = new HolyCityDeclaration(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("holy city"));
    }
}
