using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Various;

public class EntityPositionAssignment
{
    public int Id { get; set; } = -1;
    public HistoricalFigure? HistoricalFigure { get; set; }
    public int PositionId { get; set; } = -1;
    public int SquadId { get; set; } = -1;

    public EntityPositionAssignment(List<Property> properties, IWorld world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "id": Id = Convert.ToInt32(property.Value); break;
                case "histfig": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "position_id": PositionId = Convert.ToInt32(property.Value); break;
                case "squad_id": SquadId = Convert.ToInt32(property.Value); break;
            }
        }
    }
}


