using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class EntityFledSite : WorldEvent
{
    public Entity? FledCiv { get; set; }
    public Site? Site { get; set; }

    public EntityFledSite(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "fled_civ_id": FledCiv = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
            }
        }

        FledCiv?.AddEvent(this);
        Site?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        string eventString = GetYearTime();
        eventString += FledCiv?.ToLink(true, pov) ?? "an unknown civilization";
        eventString += " fled ";
        eventString += Site?.ToLink(true, pov) ?? "an unknown site";
        eventString += PrintParentCollection(link, pov);
        eventString += ".";
        return eventString;
    }
}


