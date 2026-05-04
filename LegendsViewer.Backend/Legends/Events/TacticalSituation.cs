using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class TacticalSituation : WorldEvent
{
    public HistoricalFigure? AttackerTactician { get; set; }
    public HistoricalFigure? DefenderTactician { get; set; }
    public int AttackerTacticsRoll { get; set; }
    public int DefenderTacticsRoll { get; set; }
    public TacticalSituationType Situation { get; set; }
    public Site? Site { get; set; }
    public int StructureId { get; set; }
    public Structure? Structure { get; set; }
    public WorldRegion? Region { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }

    public TacticalSituation(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "a_tactician_hfid": AttackerTactician = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "d_tactician_hfid": DefenderTactician = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "a_tactics_roll": AttackerTacticsRoll = Convert.ToInt32(property.Value); break;
                case "d_tactics_roll": DefenderTacticsRoll = Convert.ToInt32(property.Value); break;
                case "situation":
                    switch (property.Value)
                    {
                        case "neither favored":
                            Situation = TacticalSituationType.NeitherFavored;
                            break;
                        case "a slightly favored":
                            Situation = TacticalSituationType.AttackersSlightlyFavored;
                            break;
                        case "d slightly favored":
                            Situation = TacticalSituationType.DefendersSlightlyFavored;
                            break;
                        case "a strongly favored":
                            Situation = TacticalSituationType.AttackersStronglyFavored;
                            break;
                        case "d strongly favored":
                            Situation = TacticalSituationType.DefendersStronglyFavored;
                            break;
                        default:
                            property.Known = false;
                            break;
                    }
                    break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "structure_id": StructureId = Convert.ToInt32(property.Value); break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
                case "start":
                    // TODO last checked in version 0.44.10
                    property.Known = true;
                    break;
            }
        }
        if (Site != null)
        {
            Structure = Site.Structures.Find(structure => structure.LocalId == StructureId);
        }
        AttackerTactician.AddEvent(this);
        DefenderTactician.AddEvent(this);
        Site.AddEvent(this);
        Structure.AddEvent(this);
        Region.AddEvent(this);
        UndergroundRegion.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        if (AttackerTactician != null && DefenderTactician != null)
        {
            if (AttackerTacticsRoll > DefenderTacticsRoll)
            {
                sb.Append(AttackerTactician.ToLink(link, pov, this));
            }
            else
            {
                sb.Append(DefenderTactician.ToLink(link, pov, this));
            }
            if (Situation.ToString().Contains("Strongly"))
            {
                sb.Append(" entirely outwitted ");
            }
            else
            {
                sb.Append(" outmanuevered ");
            }
            if (AttackerTacticsRoll > DefenderTacticsRoll)
            {
                sb.Append(DefenderTactician?.ToLink(link, pov, this) ?? "an unknown creature");
            }
            else
            {
                sb.Append(AttackerTactician?.ToLink(link, pov, this) ?? "an unknown creature");
            }
        }
        else if (AttackerTactician != null)
        {
            sb.Append(AttackerTactician.ToLink(link, pov, this));
            sb.Append(" used ");
            sb.Append(AttackerTacticsRoll > DefenderTacticsRoll ? "good" : "poor");
            sb.Append(" tactics");
        }
        else if (DefenderTactician != null)
        {
            sb.Append(DefenderTactician.ToLink(link, pov, this));
            sb.Append(" used ");
            sb.Append(AttackerTacticsRoll > DefenderTacticsRoll ? "poor" : "good");
            sb.Append(" tactics");
        }
        else
        {
            sb.Append("the forces shifted");
        }
        switch (Situation)
        {
            case TacticalSituationType.NeitherFavored:
                sb.Append(", but neither side had a positional advantage");
                break;
            case TacticalSituationType.AttackersSlightlyFavored:
                sb.Append(", but the attackers had a slight positional advantage");
                break;
            case TacticalSituationType.DefendersSlightlyFavored:
                sb.Append(", but the defenders had a slight positional advantage");
                break;
            case TacticalSituationType.AttackersStronglyFavored:
                sb.Append(", and the attackers had a strong positional advantage");
                break;
            case TacticalSituationType.DefendersStronglyFavored:
                sb.Append(", and the defenders had a strong positional advantage");
                break;
        }
        if (Site != null)
        {
            sb.Append(" in ");
            sb.Append(Site.ToLink(link, pov, this));
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}

