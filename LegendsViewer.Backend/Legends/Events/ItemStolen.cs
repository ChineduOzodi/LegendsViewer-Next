using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.EventCollections;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class ItemStolen : WorldEvent
{
    public int StructureId { get; set; }
    public Structure? Structure { get; set; }
    public Artifact? Artifact { get; set; }
    public string? ItemType { get; set; }
    public string? ItemSubType { get; set; }
    public string? Material { get; set; }
    public int MaterialType { get; set; }
    public int MaterialIndex { get; set; }
    public HistoricalFigure? Thief { get; set; }
    public Entity? Entity { get; set; }
    public Site? Site { get; set; }
    public Site? ReturnSite { get; set; }
    public Circumstance Circumstance { get; set; }
    public int CircumstanceId { get; set; }
    public string? TheftMethod { get; set; }

    public ItemStolen(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "histfig": Thief = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "entity": Entity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "item": Artifact = world.GetArtifact(Convert.ToInt32(property.Value)); break;
                case "item_type": ItemType = property.Value.Replace("_", " "); break;
                case "item_subtype": ItemSubType = property.Value; break;
                case "mat": Material = property.Value; break;
                case "mattype": MaterialType = Convert.ToInt32(property.Value); break;
                case "matindex": MaterialIndex = Convert.ToInt32(property.Value); break;
                case "stash_site": ReturnSite = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "site":
                    if (Site == null) { Site = world.GetSite(Convert.ToInt32(property.Value)); } else { property.Known = true; }
                    break;
                case "structure": StructureId = Convert.ToInt32(property.Value); break;
                case "circumstance":
                    switch (property.Value)
                    {
                        case "historical event collection": Circumstance = Circumstance.HistoricalEventCollection; break;
                        case "defeated hf": Circumstance = Circumstance.DefeatedHf; break;
                        case "murdered hf": Circumstance = Circumstance.MurderedHf; break;
                        case "abducted hf": Circumstance = Circumstance.AbductedHf; break;
                        default: if (property.Value != "-1") { property.Known = false; } break;
                    }
                    break;
                case "circumstance_id": CircumstanceId = Convert.ToInt32(property.Value); break;
                case "reason":
                case "reason_id": if (property.Value != "-1") { property.Known = false; } break;
                case "theft_method": if (property.Value != "theft") { TheftMethod = property.Value; } break;
            }
        }
        if (Site != null)
        {
            Structure = Site.Structures.Find(structure => structure.LocalId == StructureId);
        }
        Thief?.AddEvent(this);
        Site?.AddEvent(this);
        Entity?.AddEvent(this);
        Structure?.AddEvent(this);
        Artifact?.AddEvent(this);
        ReturnSite?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        if (Artifact != null)
        {
            sb.Append(Artifact.ToLink(link, pov, this));
        }
        else if (string.IsNullOrEmpty(ItemType))
        {
            sb.Append(" an unknown item ");
        }
        else
        {
            sb.Append(" a ");
            if (!string.IsNullOrWhiteSpace(Material))
            {
                sb.Append(Material);
                sb.Append(" ");
            }
            sb.Append(ItemType);
        }
        sb.Append(" was ");
        if (!string.IsNullOrWhiteSpace(TheftMethod))
        {
            sb.Append(TheftMethod);
        }
        else
        {
            sb.Append("stolen");
        }
        sb.Append(" ");
        if (Structure != null)
        {
            sb.Append("from ");
            sb.Append(Structure.ToLink(link, pov, this));
            sb.Append(" ");
        }
        sb.Append("in ");
        if (Site != null)
        {
            sb.Append(Site.ToLink(link, pov, this));
        }
        else
        {
            sb.Append("UNKNOWN SITE");
        }
        sb.Append(" by ");
        if (Thief != null)
        {
            sb.Append(Thief.ToLink(link, pov, this));
        }
        else
        {
            sb.Append("an unknown creature");
        }

        if (ReturnSite != null)
        {
            sb.Append(" and brought to ");
            sb.Append(ReturnSite.ToLink());
        }
        if (ParentCollection is not Theft)
        {
            sb.Append(PrintParentCollection(link, pov));
        }
        sb.Append(".");
        return sb.ToString();
    }
}
