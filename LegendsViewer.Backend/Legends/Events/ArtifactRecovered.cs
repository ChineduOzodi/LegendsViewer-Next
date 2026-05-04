using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class ArtifactRecovered : WorldEvent
{
    public Artifact? Artifact { get; set; }
    public HistoricalFigure? HistoricalFigure { get; set; }
    public int UnitId { get; set; }
    public Site? Site { get; set; }
    public int StructureId { get; set; }
    public Structure? Structure { get; set; }
    public WorldRegion? Region { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }

    public ArtifactRecovered(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "artifact_id":
                    Artifact = world.GetArtifact(Convert.ToInt32(property.Value));
                    break;
                case "hist_figure_id":
                    HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value));
                    break;
                case "site_id":
                    Site = world.GetSite(Convert.ToInt32(property.Value));
                    break;
                case "structure_id":
                    StructureId = Convert.ToInt32(property.Value);
                    break;
                case "unit_id":
                    UnitId = Convert.ToInt32(property.Value);
                    if (UnitId != -1)
                    {
                        property.Known = false;
                    }
                    break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
            }
        }

        if (Site != null)
        {
            Structure = Site.Structures.Find(structure => structure.LocalId == StructureId);
        }
        Artifact?.AddEvent(this);
        HistoricalFigure?.AddEvent(this);
        Site?.AddEvent(this);
        Structure?.AddEvent(this);
        Region?.AddEvent(this);
        UndergroundRegion?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Artifact?.ToLink(link, pov, this));
        sb.Append(" was recovered by ");
        sb.Append(HistoricalFigure?.ToLink(link, pov, this));
        if (Structure != null)
        {
            sb.Append(" from ");
            sb.Append(Structure.ToLink(link, pov, this));
        }
        if (Site != null)
        {
            sb.Append(" in ");
            sb.Append(Site.ToLink(link, pov, this));
        }
        else if (Region != null)
        {
            sb.Append(" in ");
            sb.Append(Region.ToLink(link, pov, this));
        }
        else if (UndergroundRegion != null)
        {
            sb.Append(" in ");
            sb.Append(UndergroundRegion.ToLink(link, pov, this));
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append('.');
        return sb.ToString();
    }
}


