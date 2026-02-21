using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class RazedStructureTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _entity = null!;
    private Site _site = null!;
    private Structure _structure = null!;
    private List<Structure> _structuresList = null!;

    [TestInitialize]
    public void Setup()
    {
        _structuresList = [];
        
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        _mockWorld.Setup(w => w.Structures).Returns(_structuresList);

        _entity = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Razing Empire",
            Icon = "civilization"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };

        _structure = new Structure([], _mockWorld.Object, _site)
        {
            LocalId = 42,
            Name = "The Old Tower",
            Type = "TOWER"
        };
        _structuresList.Add(_structure);
        _site.Structures.Add(_structure);

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_entity);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure_id", Value = "42" }
        };

        // Act
        var evt = new RazedStructure(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_entity, evt.Entity);
        Assert.AreEqual(_site, evt.Site);
        Assert.AreEqual(42, evt.StructureId);
        Assert.AreEqual(_structure, evt.Structure);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure_id", Value = "42" }
        };

        // Act
        var evt = new RazedStructure(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Razing Empire"));
        Assert.IsTrue(result.Contains("razed"));
        Assert.IsTrue(result.Contains("The Old Tower"));
        Assert.IsTrue(result.Contains("Test Site"));
    }

    [TestMethod]
    public void Print_WithoutStructure_ReturnsUnknownStructure()
    {
        // Arrange - structure not in site
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "structure_id", Value = "999" }
        };

        // Act
        var evt = new RazedStructure(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("UNKNOWN STRUCTURE"));
    }
}
