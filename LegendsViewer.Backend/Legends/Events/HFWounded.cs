using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class HfWounded : WorldEvent
{
    public int WoundeeRace { get; set; }
    public int WoundeeCaste { get; set; }
    public int BodyPart { get; set; }
    public string? InjuryType { get; set; }
    public bool PartLost { get; set; }

    public HistoricalFigure? Woundee { get; set; }
    public HistoricalFigure? Wounder { get; set; }
    public Site? Site { get; set; }
    public WorldRegion? Region { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }
    public bool WasTorture { get; set; }

    public HfWounded(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "woundee_hfid": Woundee = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "wounder_hfid": Wounder = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
                case "woundee": if (Woundee == null) { Woundee = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
                case "wounder": if (Wounder == null) { Wounder = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
                case "site": if (Site == null) { Site = world.GetSite(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
                case "woundee_race": WoundeeRace = Convert.ToInt32(property.Value); break;
                case "woundee_caste": WoundeeCaste = Convert.ToInt32(property.Value); break;
                case "body_part": BodyPart = Convert.ToInt32(property.Value); break;
                case "injury_type": InjuryType = property.Value; break;
                case "part_lost":
                    if (int.TryParse(property.Value, out int partLost))
                    {
                        PartLost = partLost != 0;
                    }
                    else if (bool.TryParse(property.Value, out bool partLostBool))
                    {
                        PartLost = partLostBool;
                    }
                    else
                    {
                        property.Known = false;
                    }
                    break;
                case "was_torture":
                    property.Known = true;
                    WasTorture = true;
                    break;
            }
        }

        Woundee?.AddEvent(this);
        if (Woundee != Wounder)
        {
            Wounder?.AddEvent(this);
        }
        Site?.AddEvent(this);
        Region?.AddEvent(this);
        UndergroundRegion?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        if (Woundee != null)
        {
            sb.Append(Woundee.ToLink(link, pov, this));
        }
        else
        {
            sb.Append("UNKNOWN HISTORICAL FIGURE");
        }

        sb.Append(" was wounded by ");
        if (Wounder != null)
        {
            sb.Append(Wounder.ToLink(link, pov, this));
        }
        else
        {
            sb.Append("UNKNOWN HISTORICAL FIGURE");
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

        if (WasTorture)
        {
            sb.Append(" as a means of torture");
        }

        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}
