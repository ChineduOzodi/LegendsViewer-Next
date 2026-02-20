using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class BuildingProfileAcquired : WorldEvent
{
    public Site? Site { get; set; }
    public int BuildingProfileId { get; set; }
    public SiteProperty? SiteProperty { get; set; }
    public HistoricalFigure? AcquirerHf { get; set; }
    public Entity? AcquirerEntity { get; set; }
    public HistoricalFigure? LastOwnerHf { get; set; }
    public bool Inherited { get; set; }
    public bool RebuiltRuined { get; set; }
    public bool PurchasedUnowned { get; set; }

    // http://www.bay12games.com/dwarves/mantisbt/view.php?id=11346
    // 0011346: <acquirer_enid> is always -1 in "building profile acquired" event
    public BuildingProfileAcquired(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "acquirer_hfid": AcquirerHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "acquirer_enid": AcquirerEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "last_owner_hfid": LastOwnerHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "building_profile_id": BuildingProfileId = Convert.ToInt32(property.Value); break;
                case "purchased_unowned": property.Known = true; PurchasedUnowned = true; break;
                case "inherited": property.Known = true; Inherited = true; break;
                case "rebuilt_ruined": property.Known = true; RebuiltRuined = true; break;
            }
        }

        if (Site != null)
        {
            SiteProperty = Site.SiteProperties.Find(siteProperty => siteProperty.Id == BuildingProfileId);
            SiteProperty?.Structure?.AddEvent(this);
        }

        Site?.AddEvent(this);
        AcquirerHf?.AddEvent(this);
        AcquirerEntity?.AddEvent(this);
        LastOwnerHf?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        if (AcquirerHf != null)
        {
            sb.Append(AcquirerHf?.ToLink(link, pov, this));
            if (AcquirerEntity != null)
            {
                sb.Append(" of ");
            }
        }
        else if (AcquirerEntity != null)
        {
            sb.Append(AcquirerEntity.ToLink(link, pov, this));
        }
        else
        {
            sb.Append("Someone ");
        }
        if (PurchasedUnowned)
        {
            sb.Append(" purchased ");
        }
        else if (Inherited)
        {
            sb.Append(" inherited ");
        }
        else if (RebuiltRuined)
        {
            sb.Append(" rebuilt ");
        }
        else
        {
            sb.Append(" acquired ");
        }

        sb.Append(SiteProperty?.Print(link, pov));
        if (Site != null)
        {
            sb.Append(" in ");
            sb.Append(Site.ToLink(link, pov, this));
        }

        if (LastOwnerHf != null)
        {
            sb.Append(" formerly owned by ");
            sb.Append(LastOwnerHf.ToLink(link, pov, this));
        }

        sb.Append(PrintParentCollection(link, pov));
        sb.Append('.');
        return sb.ToString();
    }
}


