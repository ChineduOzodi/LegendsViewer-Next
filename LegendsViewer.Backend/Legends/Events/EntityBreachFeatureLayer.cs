using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class EntityBreachFeatureLayer : WorldEvent
{
    public Entity? SiteEntity { get; set; }
    public Entity? CivEntity { get; set; }
    public Site? Site { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }

    // http://www.bay12games.com/dwarves/mantisbt/view.php?id=11335
    // 0011335: <site_entity_id> and <civ_entity_id> of "entity breach feature layer" event point both to same entity
    public EntityBreachFeatureLayer(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "site_entity_id": SiteEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "civ_entity_id": CivEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
            }
        }

        if (SiteEntity != CivEntity)
        {
            SiteEntity?.AddEvent(this);
        }
        CivEntity?.AddEvent(this);
        Site?.AddEvent(this);
        UndergroundRegion?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(SiteEntity?.ToLink(link, pov, this));
        sb.Append(" of ");
        sb.Append(CivEntity?.ToLink(link, pov, this));
        sb.Append(" breached ");
        sb.Append(UndergroundRegion?.ToLink(link, pov, this));
        if (Site != null)
        {
            sb.Append(" at ");
            sb.Append(Site.ToLink(link, pov, this));
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}

