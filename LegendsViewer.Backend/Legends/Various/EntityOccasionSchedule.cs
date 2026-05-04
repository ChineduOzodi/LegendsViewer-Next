using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Parser;

namespace LegendsViewer.Backend.Legends.Various;

public class EntityOccasionSchedule
{
    public int Id { get; set; } = -1;
    public string Name { get; set; }
    public List<Feature> Features { get; set; } = [];
    public ScheduleType ScheduleType { get; set; } // legends_plus.xml
    public int Reference { get; set; } = -1;
    public int Reference2 { get; set; } = -1;
    public string? ItemType { get; set; }
    public string? ItemSubType { get; set; }

    public EntityOccasionSchedule(List<Property> properties, IWorld world)
    {
        Name = "Unknown Schedule";
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "id": Id = Convert.ToInt32(property.Value); break;
                case "type":
                    switch (property.Value)
                    {
                        case "procession": ScheduleType = ScheduleType.Procession; break;
                        case "ceremony": ScheduleType = ScheduleType.Ceremony; break;
                        case "foot_race": ScheduleType = ScheduleType.FootRace; break;
                        case "throwing_competition": ScheduleType = ScheduleType.ThrowingCompetition; break;
                        case "dance_performance": ScheduleType = ScheduleType.DancePerformance; break;
                        case "storytelling": ScheduleType = ScheduleType.Storytelling; break;
                        case "poetry_recital": ScheduleType = ScheduleType.PoetryRecital; break;
                        case "musical_performance": ScheduleType = ScheduleType.MusicalPerformance; break;
                        case "wrestling_competition": ScheduleType = ScheduleType.WrestlingCompetition; break;
                        case "gladiatory_competition": ScheduleType = ScheduleType.GladiatoryCompetition; break;
                        case "poetry_competition": ScheduleType = ScheduleType.PoetryCompetition; break;
                        case "dance_competition": ScheduleType = ScheduleType.DanceCompetition; break;
                        case "musical_competition": ScheduleType = ScheduleType.MusicalCompetition; break;
                        default:
                            property.Known = false;
                            break;
                    }
                    break;
                case "feature":
                    property.Known = true;
                    if (property.SubProperties != null)
                    {
                        Features.Add(new Feature(property.SubProperties, world));
                    }

                    break;
                case "reference": Reference = Convert.ToInt32(property.Value); break;
                case "reference2": Reference2 = Convert.ToInt32(property.Value); break;
                case "item_type": ItemType = string.Intern(property.Value); break;
                case "item_subtype": ItemSubType = string.Intern(property.Value); break;
            }
        }
    }
}


