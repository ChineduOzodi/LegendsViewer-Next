using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class SiteRetired : WorldEvent
{
    public Entity? Civ { get; set; }
    public Entity? SiteEntity { get; set; }
    public Site? Site { get; set; }
    public string? First { get; set; } // TODO

    public SiteRetired(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "civ_id": Civ = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_civ_id": SiteEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "first": First = property.Value; break;
            }
        }
        if (Site != null)
        {
            world.AddPlayerRelatedDwarfObjects(Site);
        }
        if (SiteEntity != null)
        {
            world.AddPlayerRelatedDwarfObjects(SiteEntity);
        }

        Civ.AddEvent(this);
        SiteEntity.AddEvent(this);
        Site.AddEvent(this);
    }
    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(SiteEntity != null ? SiteEntity.ToLink(link, pov, this) : "UNKNOWN ENTITY");
        sb.Append(" of ");
        sb.Append(Civ != null ? Civ.ToLink(link, pov, this) : "UNKNOWN CIV");
        sb.Append(" at the settlement of ");
        sb.Append(Site != null ? Site.ToLink(link, pov, this) : "UNKNOWN SITE");
        sb.Append(" regained their senses after an initial period of questionable judgment.");
        return sb.ToString();
    }
}

