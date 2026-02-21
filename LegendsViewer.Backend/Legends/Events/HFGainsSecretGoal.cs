using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class HfGainsSecretGoal : WorldEvent
{
    public HistoricalFigure? HistoricalFigure { get; set; }
    public SecretGoal Goal { get; set; }
    private readonly string? _unknownGoal;

    public HfGainsSecretGoal(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "hfid": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "secret_goal":
                    switch (property.Value)
                    {
                        case "immortality": Goal = SecretGoal.Immortality; break;
                        default:
                            Goal = SecretGoal.Unknown;
                            _unknownGoal = property.Value;
                            world.ParsingErrors.Report("Unknown Secret Goal: " + _unknownGoal);
                            break;
                    }
                    break;
            }
        }

        HistoricalFigure.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(HistoricalFigure?.ToLink(link, pov, this));
        string goalString = "";
        switch (Goal)
        {
            case SecretGoal.Immortality: goalString = " became obsessed with " + HistoricalFigure?.CasteNoun(true) + " own mortality and sought to extend " + HistoricalFigure?.CasteNoun(true) + " life by any means"; break;
            case SecretGoal.Unknown: goalString = " gained secret goal (" + _unknownGoal + ")"; break;
        }
        sb.Append(goalString);
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}
