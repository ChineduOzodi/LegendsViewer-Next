using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.Events;

public class BodyAbused : WorldEvent
{
    // TODO
    public string? ItemType { get; set; } // legends_plus.xml
    public string? ItemSubType { get; set; } // legends_plus.xml
    public string? Material { get; set; } // legends_plus.xml
    public int PileTypeId { get; set; } // legends_plus.xml
    public PileType PileType { get; set; } // legends_plus.xml
    public int MaterialTypeId { get; set; } // legends_plus.xml
    public int MaterialIndex { get; set; } // legends_plus.xml

    public AbuseType AbuseType { get; set; } // legends_plus.xml
    public Entity? Abuser { get; set; } // legends_plus.xml
    public Entity? Victim { get; set; } // legends_plus.xml
    public List<HistoricalFigure> Bodies { get; set; } = []; // legends_plus.xml
    public HistoricalFigure? HistoricalFigure { get; set; } // legends_plus.xml
    public Site? Site { get; set; }
    public WorldRegion? Region { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }
    public Location? Coordinates { get; set; }
    public Structure? Structure { get; set; }

    public BodyAbused(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        int structureId = -1;
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "coords": Coordinates = Formatting.ConvertToLocation(property.Value, world); break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
                case "site": if (Site == null) { Site = world.GetSite(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
                case "civ": Abuser = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "victim_entity": Victim = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "bodies":
                    HistoricalFigure? body = world.GetHistoricalFigure(Convert.ToInt32(property.Value));
                    if (body != null)
                    {
                        Bodies.Add(body);
                    }
                    break;
                case "histfig": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "props_item_type":
                case "item_type":
                    ItemType = property.Value;
                    break;
                case "props_item_subtype":
                case "item_subtype":
                    ItemSubType = property.Value;
                    break;
                case "props_item_mat":
                case "item_mat":
                    Material = property.Value;
                    break;
                case "abuse_type":
                    switch (property.Value)
                    {
                        case "0":
                        case "impaled":
                            AbuseType = AbuseType.Impaled;
                            break;
                        case "1":
                        case "piled":
                            AbuseType = AbuseType.Piled;
                            break;
                        case "2":
                        case "flayed":
                            AbuseType = AbuseType.Flayed;
                            break;
                        case "3":
                        case "hung":
                            AbuseType = AbuseType.Hung;
                            break;
                        case "4":
                        case "mutilated":
                            AbuseType = AbuseType.Mutilated;
                            break;
                        case "5":
                        case "animated":
                            AbuseType = AbuseType.Animated;
                            break;
                        default:
                            property.Known = false;
                            break;
                    }
                    break;
                case "pile_type":
                    switch (property.Value)
                    {
                        case "gruesomesculpture":
                            PileType = PileType.GruesomeSculpture;
                            break;
                        case "grislymound":
                            PileType = PileType.GrislyMound;
                            break;
                        case "grotesquepillar":
                            PileType = PileType.GrotesquePillar;
                            break;
                        default:
                            property.Known = false;
                            break;
                    }
                    break;
                case "props_pile_type": PileTypeId = Convert.ToInt32(property.Value); break;
                case "props_item_mat_type": MaterialTypeId = Convert.ToInt32(property.Value); break;
                case "props_item_mat_index": MaterialIndex = Convert.ToInt32(property.Value); break;
                case "tree":
                    property.Known = true; // TODO no idea what this is
                    break;
                case "structure":
                    structureId = Convert.ToInt32(property.Value);
                    break;
                case "interaction":
                    property.Known = true; // TODO no idea what this is
                    break;
            }
        }

        Site?.AddEvent(this);
        Region?.AddEvent(this);
        UndergroundRegion?.AddEvent(this);
        Bodies.ForEach(body =>
        {
            if (body != null)
            {
                body.AddEvent(this);
                if (AbuseType == AbuseType.Animated)
                {
                    body.Zombie = true;
                    body.UndeadTypes.Add(new HistoricalFigure.CreatureType("zombie", this));
                }
            }
        });
        HistoricalFigure?.AddEvent(this);
        Abuser?.AddEvent(this);
        Victim?.AddEvent(this);
        if (structureId != -1 && Site != null)
        {
            Structure = Site.Structures.Find(structure => structure.LocalId == structureId);
            Structure?.AddEvent(this);
        }
    }
    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        if (Bodies.Count > 1)
        {
            sb.Append("the bodies of ");
            for (int i = 0; i < Bodies.Count; i++)
            {
                sb.Append(Bodies[i].ToLink(link, pov, this) ?? "UNKNOWN HISTORICAL FIGURE");
                if (i != Bodies.Count - 1)
                {
                    if (i == Bodies.Count - 2)
                    {
                        sb.Append(" and ");
                    }
                    else
                    {
                        sb.Append(", ");
                    }
                }
            }
            sb.Append(" were ");
        }
        else
        {
            sb.Append("the body of ");
            sb.Append(Bodies.FirstOrDefault()?.ToLink(link, pov, this) ?? "UNKNOWN HISTORICAL FIGURE");
            sb.Append(" was ");
        }
        switch (AbuseType)
        {
            case AbuseType.Impaled:
                sb.Append("impaled on a ");
                sb.Append(!string.IsNullOrWhiteSpace(Material) ? Material + " " : "");
                if (!string.IsNullOrWhiteSpace(ItemSubType) && ItemSubType != "-1")
                {
                    sb.Append(ItemSubType);
                }
                else
                {
                    sb.Append(!string.IsNullOrWhiteSpace(ItemType) ? ItemType : "UNKNOWN ITEM");
                }
                break;
            case AbuseType.Piled:
                sb.Append("added to a ");
                sb.Append(PileType.GetDescription());
                break;
            case AbuseType.Flayed:
                sb.Append("flayed");
                break;
            case AbuseType.Hung:
                sb.Append("hung");
                break;
            case AbuseType.Mutilated:
                sb.Append("horribly mutilated");
                break;
            case AbuseType.Animated:
                sb.Append("animated");
                break;
            default:
                sb.Append("abused");
                break;
        }
        sb.Append(" by ");

        if (HistoricalFigure != null)
        {
            sb.Append(HistoricalFigure.ToLink(link, pov, this));
            if (Abuser != null)
            {
                sb.Append(" of ");
            }
        }
        if (Abuser != null)
        {
            sb.Append(Abuser.ToLink(link, pov, this));
        }
        if (Structure != null)
        {
            sb.Append(" in ");
            sb.Append(Structure.ToLink(link, pov, this));
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
        sb.Append('.');
        return sb.ToString();
    }
}

