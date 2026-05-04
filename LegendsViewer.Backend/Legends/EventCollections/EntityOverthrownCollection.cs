using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.EventCollections;

public class EntityOverthrownCollection : EventCollection
{
    public int Ordinal { get; set; } = -1;
    public Location? Coordinates { get; set; }
    public Entity? TargetEntity { get; set; }

    public EntityOverthrownCollection(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "ordinal": Ordinal = Convert.ToInt32(property.Value); break;
                case "coords": Coordinates = Formatting.ConvertToLocation(property.Value, world); break;
                case "parent_eventcol": ParentCollection = world.GetEventCollection(Convert.ToInt32(property.Value)); break;
                case "target_entity_id": TargetEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
            }
        }

        TargetEntity?.AddEventCollection(this);

        Name = $"{Formatting.AddOrdinal(Ordinal)} coup";

        Icon = HtmlStyleUtil.GetIconString("alert-decagram-outline");
    }
    public override string ToLink(bool link = true, DwarfObject? pov = null, WorldEvent? worldEvent = null)
    {
        if (link)
        {
            var sb = new StringBuilder();
            string title = GetTitle();
            sb.Append(pov != this
                ? HtmlStyleUtil.GetAnchorString(Icon, "coup", Id, title, Name)
                : HtmlStyleUtil.GetAnchorCurrentString(Icon, title, HtmlStyleUtil.CurrentDwarfObject(Name)));

            if (Site != null && pov != Site)
            {
                sb.Append(" in ");
                sb.Append(Site.ToLink(true, this));
            }

            if (Region != null && pov != Region)
            {
                sb.Append(" in ");
                sb.Append(Region.ToLink(true, this));
            }

            if (UndergroundRegion != null && pov != UndergroundRegion)
            {
                sb.Append(" in ");
                sb.Append(UndergroundRegion.ToLink(true, this));
            }
            return sb.ToString();
        }
        return Name;
    }

    private string GetTitle()
    {
        var sb = new StringBuilder();
        sb.Append(Type);
        sb.Append("&#13");
        sb.Append("Target: ");
        sb.Append(TargetEntity != null ? TargetEntity.ToLink(false) : "UNKNOWN");
        return sb.ToString();
    }

    public override string ToString()
    {
        return $"the {Name} against {TargetEntity?.Name}";
    }
}


