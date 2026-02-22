using System.Text;
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
            var sb = new StringBuilder();
            string title = GetTitle();
            sb.Append("the ");
            sb.Append(pov != this
                ? HtmlStyleUtil.GetAnchorString(Icon, "journey", Id, title, Name)
                : HtmlStyleUtil.GetAnchorCurrentString(Icon, title, HtmlStyleUtil.CurrentDwarfObject(Name)));
            if (HistoricalFigure != null && pov != HistoricalFigure)
            {
                sb.Append(" of ");
                sb.Append(HistoricalFigure.ToLink(true, this));
            }

            if (Site != null && pov != Site)
            {
                sb.Append(" to ");
                sb.Append(Site.ToLink(true, this));
            }
            else if (Region != null && pov != Region)
            {
                sb.Append(" to ");
                sb.Append(Region.ToLink(true, this));
            }
            else if (UndergroundRegion != null && pov != UndergroundRegion)
            {
                sb.Append(" to ");
                sb.Append(UndergroundRegion.ToLink(true, this));
            }
            return sb.ToString();
        }
        return ToString();
    }

    private string GetTitle()
    {
        var sb = new StringBuilder();
        sb.Append(Type);
        if (Site != null)
        {
            sb.Append("&#13");
            sb.Append("Site: ");
            sb.Append(Site.ToLink(false));
        }
        else if (Region != null)
        {
            sb.Append("&#13");
            sb.Append("Region: ");
            sb.Append(Region.ToLink(false));
        }
        else if (UndergroundRegion != null)
        {
            sb.Append("&#13");
            sb.Append("Underground Region: ");
            sb.Append(UndergroundRegion.ToLink(false));
        }
        return sb.ToString();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("the ");
        sb.Append(Name);

        if (HistoricalFigure != null)
        {
            sb.Append(" of ");
            sb.Append(HistoricalFigure.ToLink(true, this));
        }

        if (Site != null)
        {
            sb.Append(" to ");
            sb.Append(Site.ToLink(true, this));
        }
        else if (Region != null)
        {
            sb.Append(" to ");
            sb.Append(Region.ToLink(true, this));
        }
        else if (UndergroundRegion != null)
        {
            sb.Append(" to ");
            sb.Append(UndergroundRegion.ToLink(true, this));
        }
        return sb.ToString();
    }
}


