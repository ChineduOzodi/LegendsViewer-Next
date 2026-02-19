using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.WorldObjects;

public class Era : WorldObject
{
    public int StartYear { get; set; }
    public int EndYear { get; set; }

    public Era(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        Id = world.Eras.Count;

        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "start_year": StartYear = Convert.ToInt32(property.Value); break;
                case "name": Name = property.Value; break;
            }
        }
        Icon = HtmlStyleUtil.GetIconString("timelapse");
    }

    public override string ToLink(bool link = true, DwarfObject? pov = null, WorldEvent? worldEvent = null)
    {
        if (link)
        {
            string title = "Era";
            title += "&#13";
            title += "Events: " + Events.Count;
            return pov != this
                ? HtmlStyleUtil.GetAnchorString(Icon, "era", Id, title, Name)
                : HtmlStyleUtil.GetAnchorCurrentString(Icon, title, HtmlStyleUtil.CurrentDwarfObject(Name));
        }
        return Name;
    }

    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(Name) ? Subtype : Name;
    }
}


