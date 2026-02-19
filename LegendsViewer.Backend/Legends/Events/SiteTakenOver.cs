using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class SiteTakenOver : WorldEvent
{
    public Entity? Attacker { get; set; }
    public Entity? Defender { get; set; }
    public Entity? SiteEntity { get; set; }
    public Site? Site { get; set; }
    public Entity? NewSiteEntity { get; set; }

    public SiteTakenOver(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "attacker_civ_id":
                    Attacker = world.GetEntity(Convert.ToInt32(property.Value));
                    break;
                case "defender_civ_id":
                    Defender = world.GetEntity(Convert.ToInt32(property.Value));
                    break;
                case "new_site_civ_id":
                    NewSiteEntity = world.GetEntity(Convert.ToInt32(property.Value));
                    break;
                case "site_civ_id":
                    SiteEntity = world.GetEntity(Convert.ToInt32(property.Value));
                    break;
                case "site_id":
                    Site = world.GetSite(Convert.ToInt32(property.Value));
                    break;
            }
        }

        if (Site?.OwnerHistory.Count == 0)
        {
            if (SiteEntity != null)
            {
                SiteEntity.SetParent(Defender);
                Site.OwnerHistory.Add(new OwnerPeriod(Site, SiteEntity, -1, "founded"));
            }
        }

        NewSiteEntity?.SetParent(Attacker);

        if (Site != null)
        {
            Site.OwnerHistory.Last().EndCause = "taken over";
            Site.OwnerHistory.Last().EndYear = Year;
            Site.OwnerHistory.Last().Ender = Attacker;
            Site.OwnerHistory.Add(new OwnerPeriod(Site, NewSiteEntity, Year, "took over"));
        }

        Attacker.AddEvent(this);
        if (Defender != Attacker)
        {
            Defender.AddEvent(this);
        }
        NewSiteEntity.AddEvent(this);
        if (SiteEntity != Defender)
        {
            SiteEntity.AddEvent(this);
        }

        Site.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        string eventString = GetYearTime() + Attacker?.ToLink(link, pov, this) + " defeated ";
        if (SiteEntity != null && SiteEntity != Defender)
        {
            eventString += SiteEntity.ToLink(link, pov, this);
            if (Defender != null)
            {
                eventString += " of ";
            }
        }

        if (Defender != null)
        {
            eventString += Defender.ToLink(link, pov, this);
        }
        eventString += " and took over " + Site?.ToLink(link, pov, this) + ". The new government was called " + NewSiteEntity?.ToLink(link, pov, this);
        eventString += PrintParentCollection(link, pov);
        eventString += ".";
        return eventString;
    }
}

