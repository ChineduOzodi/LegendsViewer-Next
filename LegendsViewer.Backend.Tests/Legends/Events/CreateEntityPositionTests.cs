using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class CreateEntityPositionTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _hf = null!;
    private Entity _civ = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _hf = new HistoricalFigure
        {
            Id = 1,
            Name = "Test HF",
            Icon = "person"
        };

        _civ = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Civ",
            Icon = "civilization"
        };
        _civ.Honors = [];

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_hf);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_civ);
    }

    [TestMethod]
    public void Constructor_WithBasicProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "civ", Value = "1" },
            new Property { Name = "position", Value = "King" }
        };

        // Act
        var createEntityPosition = new CreateEntityPosition(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(createEntityPosition);
        Assert.AreEqual(_hf, createEntityPosition.HistoricalFigure);
        Assert.AreEqual(_civ, createEntityPosition.Civ);
        Assert.AreEqual("King", createEntityPosition.Position);
    }

    [TestMethod]
    public void Constructor_WithReason_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "civ", Value = "1" },
            new Property { Name = "reason", Value = "force_of_argument" }
        };

        // Act
        var createEntityPosition = new CreateEntityPosition(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(ReasonForCreatingEntity.ForceOfArgument, createEntityPosition.Reason);
    }

    [TestMethod]
    public void Constructor_AddsEventToHistoricalFigure()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" }
        };
        var initialEventCount = _hf.Events.Count;

        // Act
        var createEntityPosition = new CreateEntityPosition(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _hf.Events.Count);
    }

    [TestMethod]
    public void Print_WithForceOfArgument_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "civ", Value = "1" },
            new Property { Name = "position", Value = "King" },
            new Property { Name = "reason", Value = "force_of_argument" }
        };
        var createEntityPosition = new CreateEntityPosition(properties, _mockWorld.Object);

        // Act
        var result = createEntityPosition.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("created the position"));
        Assert.IsTrue(result.Contains("force of argument"));
    }

    [TestMethod]
    public void Print_WithThreatOfViolence_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "civ", Value = "1" },
            new Property { Name = "position", Value = "General" },
            new Property { Name = "reason", Value = "threat_of_violence" }
        };
        var createEntityPosition = new CreateEntityPosition(properties, _mockWorld.Object);

        // Act
        var result = createEntityPosition.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("compelled"));
        Assert.IsTrue(result.Contains("threats of violence"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "histfig", Value = "1" },
            new Property { Name = "civ", Value = "1" },
            new Property { Name = "position", Value = "King" }
        };
        var createEntityPosition = new CreateEntityPosition(properties, _mockWorld.Object);

        // Act
        var result = createEntityPosition.Print(link: false);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }
}
