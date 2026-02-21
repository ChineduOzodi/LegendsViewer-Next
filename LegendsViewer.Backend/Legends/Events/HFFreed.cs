using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class HfFreed : WorldEvent
{
    public List<HistoricalFigure> RescuedHistoricalFigures { get; set; } = [];
    public HistoricalFigure? FreeingHf { get; set; }
    public Entity? FreeingCiv { get; set; }
    public Entity? SiteCiv { get; set; }
    public Entity? HoldingCiv { get; set; }
    public Site? Site { get; set; }

    public HfFreed(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "freeing_civ_id": FreeingCiv = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "freeing_hfid": FreeingHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "rescued_hfid":
                    HistoricalFigure? rescuedHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value));
                    if (rescuedHf != null)
                    {
                        RescuedHistoricalFigures.Add(rescuedHf);
                    }
                    break;
                case "site_civ_id": SiteCiv = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "holding_civ_id": HoldingCiv = world.GetEntity(Convert.ToInt32(property.Value)); break;
            }
        }
        foreach (var rescuedHistoricalFigure in RescuedHistoricalFigures)
        {
            rescuedHistoricalFigure.AddEvent(this);
        }
        FreeingCiv.AddEvent(this);
        FreeingHf.AddEvent(this);
        Site.AddEvent(this);
        SiteCiv.AddEvent(this);
        HoldingCiv.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        if (FreeingHf != null)
        {
            sb.Append(FreeingHf?.ToLink(link, pov, this) ?? "an unknown creature");
        }
        else
        {
            sb.Append("the forces of ");
            sb.Append(FreeingCiv?.ToLink(link, pov, this) ?? "an unknown civilization");
        }
        sb.Append(" freed ");
        for (int i = 0; i < RescuedHistoricalFigures.Count; i++)
        {
            if (i > 0)
            {
                sb.Append(" and ");
            }
            sb.Append(RescuedHistoricalFigures[i]?.ToLink(link, pov, this) ?? "an unknown creature");
        }
        if (Site != null)
        {
            sb.Append(" from ");
            sb.Append(Site.ToLink(link, pov, this));
        }
        if (SiteCiv != null)
        {
            sb.Append(" and ");
            sb.Append(SiteCiv.ToLink(link, pov, this));
        }
        if (HoldingCiv != null)
        {
            sb.Append(" of ");
            sb.Append(HoldingCiv.ToLink(link, pov, this));
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}
