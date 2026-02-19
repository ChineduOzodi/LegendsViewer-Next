using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class ArtifactStored : WorldEvent
{
    public Artifact? Artifact { get; set; }
    public int UnitId { get; set; }
    public HistoricalFigure? HistoricalFigure { get; set; }
    public Site? Site { get; set; }

    public ArtifactStored(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "artifact_id": Artifact = world.GetArtifact(Convert.ToInt32(property.Value)); break;
                case "unit_id": UnitId = Convert.ToInt32(property.Value); break;
                case "hist_figure_id": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
            }
        }

        Artifact?.AddEvent(this);
        HistoricalFigure?.AddEvent(this);
        Site?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        string eventString = GetYearTime();
        eventString += Artifact?.ToLink(link, pov, this);
        eventString += " was stored";
        if (HistoricalFigure != null)
        {
            eventString += " by ";
            eventString += HistoricalFigure?.ToLink(link, pov, this);
        }
        eventString += " in ";
        eventString += Site?.ToLink(link, pov, this);
        eventString += PrintParentCollection(link, pov);
        eventString += ".";
        return eventString;
    }
}

