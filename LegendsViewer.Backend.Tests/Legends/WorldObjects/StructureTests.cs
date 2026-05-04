using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.WorldObjects;

[TestClass]
public class StructureTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
        
        // Setup Structures list to avoid NullReferenceException
        var structures = new List<Structure>();
        _mockWorld.Setup(w => w.Structures).Returns(structures);
    }

    [TestMethod]
    public void Constructor_WithNameProperty_ParsesCorrectly()
    {
        // Create a proper Site with Coordinates to avoid NullReferenceException
        var props = new List<Property>
        {
            new Property { Name = "name", Value = "Iron Hills" },
            new Property { Name = "type", Value = "fortress" }
        };
        
        var site = new Site(props, _mockWorld.Object);

        var structureProps = new List<Property>
        {
            new Property { Name = "name", Value = "The Great Hall" }
        };

        var structure = new Structure(structureProps, _mockWorld.Object, site);

        Assert.IsNotNull(structure);
        Assert.AreEqual("The Great Hall", structure.Name);
    }

    [TestMethod]
    public void ToString_ReturnsName()
    {
        var siteProps = new List<Property>
        {
            new Property { Name = "name", Value = "Test Site" },
            new Property { Name = "type", Value = "town" }
        };
        
        var site = new Site(siteProps, _mockWorld.Object);

        var structureProps = new List<Property>
        {
            new Property { Name = "name", Value = "Test Structure" }
        };

        var structure = new Structure(structureProps, _mockWorld.Object, site);

        Assert.AreEqual("Test Structure", structure.ToString());
    }
}
