using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using static LegendsViewer.Backend.Legends.WorldObjects.HistoricalFigure;

namespace LegendsViewer.Backend.Legends.Events;

public class HfRevived : WorldEvent
{
    private readonly string? _ghostType;
    public HistoricalFigure? HistoricalFigure { get; set; }
    public HistoricalFigure? Actor { get; set; }
    public Site? Site { get; set; }
    public WorldRegion? Region { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }
    public bool RaisedBefore { get; set; }
    public bool Disturbance { get; set; }

    public HfRevived(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "ghost": _ghostType = property.Value; break;
                case "hfid": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
                case "raised_before": RaisedBefore = true; property.Known = true; break;
                case "actor_hfid": Actor = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "disturbance": Disturbance = true; property.Known = true; break;
            }
        }

        if(HistoricalFigure != null)
        {
            if (_ghostType != null)
            {
                HistoricalFigure.Ghost = true;
                HistoricalFigure.GhostType = _ghostType;
                HistoricalFigure.UndeadTypes.Add(new CreatureType(_ghostType, this));
            }
            else {
                HistoricalFigure.Zombie = true;
                HistoricalFigure.UndeadTypes.Add(new CreatureType("zombie", this));
            }
            HistoricalFigure.AddEvent(this);
        }

        Site.AddEvent(this);
        Region.AddEvent(this);
        UndergroundRegion.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(HistoricalFigure?.ToLink(link, pov, this));
        if (Disturbance)
        {
            sb.Append(" was disturbed from eternal rest");
        }
        else
        {
            sb.Append(Actor != null ? " was brought" : " came");
            sb.Append(" back from the dead");
        }

        if (RaisedBefore)
        {
            sb.Append(" once more");
        }

        if (Actor != null)
        {
            sb.Append(" by ");
            sb.Append(Actor.ToLink(link, pov, this));
        }

        if (!string.IsNullOrWhiteSpace(_ghostType))
        {
            if (RaisedBefore)
            {
                sb.Append(", this time");
            }
            sb.Append(" as a ");
            sb.Append(_ghostType);
        }

        if (Site != null)
        {
            sb.Append(" in ");
            sb.Append(Site.ToLink(link, pov, this));
        }
        else if (Region != null)
        {
            sb.Append(" in ");
            sb.Append(Region.ToLink(link, pov, this));
        }
        else if (UndergroundRegion != null)
        {
            sb.Append(" in ");
            sb.Append(UndergroundRegion.ToLink(link, pov, this));
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}
