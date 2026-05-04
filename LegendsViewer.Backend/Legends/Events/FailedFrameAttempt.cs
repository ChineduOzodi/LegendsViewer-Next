using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class FailedFrameAttempt : WorldEvent
{
    public HistoricalFigure? TargetHf { get; set; }
    public Entity? ConvicterEntity { get; set; }
    public HistoricalFigure? FooledHf { get; set; }
    public HistoricalFigure? FramerHf { get; set; }
    public HistoricalFigure? PlotterHf { get; set; }
    public string? Crime { get; set; }

    public FailedFrameAttempt(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "target_hfid": TargetHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "convicter_enid": ConvicterEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "fooled_hfid": FooledHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "framer_hfid": FramerHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "plotter_hfid": PlotterHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "crime": Crime = property.Value; break;
            }
        }

        TargetHf.AddEvent(this);
        ConvicterEntity.AddEvent(this);
        if (FooledHf != TargetHf)
        {
            FooledHf.AddEvent(this);
        }
        if (FramerHf != FooledHf)
        {
            FramerHf.AddEvent(this);
        }
        PlotterHf.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(FramerHf?.ToLink(link, pov, this));
        sb.Append(" attempted to frame ");
        sb.Append(TargetHf?.ToLink(link, pov, this));
        sb.Append($" for {Crime}");
        if (PlotterHf != null)
        {
            sb.Append(" at the behest of ");
            sb.Append(PlotterHf.ToLink(link, pov, this));
        }
        sb.Append(" by fooling ");
        sb.Append(FooledHf?.ToLink(link, pov, this));
        if (ConvicterEntity != null)
        {
            sb.Append(" and ");
            sb.Append(ConvicterEntity?.ToLink(link, pov, this));
        }
        sb.Append(" with fabricated evidence, but nothing came from it");
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}


