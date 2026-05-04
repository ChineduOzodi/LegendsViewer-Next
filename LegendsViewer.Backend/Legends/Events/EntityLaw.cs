using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class EntityLaw : WorldEvent
{
    public Entity? Entity { get; set; }
    public HistoricalFigure? HistoricalFigure { get; set; }
    public EntityLawType Law { get; set; }
    public bool LawLaid { get; set; }
    private readonly string? _unknownLawType;

    public EntityLaw(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "entity_id": Entity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "hist_figure_id": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "law_add":
                case "law_remove":
                    switch (property.Value)
                    {
                        case "harsh": Law = EntityLawType.Harsh; break;
                        default:
                            Law = EntityLawType.Unknown;
                            _unknownLawType = property.Value;
                            world.ParsingErrors.Report("Unknown Law Type: " + _unknownLawType);
                            break;
                    }
                    LawLaid = property.Name == "law_add";
                    break;
            }
        }

        Entity?.AddEvent(this);
        HistoricalFigure?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(HistoricalFigure?.ToLink(link, pov, this));
        if (LawLaid)
        {
            sb.Append(" laid a series of ");
        }
        else
        {
            sb.Append(" lifted numerous ");
        }

        switch (Law)
        {
            case EntityLawType.Harsh: sb.Append("oppressive"); break;
            case EntityLawType.Unknown: sb.Append("(" + _unknownLawType + ")"); break;
        }
        if (LawLaid)
        {
            sb.Append(" edicts upon ");
        }
        else
        {
            sb.Append(" laws from ");
        }

        sb.Append(Entity?.ToLink(link, pov, this));
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}

