using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.Events;

public class FailedIntrigueCorruption : WorldEvent
{
    public HistoricalFigure? CorruptorHf { get; set; }
    public HistoricalFigure? TargetHf { get; set; }
    public Site? Site { get; set; }
    public WorldRegion? Region { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }

    public bool FailedJudgmentTest { get; set; }

    public IntrigueAction? Action { get; set; }
    public IntrigueMethod? Method { get; set; }

    public string? TopFacet { get; set; }
    public int TopFacetRating { get; set; }
    public int TopFacetModifier { get; set; }

    public string? TopValue { get; set; }
    public int TopValueRating { get; set; }
    public int TopValueModifier { get; set; }

    public string? TopRelationshipFactor { get; set; }
    public int TopRelationshipRating { get; set; }
    public int TopRelationshipModifier { get; set; }

    public int AllyDefenseBonus { get; set; }
    public int CoConspiratorBonus { get; set; }

    public HistoricalFigure? LureHf { get; set; }
    public int CorruptorIdentityId { get; set; }
    public int TargetIdentityId { get; set; }

    public Entity? RelevantEntity { get; set; }
    public int RelevantPositionProfileId { get; set; }
    public int RelevantIdForMethod { get; set; }

    public FailedIntrigueCorruption(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "corruptor_hfid": CorruptorHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "target_hfid": TargetHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
                case "failed_judgment_test": property.Known = true; FailedJudgmentTest = true; break;
                case "action":
                    switch (property.Value.Replace("_", " "))
                    {
                        case "bribe official": Action = IntrigueAction.BribeOfficial; break;
                        case "induce to embezzle": Action = IntrigueAction.InduceToEmbezzle; break;
                        case "corrupt in place": Action = IntrigueAction.CorruptInPlace; break;
                        case "bring into network": Action = IntrigueAction.BringIntoNetwork; break;
                        default:
                            property.Known = false;
                            break;
                    }
                    break;
                case "method":
                    switch (property.Value.Replace("_", " "))
                    {
                        case "intimidate": Method = IntrigueMethod.Intimidate; break;
                        case "flatter": Method = IntrigueMethod.Flatter; break;
                        case "bribe": Method = IntrigueMethod.Bribe; break;
                        case "precedence": Method = IntrigueMethod.Precedence; break;
                        case "offer immortality": Method = IntrigueMethod.OfferImmortality; break;
                        case "religious sympathy": Method = IntrigueMethod.ReligiousSympathy; break;
                        case "blackmail over embezzlement": Method = IntrigueMethod.BlackmailOverEmbezzlement; break;
                        case "revenge on grudge": Method = IntrigueMethod.RevengeOnGrudge; break;
                        default:
                            property.Known = false;
                            break;
                    }
                    break;
                case "top_facet": TopFacet = property.Value; break;
                case "top_facet_rating": TopFacetRating = Convert.ToInt32(property.Value); break;
                case "top_facet_modifier": TopFacetModifier = Convert.ToInt32(property.Value); break;
                case "top_value": TopValue = property.Value; break;
                case "top_value_rating": TopValueRating = Convert.ToInt32(property.Value); break;
                case "top_value_modifier": TopValueModifier = Convert.ToInt32(property.Value); break;
                case "top_relationship_factor": TopRelationshipFactor = property.Value; break;
                case "top_relationship_rating": TopRelationshipRating = Convert.ToInt32(property.Value); break;
                case "top_relationship_modifier": TopRelationshipModifier = Convert.ToInt32(property.Value); break;
                case "ally_defense_bonus": AllyDefenseBonus = Convert.ToInt32(property.Value); break;
                case "coconspirator_bonus": CoConspiratorBonus = Convert.ToInt32(property.Value); break;
                case "lure_hfid": LureHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "corruptor_identity": CorruptorIdentityId = Convert.ToInt32(property.Value); break;
                case "target_identity": TargetIdentityId = Convert.ToInt32(property.Value); break;
                case "relevant_entity_id": RelevantEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "relevant_position_profile_id": RelevantPositionProfileId = Convert.ToInt32(property.Value); break;
                case "relevant_id_for_method": RelevantIdForMethod = Convert.ToInt32(property.Value); break;
            }
        }

        CorruptorHf.AddEvent(this);
        TargetHf.AddEvent(this);
        Site.AddEvent(this);
        Region.AddEvent(this);
        UndergroundRegion.AddEvent(this);
        LureHf.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(CorruptorHf?.ToLink(link, pov, this));
        sb.Append(" attempted to corrupt ");
        sb.Append(TargetHf?.ToLink(link, pov, this));
        switch (Action)
        {
            case IntrigueAction.BribeOfficial:
                sb.Append(" in order to have law enforcement look the other way");
                break;
            case IntrigueAction.InduceToEmbezzle:
                sb.Append(" in order to secure embezzled funds");
                break;
            case IntrigueAction.CorruptInPlace:
                sb.Append(" in order to have an agent");
                break;
            case IntrigueAction.BringIntoNetwork:
                sb.Append(" in order to have someone to act on plots and schemes");
                break;
        }
        if (Site != null)
        {
            sb.Append(" in ");
            sb.Append(Site?.ToLink(link, pov, this));
        }
        else if (Region != null)
        {
            sb.Append(" in ");
            sb.Append(Region?.ToLink(link, pov, this));
        }
        else if (UndergroundRegion != null)
        {
            sb.Append(" in ");
            sb.Append(UndergroundRegion?.ToLink(link, pov, this));
        }
        else
        {
            sb.Append(" in the wilds");
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(". ");
        if (LureHf != null)
        {
            sb.Append(LureHf.ToLink(link, pov, this).ToUpperFirstLetter());
            sb.Append(" lured ");
            sb.Append(TargetHf?.ToLink(link, pov, this));
            sb.Append(" into a meeting");
        }
        else
        {
            sb.Append(CorruptorHf?.ToLink(link, pov, this).ToUpperFirstLetter());
            sb.Append(" met with ");
            sb.Append(TargetHf?.ToLink(link, pov, this));
        }
        sb.Append(" and, while completely misreading the situation, ");
        switch (Method)
        {
            case IntrigueMethod.Intimidate:
                sb.Append("made a threat. ");
                break;
            case IntrigueMethod.Flatter:
                sb.Append("made flattering remarks. ");
                break;
            case IntrigueMethod.Bribe:
                sb.Append("offered a bribe. ");
                break;
            case IntrigueMethod.Precedence:
                sb.Append("pulled rank. ");
                break;
            case IntrigueMethod.OfferImmortality:
                sb.Append("offered immortality. ");
                break;
            case IntrigueMethod.ReligiousSympathy:
                sb.Append($"played on sympathy by appealing to a shared worship of {World?.GetHistoricalFigure(RelevantIdForMethod)?.ToLink(link, pov, this)}. ");
                break;
            case IntrigueMethod.BlackmailOverEmbezzlement:
                var position = RelevantEntity?.EntityPositions.Find(p => p.Id == RelevantPositionProfileId);
                sb.Append($"made a blackmail threat, due to embezzlement using the position {position?.Name} of {RelevantEntity?.ToLink(link, pov, this)}. ");
                break;
            case IntrigueMethod.RevengeOnGrudge:
                sb.Append($"offered revenge upon  {World?.GetHistoricalFigure(RelevantIdForMethod)?.ToLink(link, pov, this)}. ");
                break;
        }
        sb.Append("The plan failed.");
        return sb.ToString();
    }
}
