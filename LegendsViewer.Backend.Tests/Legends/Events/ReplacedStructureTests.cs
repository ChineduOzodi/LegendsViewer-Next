using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class ReplacedStructureTests
{
    private Mock<IWorld> _mockWorld = null!;
    private Entity _civ = null!;
    private Entity _siteEntity = null!;
    private Site _site = null!;
    private Structure _oldStructure = null!;
    private Structure _newStructure = null!;
    private List<Structure> _structuresList = null!;

    [TestInitialize]
    public void Setup()
    {
        _structuresList = [];
        
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        _mockWorld.Setup(w => w.Structures).Returns(_structuresList);

        _civ = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Kingdom",
            Icon = "civilization"
        };

        _siteEntity = new Entity([], _mockWorld.Object)
        {
            Id = 2,
            Name = "Site Faction",
            Icon = "civilization"
        };

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "CITY"
        };

        _oldStructure = new Structure([], _mockWorld.Object, _site)
        {
            LocalId = 10,
            Name = "Old Hall",
            Type = "MEAD_HALL"
        };

        _newStructure = new Structure([], _mockWorld.Object, _site)
        {
            LocalId = 20,
            Name = "New Tower",
            Type = "TOWER"
        };

        _structuresList.Add(_oldStructure);
        _structuresList.Add(_newStructure);
        _site.Structures.Add(_oldStructure);
        _site.Structures.Add(_newStructure);

        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_civ);
        _mockWorld.Setup(w => w.GetEntity(2)).Returns(_siteEntity);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_civ_id", Value = "2" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "old_ab_id", Value = "10" },
            new Property { Name = "new_ab_id", Value = "20" }
        };

        // Act
        var evt = new ReplacedStructure(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(evt);
        Assert.AreEqual(_civ, evt.Civ);
        Assert.AreEqual(_siteEntity, evt.SiteEntity);
        Assert.AreEqual(_site, evt.Site);
        Assert.AreEqual(10, evt.OldStructureId);
        Assert.AreEqual(20, evt.NewStructureId);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "civ_id", Value = "1" },
            new Property { Name = "site_civ_id", Value = "2" },
            new Property { Name = "site_id", Value = "1" },
            new Property { Name = "old_ab_id", Value = "10" },
            new Property { Name = "new_ab_id", Value = "20" }
        };

        // Act
        var evt = new ReplacedStructure(properties, _mockWorld.Object);
        var result = evt.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("replaced"));
        Assert.IsTrue(result.Contains("Old Hall"));
        Assert.IsTrue(result.Contains("New Tower"));
        Assert.IsTrue(result.Contains("Test Site"));
    }
}
