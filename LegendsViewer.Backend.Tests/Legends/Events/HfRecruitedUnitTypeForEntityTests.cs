using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class HfRecruitedUnitTypeForEntityTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        var hf = new HistoricalFigure { Id = 1, Name = "Recruiter", Icon = "person" };
        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(hf);
    }

    [TestMethod]
    public void Print_ContainsRecruitedText()
    {
        var props = new List<Property>
        {
            new() { Name = "hfid", Value = "1" },
            new() { Name = "unit_type", Value = "monk" }
        };
        var evt = new HfRecruitedUnitTypeForEntity(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("recruited"));
    }
}
