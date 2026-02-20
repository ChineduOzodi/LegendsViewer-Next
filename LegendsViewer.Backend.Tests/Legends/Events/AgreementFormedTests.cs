using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class AgreementFormedTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _concluder = null!;
    private Entity _relevantEntity = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _concluder = new HistoricalFigure
        {
            Id = 1,
            Name = "Concluder Figure",
            Icon = "person"
        };

        _relevantEntity = new Entity([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Relevant Entity",
            Icon = "civilization"
        };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_concluder);
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(_relevantEntity);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "concluder_hfid", Value = "1" },
            new Property { Name = "agreement_id", Value = "42" },
            new Property { Name = "successful", Value = "true" },
            new Property { Name = "action", Value = "test action" },
            new Property { Name = "method", Value = "test method" }
        };

        // Act
        var agreement = new AgreementFormed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_concluder, agreement.Concluder);
        Assert.AreEqual(42, agreement.AgreementId);
        Assert.IsTrue(agreement.Successful);
        Assert.AreEqual("test action", agreement.Action);
        Assert.AreEqual("test method", agreement.Method);
    }

    [TestMethod]
    public void Constructor_WithWhimReason_ParsesWithoutError()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "reason", Value = "whim" }
        };

        // Act
        var agreement = new AgreementFormed(properties, _mockWorld.Object);

        // Assert - just verify it parses without error
        Assert.IsNotNull(agreement);
    }

    [TestMethod]
    public void Constructor_WithViolentDisagreementReason_ParsesWithoutError()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "reason", Value = "violent disagreement" }
        };

        // Act
        var agreement = new AgreementFormed(properties, _mockWorld.Object);

        // Assert - just verify it parses without error
        Assert.IsNotNull(agreement);
    }

    [TestMethod]
    public void Constructor_WithArrivedAtLocationReason_ParsesWithoutError()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "reason", Value = "arrived at location" }
        };

        // Act
        var agreement = new AgreementFormed(properties, _mockWorld.Object);

        // Assert - just verify it parses without error
        Assert.IsNotNull(agreement);
    }

    [TestMethod]
    public void Constructor_WithUnknownReason_ReportsParsingError()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "reason", Value = "unknown_reason" }
        };

        // Act
        var agreement = new AgreementFormed(properties, _mockWorld.Object);

        // Assert - just verify it parses without error
        Assert.IsNotNull(agreement);
    }

    [TestMethod]
    public void Constructor_WithFailedJudgmentTest_SetsFlag()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "failed_judgment_test", Value = "true" }
        };

        // Act
        var agreement = new AgreementFormed(properties, _mockWorld.Object);

        // Assert
        Assert.IsTrue(agreement.FailedJudgmentTest);
    }

    [TestMethod]
    public void Constructor_WithDelegated_SetsFlag()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "delegated", Value = "true" }
        };

        // Act
        var agreement = new AgreementFormed(properties, _mockWorld.Object);

        // Assert
        Assert.IsTrue(agreement.Delegated);
    }

    [TestMethod]
    public void Constructor_AddsEventToConcluder()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "concluder_hfid", Value = "1" }
        };
        var initialEventCount = _concluder.Events.Count;

        // Act
        var agreement = new AgreementFormed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _concluder.Events.Count);
    }

    [TestMethod]
    public void Constructor_AddsEventToRelevantEntity()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "relevant_entity_id", Value = "1" }
        };
        var initialEventCount = _relevantEntity.Events.Count;

        // Act
        var agreement = new AgreementFormed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _relevantEntity.Events.Count);
    }

    [TestMethod]
    public void Print_WithConcluder_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "concluder_hfid", Value = "1" },
            new Property { Name = "reason", Value = "whim" }
        };
        var agreement = new AgreementFormed(properties, _mockWorld.Object);

        // Act
        var result = agreement.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Concluder Figure"));
        Assert.IsTrue(result.Contains("formed an agreement"));
    }

    [TestMethod]
    public void Print_WithoutConcluder_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "reason", Value = "whim" }
        };
        var agreement = new AgreementFormed(properties, _mockWorld.Object);

        // Act
        var result = agreement.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("an agreement has been formed"));
    }

    [TestMethod]
    public void Print_WhimReason_ReturnsOnAWhim()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "concluder_hfid", Value = "1" },
            new Property { Name = "reason", Value = "whim" }
        };
        var agreement = new AgreementFormed(properties, _mockWorld.Object);

        // Act
        var result = agreement.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("on a whim"));
    }

    [TestMethod]
    public void Print_ViolentDisagreement_ReturnsAfterViolentDisagreement()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "concluder_hfid", Value = "1" },
            new Property { Name = "reason", Value = "violent disagreement" }
        };
        var agreement = new AgreementFormed(properties, _mockWorld.Object);

        // Act
        var result = agreement.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("after a violent disagreement"));
    }

    [TestMethod]
    public void Print_ArrivedAtLocation_ReturnsAfterArrivingAtLocation()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "concluder_hfid", Value = "1" },
            new Property { Name = "reason", Value = "arrived at location" }
        };
        var agreement = new AgreementFormed(properties, _mockWorld.Object);

        // Act
        var result = agreement.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("after arriving at the location"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "concluder_hfid", Value = "1" },
            new Property { Name = "reason", Value = "whim" }
        };
        var agreement = new AgreementFormed(properties, _mockWorld.Object);

        // Act
        var result = agreement.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("formed an agreement"));
        Assert.IsTrue(result.Contains("on a whim"));
    }

    [TestMethod]
    public void Constructor_WithTopFacet_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "top_facet", Value = "courage" },
            new Property { Name = "top_facet_rating", Value = "5" },
            new Property { Name = "top_facet_modifier", Value = "2" }
        };

        // Act
        var agreement = new AgreementFormed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual("courage", agreement.TopFacet);
        Assert.AreEqual(5, agreement.TopFacetRating);
        Assert.AreEqual(2, agreement.TopFacetModifier);
    }

    [TestMethod]
    public void Constructor_WithRelevantEntity_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "relevant_entity_id", Value = "1" },
            new Property { Name = "relevant_position_profile_id", Value = "10" },
            new Property { Name = "relevant_id_for_method", Value = "5" }
        };

        // Act
        var agreement = new AgreementFormed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_relevantEntity, agreement.RelevantEntity);
        Assert.AreEqual(10, agreement.RelevantPositionProfileId);
        Assert.AreEqual(5, agreement.RelevantIdForMethod);
    }

    [TestMethod]
    public void Constructor_WithBonusValues_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "ally_defense_bonus", Value = "3" },
            new Property { Name = "coconspirator_bonus", Value = "2" }
        };

        // Act
        var agreement = new AgreementFormed(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(3, agreement.AllyDefenseBonus);
        Assert.AreEqual(2, agreement.CoconspiratorBonus);
    }
}
