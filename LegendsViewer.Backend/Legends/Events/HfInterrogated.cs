using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class HfInterrogated : WorldEvent
{
    public HistoricalFigure? TargetHf { get; set; }
    public bool WantedAndRecognized { get; set; }
    public bool HeldFirmInInterrogation { get; set; }
    public HistoricalFigure? InterrogatorHf { get; set; }
    public Entity? ArrestingEntity { get; set; }

    public HfInterrogated(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "target_hfid": TargetHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "interrogator_hfid": InterrogatorHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "arresting_enid": ArrestingEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "held_firm_in_interrogation": property.Known = true; HeldFirmInInterrogation = true; break;
                case "wanted_and_recognized": property.Known = true; WantedAndRecognized = true; break;
            }
        }

        TargetHf.AddEvent(this);
        if (InterrogatorHf != TargetHf)
        {
            InterrogatorHf.AddEvent(this);
        }
        ArrestingEntity.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        if (WantedAndRecognized && HeldFirmInInterrogation)
        {
            sb.Append(TargetHf?.ToLink(link, pov, this));
            sb.Append(" was recognized and arrested by ");
            sb.Append(ArrestingEntity?.ToLink(link, pov, this));
            sb.Append(". Despite the interrogation by ");
            sb.Append(InterrogatorHf?.ToLink(link, pov, this));
            sb.Append(", ");
            sb.Append(TargetHf?.ToLink(link, pov, this));
            sb.Append(" refused to reveal anything and was released");
        }
        else
        {
            sb.Append(TargetHf?.ToLink(link, pov, this));
            sb.Append(" was interrogated");
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}
