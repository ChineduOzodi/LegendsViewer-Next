using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.EventCollections;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.Events;

public class HfTravel : WorldEvent
{
    public Location? Coordinates { get; set; }
    public bool Escaped { get; set; }
    public bool Returned { get; set; }
    public HistoricalFigure? HistoricalFigure { get; set; }
    public Site? Site { get; set; }
    public WorldRegion? Region { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }
    public HfTravel(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "coords": Coordinates = Formatting.ConvertToLocation(property.Value, world); break;
                case "escape": Escaped = true; property.Known = true; break;
                case "return": Returned = true; property.Known = true; break;
                case "group_hfid": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
            }
        }

        HistoricalFigure?.AddEvent(this);
        Site?.AddEvent(this);
        Region?.AddEvent(this);
        UndergroundRegion?.AddEvent(this);
    }
    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        string eventString = GetYearTime() + HistoricalFigure?.ToLink(link, pov, this);
        if (Escaped)
        {
            return GetYearTime() + HistoricalFigure?.ToLink(link, pov, this) + " escaped from the " + UndergroundRegion?.ToLink(link, pov, this);
        }

        if (Returned)
        {
            eventString += " returned to ";
        }
        else
        {
            eventString += " made a journey to ";
        }

        if (UndergroundRegion != null)
        {
            eventString += UndergroundRegion?.ToLink(link, pov, this);
        }
        else if (Site != null)
        {
            eventString += Site.ToLink(link, pov, this);
        }
        else if (Region != null)
        {
            eventString += Region.ToLink(link, pov, this);
        }

        if (!(ParentCollection is Journey))
        {
            eventString += PrintParentCollection(link, pov);
        }
        eventString += ".";
        return eventString;
    }
}

