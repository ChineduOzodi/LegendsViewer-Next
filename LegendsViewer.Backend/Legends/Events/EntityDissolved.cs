using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class EntityDissolved : WorldEvent
{
    public Entity? Entity { get; set; }
    public DissolveReason Reason { get; set; }
    public string? ReasonString { get; set; }

    public EntityDissolved(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "entity_id": Entity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "reason":
                    ReasonString = property.Value;
                    switch (property.Value)
                    {
                        case "heavy losses in battle": Reason = DissolveReason.HeavyLossesInBattle; break;
                        case "lack of funds": Reason = DissolveReason.LackOfFunds; break;
                        default:
                            property.Known = false;
                            break;
                    }
                    break;
            }
        }

        Entity?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Entity?.ToLink(link, pov, this));
        sb.Append(" dissolved");
        switch (Reason)
        {
            case DissolveReason.HeavyLossesInBattle:
                sb.Append(" taking ");
                break;
            case DissolveReason.LackOfFunds:
                sb.Append(" due to ");
                break;
            default:
                sb.Append(" because of ");
                break;
        }
        sb.Append(ReasonString);

        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}

