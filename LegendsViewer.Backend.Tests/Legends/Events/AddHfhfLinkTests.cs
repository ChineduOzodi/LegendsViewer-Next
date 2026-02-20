using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class AddHfhfLinkTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _historicalFigure = null!;
    private HistoricalFigure _historicalFigureTarget = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        // Setup ParsingErrors mock to avoid NullReferenceException
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _historicalFigure = new HistoricalFigure
        {
            Id = 1,
            Name = "Master User",
            Icon = "person"
        };
        _historicalFigure.RelatedHistoricalFigures = [];

        _historicalFigureTarget = new HistoricalFigure
        {
            Id = 2,
            Name = "Apprentice Newbie",
            Icon = "person"
        };
        _historicalFigureTarget.RelatedHistoricalFigures = [];

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_historicalFigure);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_historicalFigureTarget);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "master" }
        };

        // Act
        var addHfhfLink = new AddHfhfLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_historicalFigure, addHfhfLink.HistoricalFigure);
        Assert.AreEqual(_historicalFigureTarget, addHfhfLink.HistoricalFigureTarget);
        Assert.AreEqual(HistoricalFigureLinkType.Master, addHfhfLink.LinkType);
    }

    [TestMethod]
    public void Constructor_WithAlternativePropertyNames_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hf", Value = "1" },
            new Property { Name = "hf_target", Value = "2" },
            new Property { Name = "link_type", Value = "spouse" }
        };

        // Act
        var addHfhfLink = new AddHfhfLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_historicalFigure, addHfhfLink.HistoricalFigure);
        Assert.AreEqual(_historicalFigureTarget, addHfhfLink.HistoricalFigureTarget);
        Assert.AreEqual(HistoricalFigureLinkType.Spouse, addHfhfLink.LinkType);
    }

    [TestMethod]
    public void Constructor_WithUnknownLinkType_SetsUnknownAndReportsError()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "unknown_type" }
        };

        // Act
        var addHfhfLink = new AddHfhfLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_historicalFigure, addHfhfLink.HistoricalFigure);
        Assert.AreEqual(_historicalFigureTarget, addHfhfLink.HistoricalFigureTarget);
        Assert.AreEqual(HistoricalFigureLinkType.Unknown, addHfhfLink.LinkType);
    }

    [TestMethod]
    public void Constructor_AddsEventToBothHistoricalFigures()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "master" }
        };
        var initialEventCount1 = _historicalFigure.Events.Count;
        var initialEventCount2 = _historicalFigureTarget.Events.Count;

        // Act
        var addHfhfLink = new AddHfhfLink(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount1 + 1, _historicalFigure.Events.Count);
        Assert.AreEqual(initialEventCount2 + 1, _historicalFigureTarget.Events.Count);
    }

    [TestMethod]
    public void Print_MasterLink_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "master" }
        };
        var addHfhfLink = new AddHfhfLink(properties, _mockWorld.Object);

        // Act
        var result = addHfhfLink.Print(link: true);

        // Assert - Master link: "Master User began an apprenticeship under Apprentice Newbie"
        Assert.IsTrue(result.Contains("Master User"));
        Assert.IsTrue(result.Contains("Apprentice Newbie"));
        Assert.IsTrue(result.Contains("began an apprenticeship under"));
    }

    [TestMethod]
    public void Print_ApprenticeLink_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "apprentice" }
        };
        var addHfhfLink = new AddHfhfLink(properties, _mockWorld.Object);

        // Act
        var result = addHfhfLink.Print(link: true);

        // Assert - Apprentice link: "Master User became the master of Apprentice Newbie"
        Assert.IsTrue(result.Contains("Master User"));
        Assert.IsTrue(result.Contains("Apprentice Newbie"));
        Assert.IsTrue(result.Contains("became the master of"));
    }

    [TestMethod]
    public void Print_SpouseLink_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "spouse" }
        };
        var addHfhfLink = new AddHfhfLink(properties, _mockWorld.Object);

        // Act
        var result = addHfhfLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Master User"));
        Assert.IsTrue(result.Contains("Apprentice Newbie"));
        Assert.IsTrue(result.Contains("married"));
    }

    [TestMethod]
    public void Print_PrisonerLink_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "prisoner" }
        };
        var addHfhfLink = new AddHfhfLink(properties, _mockWorld.Object);

        // Act
        var result = addHfhfLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Master User"));
        Assert.IsTrue(result.Contains("Apprentice Newbie"));
        Assert.IsTrue(result.Contains("imprisoned"));
    }

    [TestMethod]
    public void Print_WithNullHistoricalFigure_HandlesGracefully()
    {
        // Arrange
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns((HistoricalFigure?)null);

        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "master" }
        };
        var addHfhfLink = new AddHfhfLink(properties, _mockWorld.Object);

        // Act
        var result = addHfhfLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("unknown creature"));
    }

    [TestMethod]
    public void Print_WithNullHistoricalFigureTarget_HandlesGracefully()
    {
        // Arrange
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns((HistoricalFigure?)null);

        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "master" }
        };
        var addHfhfLink = new AddHfhfLink(properties, _mockWorld.Object);

        // Act
        var result = addHfhfLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("unknown creature"));
    }

    [TestMethod]
    public void Print_WithUnknownLinkType_ReturnsFallbackFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "unknown_type" }
        };
        var addHfhfLink = new AddHfhfLink(properties, _mockWorld.Object);

        // Act
        var result = addHfhfLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("linked"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "spouse" }
        };
        var addHfhfLink = new AddHfhfLink(properties, _mockWorld.Object);

        // Act
        var result = addHfhfLink.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("married"));
        // Without link=true, names should be plain text
    }

    [TestMethod]
    public void Print_LoverLink_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "lover" }
        };
        var addHfhfLink = new AddHfhfLink(properties, _mockWorld.Object);

        // Act
        var result = addHfhfLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("romantically involved with"));
    }

    [TestMethod]
    public void Print_FormerSpouseLink_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "former spouse" }
        };
        var addHfhfLink = new AddHfhfLink(properties, _mockWorld.Object);

        // Act
        var result = addHfhfLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("divorced"));
    }

    [TestMethod]
    public void Print_DeityLink_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "deity" }
        };
        var addHfhfLink = new AddHfhfLink(properties, _mockWorld.Object);

        // Act
        var result = addHfhfLink.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("worship"));
    }

    [TestMethod]
    public void Print_PetOwnerLink_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "pet owner" }
        };
        var addHfhfLink = new AddHfhfLink(properties, _mockWorld.Object);

        // Act
        var result = addHfhfLink.Print(link: true);

        // Assert - "became the pet of" when hf is the pet
        Assert.IsTrue(result.Contains("became the pet of"));
    }
}
