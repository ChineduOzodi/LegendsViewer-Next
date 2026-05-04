using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class HfDestroyedSite : WorldEvent
{
    public HistoricalFigure? Attacker { get; set; }
    public Entity? DefenderCiv { get; set; }
    public Entity? SiteCiv { get; set; }
    public Site? Site { get; set; }

    public HfDestroyedSite(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "attacker_hfid":
                    Attacker = world.GetHistoricalFigure(Convert.ToInt32(property.Value));
                    break;
                case "defender_civ_id":
                    DefenderCiv = world.GetEntity(Convert.ToInt32(property.Value));
                    break;
                case "site_civ_id":
                    SiteCiv = world.GetEntity(Convert.ToInt32(property.Value));
                    break;
                case "site_id":
                    Site = world.GetSite(Convert.ToInt32(property.Value));
                    break;
            }
        }

        Attacker.AddEvent(this);
        DefenderCiv.AddEvent(this);
        SiteCiv.AddEvent(this);
        Site.AddEvent(this);

        OwnerPeriod? lastSiteOwnerPeriod = Site?.OwnerHistory.LastOrDefault();
        if (lastSiteOwnerPeriod != null)
        {
            lastSiteOwnerPeriod.EndYear = Year;
            lastSiteOwnerPeriod.EndCause = "destroyed";
            lastSiteOwnerPeriod.Destroyer = Attacker;
        }
        OwnerPeriod? lastDefenderCivOwnerPeriod = DefenderCiv?.SiteHistory.LastOrDefault(s => s.Site == Site);
        if (lastDefenderCivOwnerPeriod != null)
        {
            lastDefenderCivOwnerPeriod.EndYear = Year;
            lastDefenderCivOwnerPeriod.EndCause = "destroyed";
            lastDefenderCivOwnerPeriod.Destroyer = Attacker;
        }
        OwnerPeriod? lastSiteCivOwnerPeriod = SiteCiv?.SiteHistory.LastOrDefault(s => s.Site == Site);
        if (lastSiteCivOwnerPeriod != null)
        {
            lastSiteCivOwnerPeriod.EndYear = Year;
            lastSiteCivOwnerPeriod.EndCause = "destroyed";
            lastSiteCivOwnerPeriod.Destroyer = Attacker;
        }
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Attacker?.ToLink(link, pov, this));
        sb.Append(" routed ");
        sb.Append(SiteCiv?.ToLink(link, pov, this));
        if (DefenderCiv != null)
        {
            sb.Append(" of ");
            sb.Append(DefenderCiv.ToLink(link, pov, this));
        }
        sb.Append(" and destroyed ");
        sb.Append(Site?.ToLink(link, pov, this));
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}
