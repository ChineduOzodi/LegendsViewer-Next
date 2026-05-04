using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class SiteDied : WorldEvent
{
    public Entity? Civ { get; set; }
    public Entity? SiteEntity { get; set; }
    public Site? Site { get; set; }

    public bool Abandoned;

    public SiteDied(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "civ_id": Civ = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_civ_id": SiteEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "abandoned":
                    property.Known = true;
                    Abandoned = true;
                    break;
            }
        }

        string endCause = "withered";
        if (Abandoned)
        {
            endCause = "abandoned";
        }

        if (Site != null)
        {
            Site.OwnerHistory.Last().EndYear = Year;
            Site.OwnerHistory.Last().EndCause = endCause;
            world.AddPlayerRelatedDwarfObjects(Site);
        }
        if (SiteEntity != null)
        {
            SiteEntity.SiteHistory.Last(s => s.Site == Site).EndYear = Year;
            SiteEntity.SiteHistory.Last(s => s.Site == Site).EndCause = endCause;
            world.AddPlayerRelatedDwarfObjects(SiteEntity);
        }
        if (Civ != null)
        {
            Civ.SiteHistory.Last(s => s.Site == Site).EndYear = Year;
            Civ.SiteHistory.Last(s => s.Site == Site).EndCause = endCause;
        }

        Civ.AddEvent(this);
        SiteEntity.AddEvent(this);
        Site.AddEvent(this);

    }
    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(SiteEntity?.PrintEntity(link, pov));
        if (Abandoned)
        {
            sb.Append(" abandoned the settlement of ");
            sb.Append(Site?.ToLink(link, pov, this));
        }
        else
        {
            sb.Append(" settlement of ");
            sb.Append(Site?.ToLink(link, pov, this));
            sb.Append(" withered");
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}

