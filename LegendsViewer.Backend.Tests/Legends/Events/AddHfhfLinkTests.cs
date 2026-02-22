using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using Moq;

namespace LegendsViewer.Backend.Tests.Legends.Events;

[TestClass]
public class AddHfhfLinkTests
{
    private Mock<IWorld> _mockWorld = null!;
    private HistoricalFigure _hf1 = null!;
    private HistoricalFigure _hf2 = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockWorld = new Mock<IWorld>();
        _mockWorld.Setup(w => w.ParsingErrors).Returns(new ParsingErrors());

        _hf1 = new HistoricalFigure
        {
            Id = 1,
            Name = "Figure One",
            Icon = "person"
        };

        _hf2 = new HistoricalFigure
        {
            Id = 2,
            Name = "Figure Two",
            Icon = "person"
        };

        _mockWorld.Setup(w => w.GetHistoricalFigure(1)).Returns(_hf1);
        _mockWorld.Setup(w => w.GetHistoricalFigure(2)).Returns(_hf2);
    }

    [TestMethod]
    public void Constructor_WithValidProperties_ParsesCorrectly()
    {
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "child" }
        };

        var evt = new AddHfhfLink(properties, _mockWorld.Object);

        Assert.IsNotNull(evt);
        Assert.AreEqual(_hf1, evt.HistoricalFigure);
        Assert.AreEqual(_hf2, evt.HistoricalFigureTarget);
        Assert.AreEqual(HistoricalFigureLinkType.Child, evt.LinkType);
    }

    [TestMethod]
    public void Constructor_WithPrisonerLinkType_LinksCorrectly()
    {
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "prisoner" }
        };

        var evt = new AddHfhfLink(properties, _mockWorld.Object);

        Assert.AreEqual(HistoricalFigureLinkType.Prisoner, evt.LinkType);
    }

    [TestMethod]
    public void Print_WithParentLink_ReturnsParentText()
    {
        var properties = new List<Property>
        {
            new Property { Name = "hfid", Value = "1" },
            new Property { Name = "hfid_target", Value = "2" },
            new Property { Name = "link_type", Value = "child" }
        };

        var evt = new AddHfhfLink(properties, _mockWorld.Object);

        var result = evt.Print(link: true);

        Assert.IsTrue(result.Contains("Figure One"));
        Assert.IsTrue(result.Contains("Figure Two"));
    }
}
