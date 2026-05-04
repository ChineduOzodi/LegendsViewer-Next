using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;

namespace LegendsViewer.Backend.Legends.Events;

public class PoeticFormCreated : FormCreatedEvent
{
    public PoeticFormCreated(List<Property> properties, IWorld world) : base(properties, world)
    {
        FormType = FormType.Poetic;
        if (!string.IsNullOrWhiteSpace(FormId))
        {
            ArtForm = world.GetPoeticForm(Convert.ToInt32(FormId));
            ArtForm.AddEvent(this);
        }
    }
}

