using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.Events;

public class SquadVsSquad : WorldEvent
{
    public HistoricalFigure? AttackerHistoricalFigure { get; set; }
    public HistoricalFigure? DefenderHistoricalFigure { get; set; }
    public int AttackerSquadId { get; set; }
    public int DefenderSquadId { get; set; }
    public int DefenderRaceId { get; set; }
    public int DefenderNumber { get; set; }
    public int DefenderSlain { get; set; }
    public Site? Site { get; set; }
    public int StructureId { get; set; }
    public Structure? Structure { get; set; }
    public WorldRegion? Region { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }
    public HistoricalFigure? AttackerLeader { get; set; }
    public HistoricalFigure? DefenderLeader { get; set; }
    public int AttackerLeadershipRoll { get; set; }
    public int DefenderLeadershipRoll { get; set; }

    public SquadVsSquad(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "a_hfid": AttackerHistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "d_hfid": DefenderHistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "a_squad_id": AttackerSquadId = Convert.ToInt32(property.Value); break;
                case "d_squad_id": DefenderSquadId = Convert.ToInt32(property.Value); break;
                case "d_race": DefenderRaceId = Convert.ToInt32(property.Value); break;
                case "d_interaction":
                    // TODO last checked in version 0.44.10
                    property.Known = true;
                    break;
                case "d_effect":
                    // TODO last checked in version 0.44.10
                    property.Known = true;
                    break;
                case "d_number": DefenderNumber = Convert.ToInt32(property.Value); break;
                case "d_slain": DefenderSlain = Convert.ToInt32(property.Value); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "structure_id": StructureId = Convert.ToInt32(property.Value); break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
                case "a_leader_hfid": AttackerLeader = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "d_leader_hfid": DefenderLeader = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "a_leadership_roll": AttackerLeadershipRoll = Convert.ToInt32(property.Value); break;
                case "d_leadership_roll": DefenderLeadershipRoll = Convert.ToInt32(property.Value); break;
            }
        }
        if (Site != null)
        {
            Structure = Site.Structures.Find(structure => structure.LocalId == StructureId);
        }
        AttackerHistoricalFigure.AddEvent(this);
        DefenderHistoricalFigure.AddEvent(this);
        Site.AddEvent(this);
        Structure.AddEvent(this);
        Region.AddEvent(this);
        UndergroundRegion.AddEvent(this);
        if (AttackerLeader != AttackerHistoricalFigure)
        {
            AttackerLeader.AddEvent(this);
        }

        if (DefenderLeader != DefenderHistoricalFigure)
        {
            DefenderLeader.AddEvent(this);
        }
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(AttackerHistoricalFigure?.ToLink(link, pov, this) ?? "an unknown creature");
        if (AttackerLeader != null)
        {
            sb.Append(" as part of a squad");
            if (AttackerLeadershipRoll <= 25)
            {
                sb.Append(" poorly");
            }
            else if (AttackerLeadershipRoll >= 100)
            {
                sb.Append(" ably");
            }
            sb.Append(" led by ");
            sb.Append(AttackerLeader.ToLink(link, pov, this));
            sb.Append(",");
        }
        sb.Append(" clashed with ");
        if (DefenderHistoricalFigure != null)
        {
            sb.Append(DefenderHistoricalFigure.ToLink(link, pov, this));
            sb.Append(" as part of squad");
        }
        else if (DefenderNumber > 0 && DefenderRaceId > 0)
        {
            sb.Append(Formatting.IntegerToWords(DefenderNumber));
            sb.Append(" <span title='");
            sb.Append(DefenderRaceId);
            sb.Append("'>creatures</span>");
        }
        else
        {
            sb.Append("another squad");
        }
        if (Site != null)
        {
            sb.Append(" in ");
            sb.Append(Site.ToLink(link, pov, this));
        }
        if (DefenderLeader != null)
        {
            if (DefenderLeadershipRoll <= 25)
            {
                sb.Append(" poorly");
            }
            else if (DefenderLeadershipRoll >= 100)
            {
                sb.Append(" ably");
            }
            sb.Append(" led by ");
            sb.Append(DefenderLeader.ToLink(link, pov, this));
        }
        if (DefenderNumber == DefenderSlain)
        {
            sb.Append(", slaying them");
        }
        else if (DefenderSlain > 0)
        {
            sb.Append(", slaying ");
            sb.Append(Formatting.IntegerToWords(DefenderSlain));
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}

