using System.Text;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.WorldObjects;

public class MountainPeak : WorldObject, IHasCoordinates
{
    public WorldRegion? Region { get; set; }
    public List<Location> Coordinates { get; set; } = [];
    public int Height { get; set; } // legends_plus.xml
    public string HeightMeter { get => Height * 3 + " m"; set { } } // legends_plus.xml
    public bool IsVolcano { get; set; }

    public string TypeAsString => IsVolcano ? "Volcano" : "Mountain";

    public MountainPeak(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        Icon = HtmlStyleUtil.GetIconString("summit");
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "name":
                    Name = Formatting.InitCaps(property.Value);
                    break;
                case "coords":
                    string[] coordinateStrings = property.Value.Split(new[] { '|' },
                        StringSplitOptions.RemoveEmptyEntries);
                    foreach (var coordinateString in coordinateStrings)
                    {
                        string[] xYCoordinates = coordinateString.Split(',');
                        int x = Convert.ToInt32(xYCoordinates[0]);
                        int y = Convert.ToInt32(xYCoordinates[1]);
                        Coordinates.Add(new Location(x, y));
                    }
                    break;
                case "height":
                    Height = Convert.ToInt32(property.Value);
                    break;
                case "is_volcano":
                    IsVolcano = true;
                    Icon = HtmlStyleUtil.GetIconString("volcano-outline");
                    property.Known = true;
                    break;
            }
        }
        if(string.IsNullOrEmpty(Name))
        {
            Name = IsVolcano ? "Volcano" : "Mountain Peak";
        }
        Type = IsVolcano ? "Volcano" : "Mountain Peak";
    }

    public override string ToString() { return Name; }

    public override string ToLink(bool link = true, DwarfObject? pov = null, WorldEvent? worldEvent = null)
    {
        if (link)
        {
            var sb = new StringBuilder();
            sb.Append(IsVolcano ? "Volcano" : "Mountain Peak");
            sb.Append("&#13");
            sb.Append("Events: ");
            sb.Append(Events.Count);
            string title = sb.ToString();

            return pov != this
                ? HtmlStyleUtil.GetAnchorString(Icon, "mountainpeak", Id, title, Name)
                : HtmlStyleUtil.GetAnchorCurrentString(Icon, title, HtmlStyleUtil.CurrentDwarfObject(Name));
        }
        return Name;
    }

    public override string GetIcon()
    {
        return Icon;
    }
}

