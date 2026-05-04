using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;

namespace LegendsViewer.Backend.Legends.Events;

public class MasterpieceArchConstructed : MasterpieceArch
{
    public MasterpieceArchConstructed(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        Process = "constructed";
    }
}

