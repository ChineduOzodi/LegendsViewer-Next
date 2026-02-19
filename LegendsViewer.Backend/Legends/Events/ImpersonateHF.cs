using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class ImpersonateHf : WorldEvent
{
    public HistoricalFigure? Trickster { get; set; }
    public HistoricalFigure? Cover { get; set; }
    public Entity? Target { get; set; }

    public ImpersonateHf(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "trickster_hfid": Trickster = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "cover_hfid": Cover = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "target_enid": Target = world.GetEntity(Convert.ToInt32(property.Value)); break;
            }
        }

        Trickster.AddEvent(this);
        Cover.AddEvent(this);
        Target.AddEvent(this);
    }
    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        string eventString = GetYearTime() + Trickster?.ToLink(link, pov, this) + " fooled " + Target?.ToLink(link, pov, this)
                             + " into believing he/she was a manifestation of the deity " + Cover?.ToLink(link, pov, this);
        eventString += PrintParentCollection(link, pov);
        eventString += ".";
        return eventString;
    }
}

