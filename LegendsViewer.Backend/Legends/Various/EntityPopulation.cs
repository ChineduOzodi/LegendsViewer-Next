using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using System.Text.Json.Serialization;

namespace LegendsViewer.Backend.Legends.Various;

public class EntityPopulation : WorldObject
{
    public CreatureInfo Race { get; set; } = CreatureInfo.Unknown;
    public int Count { get; set; } // legends_plus.xml
    public int EntityId { get; set; } = -1;

    [JsonIgnore]
    public Entity? Entity { get; set; }
    public string? EntityLink => Entity?.ToLink(true);

    [JsonIgnore]
    public List<HistoricalFigure> Members { get; set; } = [];

    public EntityPopulation(List<Property> properties, IWorld world)
        :base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "race":
                    var raceCount = property.Value.Split(':');
                    Race = world.GetCreatureInfo(raceCount[0]);
                    Count = Math.Abs(Convert.ToInt32(raceCount[1]));
                    break;
                case "civ_id":
                    EntityId = property.ValueAsInt();
                    break;
            }
        }
    }
}


