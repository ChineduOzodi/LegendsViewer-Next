using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;

namespace LegendsViewer.Backend.Legends.Events;

public class PeaceRejected : PeaceEfforts
{
    public PeaceRejected(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        Decision = "rejected";
    }
}

