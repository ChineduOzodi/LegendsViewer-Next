using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class SneakIntoSite : WorldEvent
{
    public Entity? Attacker { get; set; }
    public Entity? Defender { get; set; }
    public Entity? SiteCiv { get; set; }
    public Site? Site { get; set; }

    public SneakIntoSite(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "attacker_civ_id": Attacker = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "defender_civ_id": Defender = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_civ_id": SiteCiv = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
            }
        }

        Attacker?.AddEvent(this);
        Defender?.AddEvent(this);
        SiteCiv?.AddEvent(this);
        Site?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        string eventString = GetYearTime();
        eventString += Attacker?.ToLink(true, pov) ?? "an unknown group";
        eventString += " slipped into ";
        eventString += Site?.ToLink(true, pov) ?? "an unknown site";
        if (SiteCiv != null)
        {
            eventString += " undetected by ";
            eventString += SiteCiv.ToLink(true, pov);
            eventString += " of ";
            eventString += Defender?.ToLink(true, pov) ?? "an unknown group";
        }
        eventString += PrintParentCollection(link, pov);
        eventString += ".";
        return eventString;
    }
}


