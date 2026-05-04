using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.Various;

public class CreatureInfo
{
    public static readonly CreatureInfo Unknown = new("UNKNOWN");
    public string Id { get; set; } = string.Empty;

    public string NameSingular { get; set; } = string.Empty;
    public string NamePlural { get; set; } = string.Empty;

    public CreatureInfo(List<Property> properties, IWorld world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "creature_id": Id = string.Intern(property.Value); break;
                case "name_singular": NameSingular = string.Intern(Formatting.FormatRace(property.Value)); break;
                case "name_plural": NamePlural = string.Intern(Formatting.FormatRace(property.Value)); break;
                // TODO read the other properties and use them
                default:
                    property.Known = true;
                    break;
            }
        }
    }

    public CreatureInfo(string identifier)
    {
        Id = identifier.ToLower();
        NameSingular = string.Intern(Formatting.FormatRace(identifier));
        NamePlural = string.Intern(Formatting.MakePopulationPlural(NameSingular));
    }

    public override string ToString()
    {
        return NameSingular ?? NamePlural ?? string.Empty;
    }
}


