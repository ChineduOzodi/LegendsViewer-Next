using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;
using System.Text;

namespace LegendsViewer.Backend.Legends.Events;

public class ChangeHfJob : WorldEvent
{
    public HistoricalFigure? HistoricalFigure;
    public Site? Site;
    public WorldRegion? Region;
    public UndergroundRegion? UndergroundRegion;
    public string NewJob { get; set; } = "UNKNOWN JOB";
    public string OldJob { get; set; } = "UNKNOWN JOB";
    public ChangeHfJob(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "hfid": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
                case "new_job": NewJob = string.Intern(property.Value.Replace("_", " ")); break;
                case "old_job": OldJob = string.Intern(property.Value.Replace("_", " ")); break;
                case "site": if (Site == null) { Site = world.GetSite(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
            }
        }

        HistoricalFigure?.AddEvent(this);
        Site?.AddEvent(this);
        Region?.AddEvent(this);
        UndergroundRegion?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());

        string figure = HistoricalFigure?.ToLink(link, pov, this) ?? "UNKNOWN HISTORICAL FIGURE";
        sb.Append(figure);
        sb.Append(' ');

        if (OldJob != "standard" && NewJob != "standard")
        {
            sb.Append("gave up being ");
            sb.Append(Formatting.AddArticle(OldJob));
            sb.Append(" to become ");
            sb.Append(Formatting.AddArticle(NewJob));
        }
        else if (NewJob != "standard")
        {
            sb.Append("became ");
            sb.Append(Formatting.AddArticle(NewJob));
        }
        else if (OldJob != "standard")
        {
            sb.Append("stopped being ");
            sb.Append(Formatting.AddArticle(OldJob));
        }
        else
        {
            sb.Append("became a peasant");
        }

        if (Site != null)
        {
            sb.Append(" in ");
            sb.Append(Site.ToLink(link, pov, this));
        }

        sb.Append(PrintParentCollection(link, pov));
        sb.Append('.');
        return sb.ToString();
    }
}

