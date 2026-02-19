using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;

namespace LegendsViewer.Backend.Legends.Events;

public class MusicalFormCreated : FormCreatedEvent
{
    public MusicalFormCreated(List<Property> properties, IWorld world) : base(properties, world)
    {
        FormType = FormType.Musical;
        if (!string.IsNullOrWhiteSpace(FormId))
        {
            ArtForm = world.GetMusicalForm(Convert.ToInt32(FormId));
            ArtForm.AddEvent(this);
        }
    }
}

