using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.WorldObjects;

public class Landmass : WorldObject, IHasCoordinates
{
    public List<Location> Coordinates { get; set; } // legends_plus.xml

    public Landmass(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        Name = "Untitled";
        Coordinates = [];
        string[] coordinateStrings;
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "name": Name = Formatting.InitCaps(property.Value); break;
                case "coord_1":
                    coordinateStrings = property.Value.Split(new[] { '|' },
                        StringSplitOptions.RemoveEmptyEntries);
                    foreach (var coordinateString in coordinateStrings)
                    {
                        string[] xYCoordinates = coordinateString.Split(',');
                        int x = Convert.ToInt32(xYCoordinates[0]);
                        int y = Convert.ToInt32(xYCoordinates[1]);
                        Coordinates.Add(new Location(x, y));
                    }
                    break;
                case "coord_2":
                    coordinateStrings = property.Value.Split(new[] { '|' },
                        StringSplitOptions.RemoveEmptyEntries);
                    foreach (var coordinateString in coordinateStrings)
                    {
                        string[] xYCoordinates = coordinateString.Split(',');
                        int x = Convert.ToInt32(xYCoordinates[0]);
                        int y = Convert.ToInt32(xYCoordinates[1]);
                        Coordinates.Add(new Location(x, y));
                    }
                    break;
            }
        }
        Type = GetTypeByName(Name);
        Icon = HtmlStyleUtil.GetIconString("island-variant");
    }

    private string GetTypeByName(string name)
    {
        if (name.Contains("Continent", StringComparison.InvariantCultureIgnoreCase))
        {
            return "Continent";
        }
        if (name.Contains("Island", StringComparison.InvariantCultureIgnoreCase))
        {
            return "Island";
        }
        if (name.Contains("Land", StringComparison.InvariantCultureIgnoreCase))
        {
            return "Land";
        }
        return "Unknown";
    }

    public override string ToString() { return Name; }

    public override string ToLink(bool link = true, DwarfObject? pov = null, WorldEvent? worldEvent = null)
    {
        if (link)
        {
            string title = "";
            title += "Landmass";
            title += "&#13";
            title += "Events: " + Events.Count;

            return pov != this
                ? HtmlStyleUtil.GetAnchorString(Icon, "landmass", Id, title, Name)
                : HtmlStyleUtil.GetAnchorCurrentString(Icon, title, HtmlStyleUtil.CurrentDwarfObject(Name));
        }
        return Name;
    }

    public override string GetIcon()
    {
        return Icon;
    }
}

