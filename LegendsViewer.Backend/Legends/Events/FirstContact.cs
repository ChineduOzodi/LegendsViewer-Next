using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class FirstContact : WorldEvent
{
    public Site? Site;
    public Entity? Contactor;
    public Entity? Contacted;
    public FirstContact(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "contactor_enid": Contactor = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "contacted_enid": Contacted = world.GetEntity(Convert.ToInt32(property.Value)); break;
            }
        }
        Site.AddEvent(this);
        Contactor.AddEvent(this);
        Contacted.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Contactor != null ? Contactor.ToLink(link, pov, this) : "UNKNOWN ENTITY");
        sb.Append(" made contact with ");
        sb.Append(Contacted != null ? Contacted.ToLink(link, pov, this) : "UNKNOWN ENTITY");
        sb.Append(" at ");
        sb.Append(Site != null ? Site.ToLink(link, pov, this) : "UNKNOWN SITE");
        return sb.ToString();
    }
}
