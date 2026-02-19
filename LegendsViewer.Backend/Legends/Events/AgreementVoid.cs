using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;

namespace LegendsViewer.Backend.Legends.Events;

public class AgreementVoid : WorldEvent
{
    public AgreementVoid(List<Property> properties, IWorld world) : base(properties, world)
    {
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        string eventString = GetYearTime();
        eventString += " an agreement has been annulated";
        eventString += PrintParentCollection(link, pov);
        eventString += ".";
        return eventString;
    }
}

