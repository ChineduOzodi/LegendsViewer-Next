using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class InsurrectionStartedTests
{
    private Mock<IWorld> _mockWorld = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        var entity = new Entity([], _mockWorld.Object) { Id = 1, Name = "Kingdom", Icon = "civilization" };
        var site = new Site([], _mockWorld.Object) { Id = 1, Name = "City" };
        _mockWorld.Setup(w => w.GetEntity(1)).Returns(entity);
        _mockWorld.Setup(w => w.GetSite(1)).Returns(site);
    }

    [TestMethod]
    public void Print_ContainsInsurrectionText()
    {
        var props = new List<Property>
        {
            new() { Name = "target_civ_id", Value = "1" },
            new() { Name = "site_id", Value = "1" }
        };
        var evt = new InsurrectionStarted(props, _mockWorld.Object);
        var result = evt.Print(link: true);
        Assert.IsTrue(result.Contains("insurrection"));
    }
}
