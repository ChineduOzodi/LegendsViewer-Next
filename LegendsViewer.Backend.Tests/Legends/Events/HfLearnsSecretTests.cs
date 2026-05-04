using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfLearnsSecretTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _student = null!;
    private HistoricalFigure _teacher = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();

        // Create student
        _student = new HistoricalFigure
        {
            Id = 1,
            Name = "Apprentice",
            Icon = "person"
        };

        // Create teacher
        _teacher = new HistoricalFigure
        {
            Id = 2,
            Name = "Master",
            Icon = "person"
        };

        // Setup mock world
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_student);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_teacher);
    }

    [TestMethod]
    public void Constructor_WithTeacher_ParsesCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "student_hfid", Value = "1" },
            new Property { Name = "teacher_hfid", Value = "2" },
            new Property { Name = "interaction", Value = "secret_magic" }
        };

        // Act
        var hfLearnsSecret = new HfLearnsSecret(properties, _mockWorld.Object);

        // Assert
        Assert.AreEqual(_student, hfLearnsSecret.Student);
        Assert.AreEqual(_teacher, hfLearnsSecret.Teacher);
    }

    [TestMethod]
    public void Print_WithTeacher_ReturnsFormattedString()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "student_hfid", Value = "1" },
            new Property { Name = "teacher_hfid", Value = "2" },
            new Property { Name = "interaction", Value = "secret_magic" }
        };
        var hfLearnsSecret = new HfLearnsSecret(properties, _mockWorld.Object);

        // Act
        var result = hfLearnsSecret.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("Master"));
        Assert.IsTrue(result.Contains("taught"));
        Assert.IsTrue(result.Contains("Apprentice"));
    }

    [TestMethod]
    public void Constructor_AddsNecromancerToStudent()
    {
        // Arrange
        var properties = new List<Property>
        {
            new Property { Name = "student_hfid", Value = "1" },
            new Property { Name = "teacher_hfid", Value = "2" },
            new Property { Name = "interaction", Value = "secret_magic" }
        };

        // Act
        var hfLearnsSecret = new HfLearnsSecret(properties, _mockWorld.Object);

        // Assert - student should have at least one creature type added
        Assert.IsTrue(_student.CreatureTypes.Count > 0);
    }
}
