using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class InsurrectionStarted : WorldEvent
{
    public Entity? Civ { get; set; }
    public Site? Site { get; set; }
    public InsurrectionOutcome Outcome { get; set; }
    public bool ActualStart { get; set; }
    private readonly string? _unknownOutcome;

    public InsurrectionStarted(List<Property> properties, IWorld world) : base(properties, world)
    {
        ActualStart = false;

        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "target_civ_id": Civ = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "outcome":
                    switch (property.Value)
                    {
                        case "leadership overthrown": Outcome = InsurrectionOutcome.LeadershipOverthrown; break;
                        case "population gone": Outcome = InsurrectionOutcome.PopulationGone; break;
                        default:
                            Outcome = InsurrectionOutcome.Unknown;
                            _unknownOutcome = property.Value;
                            world.ParsingErrors.Report("Unknown Insurrection Outcome: " + _unknownOutcome);
                            break;
                    }
                    break;
            }
        }

        Civ.AddEvent(this);
        Site.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        if (ActualStart)
        {
            sb.Append("an insurrection against ");
            sb.Append(Civ?.ToLink(link, pov, this));
            sb.Append(" began in ");
            sb.Append(Site?.ToLink(link, pov, this));
        }
        else
        {
            sb.Append("the insurrection in ");
            sb.Append(Site?.ToLink(link, pov, this));
            switch (Outcome)
            {
                case InsurrectionOutcome.LeadershipOverthrown:
                    sb.Append(" concluded with ");
                    sb.Append(Civ?.ToLink(link, pov, this));
                    sb.Append(" overthrown");
                    break;
                case InsurrectionOutcome.PopulationGone:
                    sb.Append(" ended with the disappearance of the rebelling population");
                    break;
                default:
                    sb.Append(" against ");
                    sb.Append(Civ?.ToLink(link, pov, this));
                    sb.Append(" concluded with (");
                    sb.Append(_unknownOutcome);
                    sb.Append(")");
                    break;
            }
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}
