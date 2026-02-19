using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.EventCollections;

public class Journey : EventCollection
{
    public int Ordinal { get; set; } = -1;
    public HistoricalFigure? HistoricalFigure { get; set; }

    public Journey(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "ordinal": Ordinal = Convert.ToInt32(property.Value); break;
            }
        }

        Name = $"{Formatting.AddOrdinal(Ordinal)} journey";
        var travelEvent = Events.OfType<HfTravel>().FirstOrDefault();
        if (travelEvent != null)
        {
            if (Site == null)
            {
                Site = travelEvent.Site;
            }
            if (Region == null)
            {
                Region = travelEvent.Region;
            }
            if (UndergroundRegion == null)
            {
                UndergroundRegion = travelEvent.UndergroundRegion;
            }
            if (HistoricalFigure == null)
            {
                HistoricalFigure = travelEvent.HistoricalFigure;
            }
        }
        Icon = HtmlStyleUtil.GetIconString("map-marker-path");
    }

    public override string ToLink(bool link = true, DwarfObject? pov = null, WorldEvent? worldEvent = null)
    {
        if (link)
        {
            string title = GetTitle();
            string linkedString = "the ";
            linkedString += pov != this
                ? HtmlStyleUtil.GetAnchorString(Icon, "journey", Id, title, Name)
                : HtmlStyleUtil.GetAnchorCurrentString(Icon, title, HtmlStyleUtil.CurrentDwarfObject(Name));
            if (HistoricalFigure != null && pov != HistoricalFigure)
            {
                linkedString += $" of {HistoricalFigure.ToLink(true, this)}";
            }

            if (Site != null && pov != Site)
            {
                linkedString += $" to {Site.ToLink(true, this)}";
            }
            else if (Region != null && pov != Region)
            {
                linkedString += $" to {Region.ToLink(true, this)}";
            }
            else if (UndergroundRegion != null && pov != UndergroundRegion)
            {
                linkedString += $" to {UndergroundRegion.ToLink(true, this)}";
            }
            return linkedString;
        }
        return ToString();
    }

    private string GetTitle()
    {
        string title = Type;
        if (Site != null)
        {
            title += "&#13";
            title += "Site: ";
            title += Site.ToLink(false);
        }
        else if (Region != null)
        {
            title += "&#13";
            title += "Region: ";
            title += Region.ToLink(false);
        }
        else if (UndergroundRegion != null)
        {
            title += "&#13";
            title += "Underground Region: ";
            title += UndergroundRegion.ToLink(false);
        }
        return title;
    }

    public override string ToString()
    {
        string text = "the ";
        text += Name;

        if (HistoricalFigure != null)
        {
            text += $" of {HistoricalFigure.ToLink(true, this)}";
        }

        if (Site != null)
        {
            text += $" to {Site.ToLink(true, this)}";
        }
        else if (Region != null)
        {
            text += $" to {Region.ToLink(true, this)}";
        }
        else if (UndergroundRegion != null)
        {
            text += $" to {UndergroundRegion.ToLink(true, this)}";
        }
        return text;
    }
}


