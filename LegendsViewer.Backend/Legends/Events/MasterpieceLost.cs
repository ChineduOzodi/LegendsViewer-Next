using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class MasterpieceLost : WorldEvent
{
    public HistoricalFigure? HistoricalFigure { get; set; }
    public Site? Site { get; set; }
    public string? Method { get; set; }
    public MasterpieceItem? CreationEvent { get; set; }

    public MasterpieceLost(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "histfig": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "site": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "creation_event": CreationEvent = world.GetEvent(Convert.ToInt32(property.Value)) as MasterpieceItem; break;
                case "method": Method = property.Value; break;
            }
        }
        HistoricalFigure.AddEvent(this);
        Site.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append("the masterful ");
        if (CreationEvent != null)
        {
            sb.Append(!string.IsNullOrWhiteSpace(CreationEvent.Material) ? CreationEvent.Material + " " : "");
            if (!string.IsNullOrWhiteSpace(CreationEvent.ItemSubType) && CreationEvent.ItemSubType != "-1")
            {
                sb.Append(CreationEvent.ItemSubType);
            }
            else
            {
                sb.Append(!string.IsNullOrWhiteSpace(CreationEvent.ItemType) ? CreationEvent.ItemType : "UNKNOWN ITEM");
            }
            sb.Append(" created by ");
            sb.Append(CreationEvent.Maker != null ? CreationEvent.Maker.ToLink(link, pov, this) : "UNKNOWN HISTORICAL FIGURE");
            sb.Append(" for ");
            sb.Append(CreationEvent.MakerEntity != null ? CreationEvent.MakerEntity.ToLink(link, pov, this) : "UNKNOWN ENTITY");
            sb.Append(" at ");
            sb.Append(CreationEvent.Site != null ? CreationEvent.Site.ToLink(link, pov, this) : "UNKNOWN SITE");
            sb.Append(" ");
            sb.Append(CreationEvent.GetYearTime());
        }
        else
        {
            sb.Append("UNKNOWN ITEM");
        }
        sb.Append(" was destroyed by ");
        sb.Append(HistoricalFigure != null ? HistoricalFigure.ToLink(link, pov, this) : "an unknown creature");
        sb.Append(" in ");
        sb.Append(Site != null ? Site.ToLink(link, pov, this) : "UNKNOWN SITE");
        sb.Append(".");
        return sb.ToString();
    }
}
