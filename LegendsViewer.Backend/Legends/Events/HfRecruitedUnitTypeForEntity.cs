using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class HfRecruitedUnitTypeForEntity : WorldEvent
{
    public HistoricalFigure? HistoricalFigure { get; set; }
    public Entity? Entity { get; set; }
    public UnitType UnitType { get; set; }
    public Site? Site { get; set; }
    public WorldRegion? Region { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }

    public HfRecruitedUnitTypeForEntity(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "hfid": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "entity_id": Entity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "unit_type":
                    switch (property.Value)
                    {
                        case "monk":
                            UnitType = UnitType.Monk;
                            break;
                        default:
                            property.Known = false;
                            break;
                    }
                    break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
            }
        }
        HistoricalFigure.AddEvent(this);
        Site.AddEvent(this);
        Region.AddEvent(this);
        UndergroundRegion.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(HistoricalFigure?.ToLink(link, pov, this));
        sb.Append(" recruited ");
        switch (UnitType)
        {
            case UnitType.Monk:
                sb.Append("monks");
                break;
            default:
                sb.Append(UnitType.GetDescription());
                break;
        }
        if (Entity != null)
        {
            sb.Append(" into ");
            sb.Append(Entity.ToLink(link, pov, this));
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
        else
        {
            sb.Append("UNKNOWN LOCATION");
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}
