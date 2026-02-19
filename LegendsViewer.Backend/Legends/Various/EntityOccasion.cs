using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.Various;

public class EntityOccasion
{
    public int Id { get; set; } = -1;
    public string Name { get; set; }
    public int EventId { get; set; }
    public List<EntityOccasionSchedule> Schedules { get; set; } = [];
    public Entity? Entity { get; set; }

    public EntityOccasion(List<Property> properties, IWorld world, Entity entity)
    {
        Name = "UNKNOWN OCCASION";
        Schedules = [];

        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "id": Id = Convert.ToInt32(property.Value); break;
                case "name": Name = Formatting.InitCaps(property.Value); break;
                case "event": EventId = Convert.ToInt32(property.Value); break;
                case "schedule":
                    property.Known = true;
                    if (property.SubProperties != null)
                    {
                        Schedules.Add(new EntityOccasionSchedule(property.SubProperties, world));
                    }
                    break;
            }
        }

        Entity = entity;
    }
}


