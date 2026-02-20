using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.Events;

public class ChangedCreatureType : WorldEvent
{
    public HistoricalFigure? Changee { get; set; }
    public HistoricalFigure? Changer { get; set; }
    public string? OldRace { get; set; }
    public string? NewRace { get; set; }

    // TODO Handle caste changes
    public string? OldCaste { get; set; }
    public string? NewCaste { get; set; }

    public ChangedCreatureType(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "old_race": OldRace = Formatting.FormatRace(property.Value); break;
                case "old_caste": OldCaste = property.Value; break;
                case "new_race": NewRace = Formatting.FormatRace(property.Value); break;
                case "new_caste": NewCaste = property.Value; break;
                case "changee_hfid": Changee = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "changer_hfid": Changer = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "changee": if (Changee == null) { Changee = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
                case "changer": if (Changer == null) { Changer = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
            }
        }
        if (Changee != null && !string.IsNullOrEmpty(NewRace))
        {
            Changee.PreviousRace = OldRace ?? string.Empty;
            Changee.CreatureTypes.Add(new HistoricalFigure.CreatureType(NewRace, this));
            Changee.AddEvent(this);
        }

        Changer?.AddEvent(this);
    }
    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Changer?.ToLink(link, pov, this) ?? "An unknown creature");
        sb.Append(" changed ");
        sb.Append(Changee?.ToLink(link, pov, this) ?? "an unknown creature");
        sb.Append(" from ");
        sb.Append(Formatting.AddArticle(OldRace ?? "unknown race").ToLower());
        sb.Append(" into ");
        sb.Append(Formatting.AddArticle(NewRace ?? "unknown race").ToLower());
        sb.Append(PrintParentCollection(link, pov));
        sb.Append('.');
        return sb.ToString();
    }
}

