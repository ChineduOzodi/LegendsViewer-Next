using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class KnowledgeDiscovered : WorldEvent
{
    public List<string> Knowledge { get; set; } = [];
    public bool First { get; set; }
    public HistoricalFigure? HistoricalFigure { get; set; }

    public KnowledgeDiscovered(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "hfid": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "knowledge": Knowledge.AddRange(property.Value.Split(':')); break;
                case "first": First = true; property.Known = true; break;
            }
        }

        HistoricalFigure.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(HistoricalFigure?.ToLink(link, pov, this));
        if (First)
        {
            sb.Append(" was the first to discover ");
        }
        else
        {
            sb.Append(" independently discovered ");
        }
        if (Knowledge.Count > 1)
        {
            sb.Append(" the ");
            sb.Append(Knowledge[1]);
            if (Knowledge.Count > 2)
            {
                sb.Append(" (");
                sb.Append(Knowledge[2]);
                sb.Append(")");
            }
            sb.Append(" in the field of ");
            sb.Append(Knowledge[0]);
            sb.Append(".");
        }
        return sb.ToString();
    }
}
