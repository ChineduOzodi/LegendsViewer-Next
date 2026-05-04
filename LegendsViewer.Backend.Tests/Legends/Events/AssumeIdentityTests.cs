using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class AssumeIdentityTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _trickster = null!;
    private Entity _target = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _trickster = new HistoricalFigure
        {
            Id = 1,
            Name = "Trickster",
            Icon = "person"
        };

        _target = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Target Entity",
            Icon = "civilization"
        };
        _target.Honors = [];

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_trickster);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_target);
        _mockWorld.Setup(w => w.GetCreatureInfo(It.IsAny<string>())).Returns(new CreatureInfo("HUMAN"));
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "trickster_hfid", Value = "1" },
            new Property { Name = "identity_id", Value = "1" },
            new Property { Name = "target_enid", Value = "1" }
        };

        // Act
        var assumeIdentity = new AssumeIdentity(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(assumeIdentity);
        Assert.AreEqual(_trickster, assumeIdentity.Trickster);
        Assert.AreEqual(_target, assumeIdentity.Target);
        Assert.AreEqual(1, assumeIdentity.IdentityId);
    }

    [TestMethod]
    public void Constructor_WithIdentityName_CreatesIdentity()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "trickster_hfid", Value = "1" },
            new Property { Name = "identity_name", Value = "Fake Name" },
            new Property { Name = "identity_race", Value = "HUMAN" },
            new Property { Name = "identity_caste", Value = "MALE" }
        };

        // Act
        var assumeIdentity = new AssumeIdentity(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(assumeIdentity.Identity);
        Assert.AreEqual("Fake Name", assumeIdentity.Identity.Name);
    }

    [TestMethod]
    public void Constructor_AddsEventToTrickster()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "trickster_hfid", Value = "1" }
        };
        var initialEventCount = _trickster.Events.Count;

        // Act
        var assumeIdentity = new AssumeIdentity(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _trickster.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToTarget()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "trickster_hfid", Value = "1" },
            new Property { Name = "target_enid", Value = "1" }
        };
        var initialEventCount = _target.Events.Count;

        // Act
        var assumeIdentity = new AssumeIdentity(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _target.Events.Count);
    }

    [TestMethod]
    public void Print_WithTarget_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "trickster_hfid", Value = "1" },
            new Property { Name = "identity_id", Value = "1" },
            new Property { Name = "target_enid", Value = "1" }
        };
        var assumeIdentity = new AssumeIdentity(properties, _mockWorld.Object);

        // Act
        var result = assumeIdentity.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Trickster"));
        Assert.IsTrue(result.Contains("fooled"));
        Assert.IsTrue(result.Contains("Target Entity"));
        Assert.IsTrue(result.Contains("believing"));
    }

    [TestMethod]
    public void Print_WithoutTarget_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "trickster_hfid", Value = "1" },
            new Property { Name = "identity_id", Value = "1" }
        };
        var assumeIdentity = new AssumeIdentity(properties, _mockWorld.Object);

        // Act
        var result = assumeIdentity.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("assumed the identity"));
    }

    [TestMethod]
    public void Print_WithIdentityName_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "trickster_hfid", Value = "1" },
            new Property { Name = "identity_name", Value = "Fake Name" },
            new Property { Name = "identity_race", Value = "HUMAN" }
        };
        var assumeIdentity = new AssumeIdentity(properties, _mockWorld.Object);

        // Act
        var result = assumeIdentity.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Fake Name"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "trickster_hfid", Value = "1" },
            new Property { Name = "identity_id", Value = "1" },
            new Property { Name = "target_enid", Value = "1" }
        };
        var assumeIdentity = new AssumeIdentity(properties, _mockWorld.Object);

        // Act
        var result = assumeIdentity.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("fooled"));
    }
}
