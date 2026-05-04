using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Parser;

namespace LegendsViewer.Backend.Legends.Events;

public class Ceremony : OccasionEvent
{
    public Ceremony(List<Property> properties, IWorld world) : base(properties, world)
    {
        OccasionType = OccasionType.Ceremony;
    }
}

