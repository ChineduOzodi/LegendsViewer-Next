using System.Text;
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
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Attacker?.ToLink(link, pov, this));
        sb.Append(" defeated ");
        if (SiteEntity != null && SiteEntity != Defender)
        {
            sb.Append(SiteEntity.ToLink(link, pov, this));
            if (Defender != null)
            {
                sb.Append(" of ");
            }
        }

        if (Defender != null)
        {
            sb.Append(Defender.ToLink(link, pov, this));
        }
        sb.Append(" and took over ");
        sb.Append(Site?.ToLink(link, pov, this));
        sb.Append(". The new government was called ");
        sb.Append(NewSiteEntity?.ToLink(link, pov, this));
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}

