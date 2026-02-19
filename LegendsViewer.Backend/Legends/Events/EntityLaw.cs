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
        string eventString = GetYearTime() + HistoricalFigure?.ToLink(link, pov, this);
        if (LawLaid)
        {
            eventString += " laid a series of ";
        }
        else
        {
            eventString += " lifted numerous ";
        }

        switch (Law)
        {
            case EntityLawType.Harsh: eventString += "oppressive"; break;
            case EntityLawType.Unknown: eventString += "(" + _unknownLawType + ")"; break;
        }
        if (LawLaid)
        {
            eventString += " edicts upon ";
        }
        else
        {
            eventString += " laws from ";
        }

        eventString += Entity?.ToLink(link, pov, this);
        eventString += PrintParentCollection(link, pov);
        eventString += ".";
        return eventString;
    }
}

