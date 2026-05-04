using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class CreatedSite : WorldEvent
{
    public Entity? Civ { get; set; }
    public Entity? ResidentCiv { get; set; }
    public Entity? SiteEntity { get; set; }
    public Site? Site { get; set; }
    public HistoricalFigure? Builder { get; set; }

    public CreatedSite(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "civ_id": Civ = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "resident_civ_id": ResidentCiv = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_civ_id": SiteEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "builder_hfid": Builder = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
            }
        }

        if (ResidentCiv != null)
        {
            Site?.OwnerHistory.Add(new OwnerPeriod(Site, ResidentCiv, Year, "constructed", Builder));
        }
        else if (SiteEntity != null)
        {
            if (Civ != null)
            {
                SiteEntity.SetParent(Civ);
            }

            Site?.OwnerHistory.Add(new OwnerPeriod(Site, SiteEntity, Year, "founded", Builder));
        }
        else if (Civ != null)
        {
            Site?.OwnerHistory.Add(new OwnerPeriod(Site, Civ, Year, "founded", Builder));
        }
        ResidentCiv?.AddEvent(this);
        Site?.AddEvent(this);
        SiteEntity?.AddEvent(this);
        Civ?.AddEvent(this);
        Builder?.AddEvent(this);
    }
    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        if (Builder != null)
        {
            sb.Append(Builder.ToLink(link, pov, this));
            sb.Append(" constructed ");
            sb.Append(Site?.ToLink(link, pov, this));
            if (ResidentCiv != null)
            {
                sb.Append(" for ");
                sb.Append(ResidentCiv.ToLink(link, pov, this));
            }
        }
        else
        {
            if (SiteEntity != null)
            {
                sb.Append(SiteEntity.ToLink(link, pov, this));
                sb.Append(" of ");
            }

            sb.Append(Civ?.ToLink(link, pov, this));
            sb.Append(" founded ");
            sb.Append(Site?.ToLink(link, pov, this));
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append('.');
        return sb.ToString();
    }
}

