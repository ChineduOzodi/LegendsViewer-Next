using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using System.Text.Json.Serialization;

namespace LegendsViewer.Backend.Legends.Various;

public class HonorEntity
{
    [JsonIgnore]
    public int EntityId { get; }
    [JsonIgnore]
    public List<int> HonorIds { get; set; }

    [JsonIgnore]
    public Entity? Entity { get; set; }
    public string? EntityToLink => Entity?.ToLink(true);

    public List<Honor> Honors { get; set; }
    public int Battles { get; set; }
    public int Kills { get; set; }

    public HonorEntity(List<Property> properties, IWorld world)
    {
        HonorIds = [];
        Honors = [];
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "entity": EntityId = Convert.ToInt32(property.Value); break;
                case "battles": Battles = Convert.ToInt32(property.Value); break;
                case "kills": Kills = Convert.ToInt32(property.Value); break;
                case "honor_id": HonorIds.Add(Convert.ToInt32(property.Value)); break;
            }
        }
    }

    public void Resolve(IWorld world, HistoricalFigure historicalFigure)
    {
        Entity = world.GetEntity(EntityId);
        if (Entity != null)
        {
            foreach (var honorId in HonorIds)
            {
                var honor = Entity.Honors.Find(h => h.Id == honorId);
                if (honor != null)
                {
                    honor.HonoredHfs.Add(historicalFigure);
                    Honors.Add(honor);
                }
            }
        }
    }
}



