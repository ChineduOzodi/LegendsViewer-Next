using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.EventCollections;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class HfAbducted : WorldEvent
{
    public HistoricalFigure? Target { get; set; }
    public HistoricalFigure? Snatcher { get; set; }
    public Site? Site { get; set; }
    public WorldRegion? Region { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }
    public HfAbducted(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "target_hfid": Target = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "snatcher_hfid": Snatcher = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
            }
        }

        Target?.AddEvent(this);
        Snatcher?.AddEvent(this);
        Site?.AddEvent(this);
        Region?.AddEvent(this);
        UndergroundRegion?.AddEvent(this);
    }
    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        string eventString = GetYearTime();
        if (Snatcher != null)
        {
            eventString += Snatcher.ToLink(link, pov, this);
        }
        else
        {
            eventString += "UNKNOWN HISTORICAL FIGURE";
        }

        eventString += $" abducted {Target?.ToLink(link, pov, this)} from {Site?.ToLink(link, pov, this)}";
        if (ParentCollection is not Abduction)
        {
            eventString += PrintParentCollection(link, pov);
        }
        eventString += ".";
        return eventString;
    }
}

