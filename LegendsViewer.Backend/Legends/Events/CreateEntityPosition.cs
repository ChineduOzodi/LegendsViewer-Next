using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class CreateEntityPosition : WorldEvent
{
    public HistoricalFigure? HistoricalFigure { get; set; }
    public Entity? Civ { get; set; }
    public Entity? SiteCiv { get; set; }
    public string? Position { get; set; }
    public ReasonForCreatingEntity Reason { get; set; }

    public CreateEntityPosition(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "histfig": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "civ": Civ = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_civ": SiteCiv = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "position": Position = property.Value; break;
                case "reason":
                    switch (property.Value)
                    {
                        case "0":
                        case "force_of_argument":
                            Reason = ReasonForCreatingEntity.ForceOfArgument;
                            break;
                        case "1":
                        case "threat_of_violence":
                            Reason = ReasonForCreatingEntity.ThreatOfViolence;
                            break;
                        case "2":
                        case "collaboration":
                            Reason = ReasonForCreatingEntity.Collaboration;
                            break;
                        case "3":
                        case "wave_of_popular_support":
                            Reason = ReasonForCreatingEntity.WaveOfPopularSupport;
                            break;
                        case "4":
                        case "as_a_matter_of_course":
                            Reason = ReasonForCreatingEntity.AsAMatterOfCourse;
                            break;
                    }
                    break;
            }
        }
        HistoricalFigure?.AddEvent(this);
        Civ?.AddEvent(this);
        if (SiteCiv != Civ)
        {
            SiteCiv?.AddEvent(this);
        }
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        switch (Reason)
        {
            case ReasonForCreatingEntity.ForceOfArgument:
                sb.Append(HistoricalFigure != null ? HistoricalFigure.ToLink(link, pov, this) : "UNKNOWN HISTORICAL FIGURE");
                sb.Append(" of ");
                sb.Append(Civ != null ? Civ.ToLink(link, pov, this) : "UNKNOWN CIV");
                sb.Append(" created the position of ");
                sb.Append(!string.IsNullOrWhiteSpace(Position) ? Position : "UNKNOWN POSITION");
                sb.Append(" through force of argument");
                break;
            case ReasonForCreatingEntity.ThreatOfViolence:
                sb.Append(HistoricalFigure != null ? HistoricalFigure.ToLink(link, pov, this) : "UNKNOWN HISTORICAL FIGURE");
                sb.Append(" of ");
                sb.Append(Civ != null ? Civ.ToLink(link, pov, this) : "UNKNOWN CIV");
                sb.Append(" compelled the creation of the position of ");
                sb.Append(!string.IsNullOrWhiteSpace(Position) ? Position : "UNKNOWN POSITION");
                sb.Append(" with threats of violence");
                break;
            case ReasonForCreatingEntity.Collaboration:
                sb.Append(SiteCiv != null ? SiteCiv.ToLink(link, pov, this) : "UNKNOWN ENTITY");
                sb.Append(" collaborated to create the position of ");
                sb.Append(!string.IsNullOrWhiteSpace(Position) ? Position : "UNKNOWN POSITION");
                break;
            case ReasonForCreatingEntity.WaveOfPopularSupport:
                sb.Append(HistoricalFigure != null ? HistoricalFigure.ToLink(link, pov, this) : "UNKNOWN HISTORICAL FIGURE");
                sb.Append(" of ");
                sb.Append(Civ != null ? Civ.ToLink(link, pov, this) : "UNKNOWN CIV");
                sb.Append(" created the position of ");
                sb.Append(!string.IsNullOrWhiteSpace(Position) ? Position : "UNKNOWN POSITION");
                sb.Append(", pushed by a wave of popular support");
                break;
            case ReasonForCreatingEntity.AsAMatterOfCourse:
                sb.Append(HistoricalFigure != null ? HistoricalFigure.ToLink(link, pov, this) : "UNKNOWN HISTORICAL FIGURE");
                sb.Append(" of ");
                sb.Append(Civ != null ? Civ.ToLink(link, pov, this) : "UNKNOWN CIV");
                sb.Append(" created the position of ");
                sb.Append(!string.IsNullOrWhiteSpace(Position) ? Position : "UNKNOWN POSITION");
                sb.Append(" as a matter of course");
                break;
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append('.');
        return sb.ToString();
    }
}

