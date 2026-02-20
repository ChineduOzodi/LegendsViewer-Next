using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class EntityAllianceFormed : WorldEvent
{
    public Entity? InitiatingEntity { get; set; }
    public Entity? JoiningEntity { get; set; }

    public EntityAllianceFormed(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "initiating_enid": InitiatingEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "joining_enid": JoiningEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
            }
        }


        InitiatingEntity?.AddEvent(this);
        JoiningEntity?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(JoiningEntity?.ToLink(link, pov, this));
        sb.Append(" swore to support ");
        sb.Append(InitiatingEntity?.ToLink(link, pov, this));
        sb.Append(" in war if the latter did likewise");

        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}


