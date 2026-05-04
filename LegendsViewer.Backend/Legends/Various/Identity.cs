using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;
using System.Text.Json.Serialization;

namespace LegendsViewer.Backend.Legends.Various;

public class Identity
{
    public int Id { get; set; }
    public string? Name { get; set; }

    [JsonIgnore]
    public HistoricalFigure? HistoricalFigure { get; set; }
    public string? HistoricalFigureToLink => HistoricalFigure?.ToLink(true);

    public int BirthYear { get; set; }
    public int BirthSeconds72 { get; set; }

    [JsonIgnore]
    public Entity? Entity { get; set; }
    public string? EntityToLink => Entity?.ToLink(true);

    public CreatureInfo Race { get; set; } = CreatureInfo.Unknown;
    public string? Caste { get; set; }
    public string? Profession { get; set; }

    public Identity(string name, CreatureInfo race, string? caste)
    {
        Name = name;
        Race = race;
        Caste = caste;
    }

    public Identity(List<Property> properties, IWorld world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "id": Id = Convert.ToInt32(property.Value); break;
                case "name": Name = Formatting.InitCaps(property.Value.Replace("'", "`")); break;
                case "nemesis_id":
                case "histfig_id":
                    HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value));
                    break;
                case "birth_year": BirthYear = Convert.ToInt32(property.Value); break;
                case "birth_second": BirthSeconds72 = Convert.ToInt32(property.Value); break;
                case "entity_id": Entity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "race": Race = world.GetCreatureInfo(property.Value); break;
                case "caste": Caste = Formatting.InitCaps(property.Value); break;
                case "profession": Profession = Formatting.InitCaps(property.Value); break;
            }
        }

        HistoricalFigure?.Identities.Add(this);
    }

    public string Print(bool link = true, DwarfObject? pov = null, WorldEvent? worldEvent = null)
    {
        var identityString = "the ";
        if (Race != null && Race != CreatureInfo.Unknown)
        {
            identityString += Race.NameSingular.ToLower();
        }
        else if (HistoricalFigure != null)
        {
            identityString += HistoricalFigure.GetRaceString();
        }

        if (!string.IsNullOrWhiteSpace(Profession))
        {
            identityString += " " + Profession.ToLower();
        }
        var icon = !string.IsNullOrWhiteSpace(Caste) ? GetIcon() : HistoricalFigure?.GetIcon();
        if (!string.IsNullOrWhiteSpace(icon))
        {
            identityString += " " + icon;
        }
        identityString += " " + Name;
        if (Entity != null)
        {
            identityString += " of ";
            identityString += Entity.ToLink(link, pov, worldEvent);
        }
        return identityString;
    }

    public string GetIcon()
    {
        if (Caste == "Female")
        {
            return HistoricalFigure.FemaleIcon;
        }
        if (Caste == "Male")
        {
            return HistoricalFigure.MaleIcon;
        }
        return Caste == "Default" ? HistoricalFigure.NeuterIcon : "";
    }
}


