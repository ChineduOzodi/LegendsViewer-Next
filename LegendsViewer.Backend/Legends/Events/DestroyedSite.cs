using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class DestroyedSite : WorldEvent
{
    public Site? Site { get; set; }
    public Entity? SiteEntity { get; set; }
    public Entity? Attacker { get; set; }
    public Entity? Defender { get; set; }
    public bool NoDefeatMention { get; set; }

    public DestroyedSite(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "site_civ_id": SiteEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "attacker_civ_id": Attacker = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "defender_civ_id": Defender = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "no_defeat_mention":
                    NoDefeatMention = true;
                    property.Known = true;
                    break;
            }
        }

        if (Site?.OwnerHistory.Count == 0)
        {
            if (Defender != null && SiteEntity != null)
            {
                SiteEntity.SetParent(Defender);
            }
            Site.OwnerHistory.Add(new OwnerPeriod(Site, SiteEntity, -1, "founded"));
        }

        if(Site != null)
        {
            Site.OwnerHistory.Last().EndCause = "destroyed";
            Site.OwnerHistory.Last().EndYear = Year;
            Site.OwnerHistory.Last().Ender = Attacker;

            Site?.AddEvent(this);
        }
        if (SiteEntity != Defender)
        {
            SiteEntity?.AddEvent(this);
        }

        Attacker?.AddEvent(this);
        Defender?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        string eventString = GetYearTime();
        eventString += Attacker?.ToLink(link, pov, this) ?? "an unknown entity";
        if (!NoDefeatMention)
        {
            eventString += " defeated ";
            if (SiteEntity != null && SiteEntity != Defender)
            {
                eventString += SiteEntity.ToLink(link, pov, this) + " of ";
            }

            eventString += Defender?.ToLink(link, pov, this) ?? "an unknown entity";
            eventString += " and";
        }
        eventString += " destroyed ";
        eventString += Site?.ToLink(link, pov, this) ?? "an unknown site";
        eventString += PrintParentCollection(link, pov);
        eventString += ".";
        return eventString;
    }
}

