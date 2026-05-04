using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.Events;

public class ChangeHfBodyState : WorldEvent
{
    public HistoricalFigure? HistoricalFigure { get; set; }
    public BodyState BodyState { get; set; }
    public Site? Site { get; set; }
    public int StructureId { get; set; }
    public Structure? Structure { get; set; }
    public WorldRegion? Region { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }
    public Location? Coordinates { get; set; }
    private readonly string? _unknownBodyState;

    public ChangeHfBodyState(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "hfid": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "body_state":
                    switch (property.Value)
                    {
                        case "entombed at site": BodyState = BodyState.EntombedAtSite; break;
                        default:
                            BodyState = BodyState.Unknown;
                            _unknownBodyState = property.Value;
                            world.ParsingErrors.Report("Unknown HF Body State: " + _unknownBodyState);
                            break;
                    }
                    break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "structure_id":
                case "building_id":
                    StructureId = Convert.ToInt32(property.Value);
                    break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
                case "coords": Coordinates = Formatting.ConvertToLocation(property.Value, world); break;
            }
        }
        if (Site != null)
        {
            Structure = Site.Structures.Find(structure => structure.LocalId == StructureId);
        }
        Structure?.AddEvent(this);
        HistoricalFigure?.AddEvent(this);
        Site?.AddEvent(this);
        Region?.AddEvent(this);
        UndergroundRegion?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(HistoricalFigure?.ToLink(link, pov, this));
        sb.Append(' ');
        string stateString = "";
        switch (BodyState)
        {
            case BodyState.EntombedAtSite: stateString = "was entombed"; break;
            case BodyState.Unknown: stateString = "(" + _unknownBodyState + ")"; break;
        }
        sb.Append(stateString);
        if (Region != null)
        {
            sb.Append(" in ");
            sb.Append(Region.ToLink(link, pov, this));
        }

        if (Site != null)
        {
            sb.Append(" at ");
            sb.Append(Site.ToLink(link, pov, this));
        }

        sb.Append(" within ");
        sb.Append(Structure != null ? Structure.ToLink(link, pov, this) : "UNKNOWN STRUCTURE");
        sb.Append(PrintParentCollection(link, pov));
        sb.Append('.');
        return sb.ToString();
    }
}

