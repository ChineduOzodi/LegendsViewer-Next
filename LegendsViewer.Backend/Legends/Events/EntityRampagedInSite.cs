using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class EntityRampagedInSite : WorldEvent
{
    public Entity? RampageCiv { get; set; }
    public Site? Site { get; set; }

    public EntityRampagedInSite(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "rampage_civ_id": RampageCiv = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
            }
        }

        RampageCiv.AddEvent(this);
        Site.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        string eventString = GetYearTime();
        eventString += " the forces of ";
        eventString += RampageCiv?.ToLink(true, pov) ?? "an unknown civilization";
        eventString += " rampaged throughout ";
        eventString += Site?.ToLink(true, pov) ?? "an unknown site";
        eventString += PrintParentCollection(link, pov);
        eventString += ".";
        return eventString;
    }
}


