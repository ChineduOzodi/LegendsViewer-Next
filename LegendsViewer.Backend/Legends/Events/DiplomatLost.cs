using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class DiplomatLost : WorldEvent
{
    public Entity? Entity { get; set; }
    public Entity? InvolvedEntity { get; set; }
    public Site? Site { get; set; }

    public DiplomatLost(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "site": if (Site == null) { Site = world.GetSite(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
                case "entity": Entity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "involved": InvolvedEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
            }
        }

        Site?.AddEvent(this);
        Entity?.AddEvent(this);
        InvolvedEntity?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Entity != null ? Entity.ToLink(link, pov, this) : "UNKNOWN ENTITY");
        sb.Append(" lost a diplomat at ");
        sb.Append(Site != null ? Site.ToLink(link, pov, this) : "UNKNOWN SITE");
        sb.Append(". They suspected the involvement of ");
        sb.Append(InvolvedEntity != null ? InvolvedEntity.ToLink(link, pov, this) : "UNKNOWN ENTITY");
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}

