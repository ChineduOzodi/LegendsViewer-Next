using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class DiplomatLost : WorldEvent
{
    public Entity? Entity { get; set; }
    public Entity? InvolvedEntity { get; set; }
    public Site? Site { get; set; }

    public DiplomatLost(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "site": if (Site == null) { Site = world.GetSite(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
                case "entity": Entity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "involved": InvolvedEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
            }
        }

        Site?.AddEvent(this);
        Entity?.AddEvent(this);
        InvolvedEntity?.AddEvent(this);
    }
    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        string eventString = GetYearTime();
        eventString += Entity != null ? Entity.ToLink(link, pov, this) : "UNKNOWN ENTITY";
        eventString += " lost a diplomat at ";
        eventString += Site != null ? Site.ToLink(link, pov, this) : "UNKNOWN SITE";
        eventString += ". They suspected the involvement of ";
        eventString += InvolvedEntity != null ? InvolvedEntity.ToLink(link, pov, this) : "UNKNOWN ENTITY";
        eventString += PrintParentCollection(link, pov);
        eventString += ".";
        return eventString;
    }
}

