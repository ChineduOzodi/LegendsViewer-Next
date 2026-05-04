using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class AddHfEntityHonor : WorldEvent
{
    public Entity? Entity { get; set; }
    public HistoricalFigure? HistoricalFigure { get; set; }
    public Honor? Honor { get; set; }
    public int HonorId { get; set; }

    public AddHfEntityHonor(List<Property> properties, IWorld world) : base(properties, world)
    {
        HonorId = -1;
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "entity_id": Entity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "hfid": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "honor_id": HonorId = Convert.ToInt32(property.Value); break;
            }
        }

        if (HonorId >= 0 && Entity != null)
        {
            // Use List.Find for O(n) lookup - faster than LINQ FirstOrDefault
            Honor = Entity.Honors.Find(h => h.Id == HonorId);
        }
        Entity?.AddEvent(this);
        HistoricalFigure?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        // Use StringBuilder instead of string concatenation for better performance
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(HistoricalFigure?.ToLink(link, pov, this));
        sb.Append($" received the title {Honor?.Name} in ");
        sb.Append(Entity?.ToLink(link, pov, this));
        
        string? requirementsString = Honor?.PrintRequirementsAsString();
        if (!string.IsNullOrWhiteSpace(requirementsString))
        {
            sb.Append($" after {requirementsString}");
        }
        
        sb.Append(PrintParentCollection(link, pov));
        sb.Append('.');
        return sb.ToString();
    }
}

