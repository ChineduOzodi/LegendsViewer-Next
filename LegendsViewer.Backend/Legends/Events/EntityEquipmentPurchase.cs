using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class EntityEquipmentPurchase : WorldEvent
{
    public Entity? Entity { get; set; }
    public HistoricalFigure? HistoricalFigure { get; set; }
    public int Quality { get; set; }

    public EntityEquipmentPurchase(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "entity_id": Entity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "hfid": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "new_equipment_level": Quality = Convert.ToInt32(property.Value); break;
            }
        }

        Entity?.AddEvent(this);
        HistoricalFigure?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Entity?.ToLink(link, pov, this));
        sb.Append(" purchased ");
        if (Quality == 1)
        {
            sb.Append("well-crafted ");
        }
        else if (Quality == 2)
        {
            sb.Append("finely-crafted ");
        }
        else if (Quality == 3)
        {
            sb.Append("superior quality ");
        }
        else if (Quality == 4)
        {
            sb.Append("exceptional ");
        }
        else if (Quality == 5)
        {
            sb.Append("masterwork ");
        }
        sb.Append("equipment");
        if (HistoricalFigure != null)
        {
            sb.Append(", which ");
            sb.Append(HistoricalFigure.ToLink(link, pov, this));
            sb.Append(" received");
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}

