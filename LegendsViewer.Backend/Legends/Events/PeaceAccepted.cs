using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;

namespace LegendsViewer.Backend.Legends.Events;

public class PeaceAccepted : PeaceEfforts
{
    public PeaceAccepted(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        Decision = "accepted";
    }
}

