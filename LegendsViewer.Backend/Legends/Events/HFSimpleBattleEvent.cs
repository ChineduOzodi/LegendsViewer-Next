using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class HfSimpleBattleEvent : WorldEvent
{
    public HfSimpleBattleType SubType { get; set; }
    public string? UnknownSubType { get; set; }
    public HistoricalFigure? HistoricalFigure1 { get; set; }
    public HistoricalFigure? HistoricalFigure2 { get; set; }
    public Site? Site { get; set; }
    public WorldRegion? Region { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }

    public HfSimpleBattleEvent(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "subtype":
                    switch (property.Value)
                    {
                        case "attacked": SubType = HfSimpleBattleType.Attacked; break;
                        case "scuffle": SubType = HfSimpleBattleType.Scuffle; break;
                        case "confront": SubType = HfSimpleBattleType.Confronted; break;
                        case "2 lost after receiving wounds": SubType = HfSimpleBattleType.Hf2LostAfterReceivingWounds; break;
                        case "2 lost after giving wounds": SubType = HfSimpleBattleType.Hf2LostAfterGivingWounds; break;
                        case "2 lost after mutual wounds": SubType = HfSimpleBattleType.Hf2LostAfterMutualWounds; break;
                        case "happen upon": SubType = HfSimpleBattleType.HappenedUpon; break;
                        case "ambushed": SubType = HfSimpleBattleType.Ambushed; break;
                        case "corner": SubType = HfSimpleBattleType.Cornered; break;
                        case "surprised": SubType = HfSimpleBattleType.Surprised; break;
                        case "got into a brawl": SubType = HfSimpleBattleType.GotIntoABrawl; break;
                        case "subdued": SubType = HfSimpleBattleType.Subdued; break;
                        default: SubType = HfSimpleBattleType.Unknown; UnknownSubType = property.Value; property.Known = false; break;
                    }
                    break;
                case "group_1_hfid": HistoricalFigure1 = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "group_2_hfid": HistoricalFigure2 = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
            }
        }

        HistoricalFigure1.AddEvent(this);
        HistoricalFigure2.AddEvent(this);
        Site.AddEvent(this);
        Region.AddEvent(this);
        UndergroundRegion.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());

        switch (SubType)
        {
            case HfSimpleBattleType.Hf2LostAfterGivingWounds:
                sb.Append(HistoricalFigure2?.ToLink(link, pov, this));
                sb.Append(" was forced to retreat from ");
                sb.Append(HistoricalFigure1?.ToLink(link, pov, this));
                sb.Append(" despite the latter's wounds");
                break;
            case HfSimpleBattleType.Hf2LostAfterMutualWounds:
                sb.Append(HistoricalFigure1?.ToLink(link, pov, this));
                sb.Append(" eventually prevailed and ");
                sb.Append(HistoricalFigure2?.ToLink(link, pov, this));
                sb.Append(" was forced to make a hasty escape");
                break;
            case HfSimpleBattleType.Hf2LostAfterReceivingWounds:
                sb.Append(HistoricalFigure2?.ToLink(link, pov, this));
                sb.Append(" managed to escape from ");
                sb.Append(HistoricalFigure1?.ToLink(link, pov, this));
                sb.Append("'s onslaught");
                break;
            case HfSimpleBattleType.Scuffle:
                sb.Append(HistoricalFigure1?.ToLink(link, pov, this));
                sb.Append(" fought with ");
                sb.Append(HistoricalFigure2?.ToLink(link, pov, this));
                sb.Append(". While defeated, the latter escaped unscathed");
                break;
            case HfSimpleBattleType.Attacked:
                sb.Append(HistoricalFigure1?.ToLink(link, pov, this));
                sb.Append(" attacked ");
                sb.Append(HistoricalFigure2?.ToLink(link, pov, this));
                break;
            case HfSimpleBattleType.Confronted:
                sb.Append(HistoricalFigure1?.ToLink(link, pov, this));
                sb.Append(" confronted ");
                sb.Append(HistoricalFigure2?.ToLink(link, pov, this));
                break;
            case HfSimpleBattleType.HappenedUpon:
                sb.Append(HistoricalFigure1?.ToLink(link, pov, this));
                sb.Append(" happened upon ");
                sb.Append(HistoricalFigure2?.ToLink(link, pov, this));
                break;
            case HfSimpleBattleType.Ambushed:
                sb.Append(HistoricalFigure1?.ToLink(link, pov, this));
                sb.Append(" ambushed ");
                sb.Append(HistoricalFigure2?.ToLink(link, pov, this));
                break;
            case HfSimpleBattleType.Cornered:
                sb.Append(HistoricalFigure1?.ToLink(link, pov, this));
                sb.Append(" cornered ");
                sb.Append(HistoricalFigure2?.ToLink(link, pov, this));
                break;
            case HfSimpleBattleType.Surprised:
                sb.Append(HistoricalFigure1?.ToLink(link, pov, this));
                sb.Append(" surprised ");
                sb.Append(HistoricalFigure2?.ToLink(link, pov, this));
                break;
            case HfSimpleBattleType.GotIntoABrawl:
                sb.Append(HistoricalFigure1?.ToLink(link, pov, this));
                sb.Append(" got into a brawl with ");
                sb.Append(HistoricalFigure2?.ToLink(link, pov, this));
                break;
            case HfSimpleBattleType.Subdued:
                sb.Append(HistoricalFigure1?.ToLink(link, pov, this));
                sb.Append(" fought with and subdued ");
                sb.Append(HistoricalFigure2?.ToLink(link, pov, this));
                break;
            default:
                sb.Append(HistoricalFigure1?.ToLink(link, pov, this));
                sb.Append(" fought (");
                sb.Append(UnknownSubType);
                sb.Append(") ");
                sb.Append(HistoricalFigure2?.ToLink(link, pov, this));
                break;
        }

        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}
