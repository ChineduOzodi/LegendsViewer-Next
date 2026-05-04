using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class MasterpieceDye : WorldEvent
{
    private string? SkillAtTime { get; set; }
    public HistoricalFigure? Maker { get; set; }
    public Entity? MakerEntity { get; set; }
    public Site? Site { get; set; }
    public string? ItemType { get; set; }
    public string? ItemSubType { get; set; }
    public string? Material { get; set; }
    public int MaterialType { get; set; }
    public int MaterialIndex { get; set; }
    public string? DyeMaterial { get; set; }
    public int DyeMaterialType { get; set; }
    public int DyeMaterialIndex { get; set; }

    public MasterpieceDye(List<Property> properties, IWorld world)
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
                case "item_type": ItemType = property.Value.Replace("_", " "); break;
                case "item_subtype": ItemSubType = property.Value.Replace("_", " "); break;
                case "mat": Material = property.Value.Replace("_", " "); break;
                case "mat_type": MaterialType = Convert.ToInt32(property.Value); break;
                case "mat_index": MaterialIndex = Convert.ToInt32(property.Value); break;
                case "dye_mat": DyeMaterial = property.Value.Replace("_", " "); break;
                case "dye_mat_type": DyeMaterialType = Convert.ToInt32(property.Value); break;
                case "dye_mat_index": DyeMaterialIndex = Convert.ToInt32(property.Value); break;
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
        sb.Append(" masterfully dyed a ");
        if (!string.IsNullOrWhiteSpace(Material))
        {
            sb.Append(Material);
            sb.Append(" ");
        }
        if (!string.IsNullOrWhiteSpace(ItemSubType) && ItemSubType != "-1")
        {
            sb.Append(ItemSubType);
        }
        else
        {
            sb.Append(!string.IsNullOrWhiteSpace(ItemType) ? ItemType : "UNKNOWN ITEM");
        }
        sb.Append(" with ");
        sb.Append(!string.IsNullOrWhiteSpace(DyeMaterial) ? DyeMaterial : "UNKNOWN DYE");
        sb.Append(" for ");
        sb.Append(MakerEntity != null ? MakerEntity.ToLink(link, pov, this) : "UNKNOWN ENTITY");
        sb.Append(" in ");
        sb.Append(Site != null ? Site.ToLink(link, pov, this) : "UNKNOWN SITE");
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}
