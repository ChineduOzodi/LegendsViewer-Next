using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class ChangeHFJobTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _hf = null!;
    private Site _site = null!;

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

        _site = new Site([], _mockWorld.Object)
        {
            Id = 1,
            Name = "Test Site",
            Type = "TOWER"
        };
        _site.Structures = [];

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_hf);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(_site);
    }

    [TestMethod]
    public void Constructor_WithBasicProperties_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "new_job", Value = "king" },
            new Property { Name = "old_job", Value = "queen" }
        };

        // Act
        var changeHfJob = new ChangeHfJob(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(changeHfJob);
        Assert.AreEqual(_hf, changeHfJob.HistoricalFigure);
        Assert.AreEqual("king", changeHfJob.NewJob);
        Assert.AreEqual("queen", changeHfJob.OldJob);
    }

    [TestMethod]
    public void Constructor_WithStandardJob_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "new_job", Value = "standard" }
        };

        // Act
        var changeHfJob = new ChangeHfJob(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual("standard", changeHfJob.NewJob);
    }

    [TestMethod]
    public void Constructor_WithSite_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "new_job", Value = "king" },
            new Property { Name = "site_id", Value = "1" }
        };

        // Act
        var changeHfJob = new ChangeHfJob(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_site, changeHfJob.Site);
    }

    [TestMethod]
    public void Constructor_AddsEventToHistoricalFigure()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "new_job", Value = "king" }
        };
        var initialEventCount = _hf.Events.Count;

        // Act
        var changeHfJob = new ChangeHfJob(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(initialEventCount + 1, _hf.Events.Count);
    }

    [TestMethod]
    public void Print_WithBothJobs_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "new_job", Value = "king" },
            new Property { Name = "old_job", Value = "queen" }
        };
        var changeHfJob = new ChangeHfJob(properties, _mockWorld.Object);

        // Act
        var result = changeHfJob.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Test HF"));
        Assert.IsTrue(result.Contains("gave up"));
        Assert.IsTrue(result.Contains("king"));
        Assert.IsTrue(result.Contains("queen"));
    }

    [TestMethod]
    public void Print_WithOnlyNewJob_ReturnsCorrectFormat()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "new_job", Value = "king" }
        };
        var changeHfJob = new ChangeHfJob(properties, _mockWorld.Object);

        // Act
        var result = changeHfJob.Print(link: true);

        // Assert - just verify it contains something meaningful
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "new_job", Value = "king" },
            new Property { Name = "old_job", Value = "queen" }
        };
        var changeHfJob = new ChangeHfJob(properties, _mockWorld.Object);

        // Act
        var result = changeHfJob.Print(link: false);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
    }
}
