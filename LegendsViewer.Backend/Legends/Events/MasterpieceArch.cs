using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class MasterpieceArch : WorldEvent
{
    private string? SkillAtTime { get; set; }
    public HistoricalFigure? Maker { get; set; }
    public Entity? MakerEntity { get; set; }
    public Site? Site { get; set; }
    public string? BuildingType { get; set; }
    public string? BuildingSubType { get; set; }
    public int BuildingCustom { get; set; }
    public string? Process { get; set; }

    public MasterpieceArch(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "skill_at_time": SkillAtTime = property.Value; break;
                case "hfid": Maker = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "entity_id": MakerEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "maker": if (Maker == null) { Maker = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
                case "maker_entity": if (MakerEntity == null) { MakerEntity = world.GetEntity(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
                case "site": if (Site == null) { Site = world.GetSite(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
                case "building_type": BuildingType = property.Value; break;
                case "building_subtype": BuildingSubType = property.Value; break;
                case "building_custom": BuildingCustom = Convert.ToInt32(property.Value); break;
            }
        }
        Maker.AddEvent(this);
        MakerEntity.AddEvent(this);
        Site.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Maker != null ? Maker.ToLink(link, pov, this) : "UNKNOWN HISTORICAL FIGURE");
        sb.Append(" ");
        sb.Append(Process);
        sb.Append(" a masterful ");
        if (!string.IsNullOrWhiteSpace(BuildingSubType) && BuildingSubType != "-1")
        {
            sb.Append(BuildingSubType);
        }
        else
        {
            sb.Append(!string.IsNullOrWhiteSpace(BuildingType) ? BuildingType : "UNKNOWN BUILDING");
        }
        sb.Append(" for ");
        sb.Append(MakerEntity != null ? MakerEntity.ToLink(link, pov, this) : "UNKNOWN ENTITY");
        sb.Append(" in ");
        sb.Append(Site != null ? Site.ToLink(link, pov, this) : "UNKNOWN SITE");
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}
