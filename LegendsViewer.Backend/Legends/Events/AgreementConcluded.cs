using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using System.Text;

namespace LegendsViewer.Backend.Legends.Events;

public class AgreementConcluded : WorldEvent
{
    public Entity? Source { get; set; }
    public Entity? Destination { get; set; }
    public Site? Site { get; set; }
    public AgreementTopic Topic { get; set; }
    public int Result { get; set; }

    public AgreementConcluded(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "topic":
                    switch (property.Value)
                    {
                        case "treequota": Topic = AgreementTopic.TreeQuota; break;
                        case "becomelandholder": Topic = AgreementTopic.BecomeLandHolder; break;
                        case "promotelandholder": Topic = AgreementTopic.PromoteLandHolder; break;
                        case "tributeagreement":
                        case "unknown 9": Topic = AgreementTopic.Tribute; break;
                        default:
                            Topic = AgreementTopic.Unknown;
                            property.Known = false;
                            break;
                    }
                    break;
                case "source": Source = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "destination": Destination = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site": if (Site == null) { Site = world.GetSite(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
                case "result": Result = Convert.ToInt32(property.Value); break;
            }
        }

        Site?.AddEvent(this);
        Source?.AddEvent(this);
        Destination?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var eventString = new StringBuilder();
        eventString.Append(GetYearTime());
        switch (Topic)
        {
            case AgreementTopic.TreeQuota:
                eventString.Append("a lumber agreement between ");
                break;
            case AgreementTopic.BecomeLandHolder:
                eventString.Append("the establishment of landed nobility agreement between ");
                break;
            case AgreementTopic.PromoteLandHolder:
                eventString.Append("the elevation of the landed nobility agreement between ");
                break;
            case AgreementTopic.Tribute:
                eventString.Append("a tribute agreement between ");
                break;
            default:
                eventString.Append("UNKNOWN AGREEMENT");
                break;
        }
        eventString.Append(Source != null ? Source.ToLink(link, pov, this) : "UNKNOWN ENTITY");
        eventString.Append(" and ");
        eventString.Append(Destination != null ? Destination.ToLink(link, pov, this) : "UNKNOWN ENTITY");
        eventString.Append(" at ");
        eventString.Append(Site != null ? Site.ToLink(link, pov, this) : "UNKNOWN SITE");
        eventString.Append(" concluded");
        switch (Result)
        {
            case -3:
                eventString.Append("  with miserable outcome");
                break;
            case -2:
                eventString.Append(" with a strong negative outcome");
                break;
            case -1:
                eventString.Append(" in an unsatisfactory fashion");
                break;
            case 0:
                eventString.Append(" fairly");
                break;
            case 1:
                eventString.Append(" with a positive outcome");
                break;
            case 2:
                eventString.Append(", cementing bonds of mutual trust");
                break;
            case 3:
                eventString.Append(" with a very strong positive outcome");
                break;
            default:
                eventString.Append(" with an unknown outcome");
                break;
        }
        eventString.Append(PrintParentCollection(link, pov));
        eventString.Append('.');
        return eventString.ToString();
    }
}

