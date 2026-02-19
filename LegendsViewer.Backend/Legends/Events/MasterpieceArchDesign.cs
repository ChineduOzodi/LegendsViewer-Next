using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;

namespace LegendsViewer.Backend.Legends.Events;

public class MasterpieceArchDesign : MasterpieceArch
{
    public MasterpieceArchDesign(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        Process = "designed";
    }
}

