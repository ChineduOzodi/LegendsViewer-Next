using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using System.Text;

namespace LegendsViewer.Backend.Legends.Events;

public class AgreementVoid : WorldEvent
{
    public AgreementVoid(List<Property> properties, IWorld world) : base(properties, world)
    {
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var eventString = new StringBuilder();
        eventString.Append(GetYearTime());
        eventString.Append(" an agreement has been annulated");
        eventString.Append(PrintParentCollection(link, pov));
        eventString.Append('.');
        return eventString.ToString();
    }
}

