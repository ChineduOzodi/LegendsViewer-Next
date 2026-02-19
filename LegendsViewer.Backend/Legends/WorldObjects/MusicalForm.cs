using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.WorldObjects;

public class MusicalForm : ArtForm
{
    public MusicalForm(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        FormType = FormType.Musical;
        Type = FormType.GetDescription();
        Icon = HtmlStyleUtil.GetIconString("music-clef-treble");
    }

    public override string ToLink(bool link = true, DwarfObject? pov = null, WorldEvent? worldEvent = null)
    {
        if (link)
        {
            string title = "Musical Form";
            title += "&#13";
            title += "Events: " + Events.Count;

            return pov != this
                ? HtmlStyleUtil.GetAnchorString(Icon, "musicalform", Id, title, Name)
                : HtmlStyleUtil.GetAnchorCurrentString(Icon, title, HtmlStyleUtil.CurrentDwarfObject(Name));
        }
        return Name;
    }

    public override string GetIcon()
    {
        return Icon;
    }
}


