using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.EventCollections;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.Events;

public class WorldEvent : DwarfObject, IComparable<WorldEvent>
{
    private int _seconds72;

    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }

    public string Date
    {
        get
        {
            return Year < 0 ? "-" : $"{Year:0000}-{Month:00}-{Day:00}";
        }
    }

    public int Seconds72
    {
        get => _seconds72;
        set
        {
            _seconds72 = value;
            Month = 1 + _seconds72 / (28 * 1200);
            Day = 1 + _seconds72 % (28 * 1200) / 1200;
        }
    }

    public string Type { get; set; } = string.Empty;
    public IWorld? World { get; set; }
    public EventCollection? ParentCollection { get; set; }

    public WorldEvent(List<Property> properties, IWorld? world)
    {
        World = world;
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "id": Id = Convert.ToInt32(property.Value); property.Known = true; break;
                case "year": Year = Convert.ToInt32(property.Value); property.Known = true; break;
                case "seconds72": Seconds72 = Convert.ToInt32(property.Value); property.Known = true; break;
                case "type": Type = string.Intern(property.Value); property.Known = true; break;
            }
        }
    }

    public virtual string Print(bool link = true, DwarfObject? pov = null)
    {
        string eventString = GetYearTime() + Type;
        eventString += PrintParentCollection(link, pov);
        eventString += ".";
        return eventString;
    }

    public virtual string GetYearTime()
    {
        return Formatting.YearPlusSeconds72ToProsa(Year, Seconds72);
    }

    public string PrintParentCollection(bool link = true, DwarfObject? pov = null)
    {
        if (ParentCollection == null)
        {
            return "";
        }
        EventCollection? parent = ParentCollection;
        string collectionString = "";
        while (parent != null)
        {
            if (collectionString.Length > 0)
            {
                collectionString += " as part of ";
            }
            collectionString += parent.ToLink(link, pov, this);
            parent = parent.ParentCollection;
        }
        return " during " + collectionString;
    }

    public int Compare(WorldEvent worldEvent)
    {
        return Id.CompareTo(worldEvent.Id);
    }

    public int CompareTo(object obj)
    {
        return Id.CompareTo(obj);
    }

    public int CompareTo(WorldEvent? other)
    {
        return Id.CompareTo(other?.Id);
    }
}
