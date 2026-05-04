using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class ArtifactTransformed : WorldEvent
{
    public int UnitId { get; set; }
    public Artifact? NewArtifact { get; set; }
    public Artifact? OldArtifact { get; set; }
    public HistoricalFigure? HistoricalFigure { get; set; }
    public Site? Site { get; set; }

    public ArtifactTransformed(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "unit_id": UnitId = Convert.ToInt32(property.Value); break;
                case "new_artifact_id": NewArtifact = world.GetArtifact(Convert.ToInt32(property.Value)); break;
                case "old_artifact_id": OldArtifact = world.GetArtifact(Convert.ToInt32(property.Value)); break;
                case "hist_figure_id": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
            }
        }

        NewArtifact?.AddEvent(this);
        OldArtifact?.AddEvent(this);
        HistoricalFigure?.AddEvent(this);
        Site?.AddEvent(this);
    }
    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(NewArtifact?.ToLink(link, pov, this));
        sb.Append(", ");
        if (!string.IsNullOrWhiteSpace(NewArtifact?.Material))
        {
            sb.Append(NewArtifact.Material);
        }
        if (!string.IsNullOrWhiteSpace(NewArtifact?.Subtype))
        {
            sb.Append(" ");
            sb.Append(NewArtifact.Subtype);
        }
        else
        {
            sb.Append(" ");
            sb.Append(!string.IsNullOrWhiteSpace(NewArtifact?.Type) ? NewArtifact.Type.ToLower() : "UNKNOWN TYPE");
        }
        sb.Append(" was made from ");
        sb.Append(OldArtifact?.ToLink(link, pov, this));
        sb.Append(", ");
        if (!string.IsNullOrWhiteSpace(OldArtifact?.Material))
        {
            sb.Append(OldArtifact.Material);
        }
        if (!string.IsNullOrWhiteSpace(OldArtifact?.Subtype))
        {
            sb.Append(" ");
            sb.Append(OldArtifact.Subtype);
        }
        else
        {
            sb.Append(" ");
            sb.Append(!string.IsNullOrWhiteSpace(OldArtifact?.Type) ? OldArtifact.Type.ToLower() : "UNKNOWN TYPE");
        }
        if (Site != null)
        {
            sb.Append(" in ");
            sb.Append(Site?.ToLink(link, pov, this));
        }

        sb.Append(" by ");
        sb.Append(HistoricalFigure != null ? HistoricalFigure?.ToLink(link, pov, this) : "UNKNOWN HISTORICAL FIGURE");
        sb.Append(PrintParentCollection(link, pov));
        sb.Append('.');
        return sb.ToString();
    }
}
