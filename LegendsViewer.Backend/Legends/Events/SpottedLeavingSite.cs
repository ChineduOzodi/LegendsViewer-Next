using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class SpottedLeavingSite : WorldEvent
{
    public HistoricalFigure? Spotter { get; set; }
    public Entity? LeaverCiv { get; set; }
    public Entity? SiteCiv { get; set; }
    public Site? Site { get; set; }

    public SpottedLeavingSite(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "spotter_hfid": Spotter = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "leaver_civ_id": LeaverCiv = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_civ_id": SiteCiv = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
            }
        }

        Spotter.AddEvent(this);
        LeaverCiv.AddEvent(this);
        SiteCiv.AddEvent(this);
        Site.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Spotter?.ToLink(true, pov) ?? "An unknown creature");
        if (SiteCiv != null)
        {
            sb.Append(" of ");
            sb.Append(SiteCiv.ToLink(true, pov));
        }
        sb.Append(" spotted the forces");
        if (LeaverCiv != null)
        {
            sb.Append(" of ");
            sb.Append(LeaverCiv.ToLink(true, pov));
        }
        sb.Append(" slipping out");
        if (Site != null)
        {
            sb.Append(" of ");
            sb.Append(Site.ToLink(true, pov));
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}


