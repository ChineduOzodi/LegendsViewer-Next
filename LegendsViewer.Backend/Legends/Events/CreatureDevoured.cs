using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class CreatureDevoured : WorldEvent
{
    public string? Race { get; set; }
    public string? Caste { get; set; }

    public HistoricalFigure? Eater, Victim;
    public Entity? Entity { get; set; }
    public Site? Site { get; set; }
    public WorldRegion? Region { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }

    public CreatureDevoured(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
                case "victim": Victim = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "eater": Eater = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "entity": Entity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "race": Race = property.Value.Replace("_", " "); break;
                case "caste": Caste = property.Value; break;
                case "site": if (Site == null) { Site = world.GetSite(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
            }
        }

        Site?.AddEvent(this);
        Region?.AddEvent(this);
        UndergroundRegion?.AddEvent(this);
        Eater?.AddEvent(this);
        Victim?.AddEvent(this);
        Entity?.AddEvent(this);
    }
    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        if (Eater != null)
        {
            sb.Append(Eater.ToLink(link, pov, this));
        }
        else
        {
            sb.Append("UNKNOWN HISTORICAL FIGURE");
        }
        sb.Append(" devoured ");
        if (Victim != null)
        {
            sb.Append(Victim.ToLink(link, pov, this));
        }
        else if (!string.IsNullOrWhiteSpace(Race))
        {
            sb.Append(" a ");
            if (!string.IsNullOrWhiteSpace(Caste))
            {
                sb.Append(Caste);
                sb.Append(' ');
            }
            sb.Append(Race);
        }
        else
        {
            sb.Append("UNKNOWN HISTORICAL FIGURE");
        }
        sb.Append(" in ");
        if (Site != null)
        {
            sb.Append(Site.ToLink(link, pov, this));
        }
        else if (Region != null)
        {
            sb.Append(Region.ToLink(link, pov, this));
        }
        else if (UndergroundRegion != null)
        {
            sb.Append(UndergroundRegion.ToLink(link, pov, this));
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append('.');
        return sb.ToString();
    }
}

