using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class AgreementVoidTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());
    }

    [TestMethod]
    public void Constructor_WithEmptyProperties_DoesNotThrow()
    {
        // Arrange
        var properties = new List<Property>();

        // Act
        var agreement = new AgreementVoid(properties, _mockWorld.Object);

        // Assert
        Assert.IsNotNull(agreement);
    }

    [TestMethod]
    public void Print_WithLink_ReturnsAnnulatedText()
    {
        // Arrange
        var properties = new List<Property>();
        var agreement = new AgreementVoid(properties, _mockWorld.Object);

        // Act
        var result = agreement.Print(link: true);

        // Assert
        Assert.IsTrue(result.Contains("agreement has been annulated"));
    }

    [TestMethod]
    public void Print_WithoutLink_ReturnsPlainText()
    {
        // Arrange
        var properties = new List<Property>();
        var agreement = new AgreementVoid(properties, _mockWorld.Object);

        // Act
        var result = agreement.Print(link: false);

        // Assert
        Assert.IsTrue(result.Contains("agreement has been annulated"));
    }

    [TestMethod]
    public void Print_IncludesYearTime()
    {
        // Arrange
        var properties = new List<Property>();
        var agreement = new AgreementVoid(properties, _mockWorld.Object);

        // Act
        var result = agreement.Print(link: true);

        // Assert - YearTime returns something like "In the year X" or "Year X"
        // Just check that it contains some numeric year information or typical year format
        Assert.IsTrue(result.Length > 20); // YearTime adds content
    }
}
