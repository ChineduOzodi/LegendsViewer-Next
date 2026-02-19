using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class KnowledgeDiscovered : WorldEvent
{
    public List<string> Knowledge { get; set; } = []; // TODO
    public bool First { get; set; }
    public HistoricalFigure? HistoricalFigure { get; set; }

    public KnowledgeDiscovered(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "hfid":
                    HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value));
                    break;
                case "knowledge":
                    Knowledge.AddRange(property.Value.Split(':'));
                    break;
                case "first":
                    First = true;
                    property.Known = true;
                    break;
            }
        }

        HistoricalFigure.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        string eventString = GetYearTime();
        eventString += HistoricalFigure?.ToLink(link, pov, this);
        if (First)
        {
            eventString += " was the first to discover ";
        }
        else
        {
            eventString += " independently discovered ";
        }
        if (Knowledge.Count > 1)
        {
            eventString += " the " + Knowledge[1];
            if (Knowledge.Count > 2)
            {
                eventString += " (" + Knowledge[2] + ")";
            }
            eventString += " in the field of " + Knowledge[0] + ".";
        }
        return eventString;
    }
}

