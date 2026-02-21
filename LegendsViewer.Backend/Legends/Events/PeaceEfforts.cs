using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class PeaceEfforts : WorldEvent
{
    public string? Decision { get; set; }
    public string? Topic { get; set; }
    public Entity? Source { get; set; }
    public Entity? Destination { get; set; }
    public Site? Site { get; set; }

    public PeaceEfforts(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "site": if (Site == null) { Site = world.GetSite(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
                case "topic": Topic = property.Value; break;
                case "source": Source = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "destination": Destination = world.GetEntity(Convert.ToInt32(property.Value)); break;
            }
        }

        Site.AddEvent(this);
        Source.AddEvent(this);
        Destination.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        if (Source != null && Destination != null)
        {
            sb.Append(Destination.ToLink(link, pov, this));
            sb.Append(" ");
            sb.Append(Decision);
            sb.Append(" an offer of peace from ");
            sb.Append(Source.ToLink(link, pov, this));
            sb.Append(" in ");
            sb.Append(ParentCollection?.ToLink(link, pov, this));
            sb.Append(".");
        }
        else
        {
            sb.Append("Peace ");
            sb.Append(Decision);
            sb.Append(" in ");
            sb.Append(ParentCollection?.ToLink(link, pov, this));
            sb.Append(".");
        }
        return sb.ToString();
    }
}
