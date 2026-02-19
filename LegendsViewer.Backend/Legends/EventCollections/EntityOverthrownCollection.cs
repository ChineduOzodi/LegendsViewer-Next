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
            string title = GetTitle();

            string linkedString = pov != this
                ? HtmlStyleUtil.GetAnchorString(Icon, "coup", Id, title, Name)
                : HtmlStyleUtil.GetAnchorCurrentString(Icon, title, HtmlStyleUtil.CurrentDwarfObject(Name));

            if (Site != null && pov != Site)
            {
                linkedString += $" in {Site.ToLink(true, this)}";
            }

            if (Region != null && pov != Region)
            {
                linkedString += $" in {Region.ToLink(true, this)}";
            }

            if (UndergroundRegion != null && pov != UndergroundRegion)
            {
                linkedString += $" in {UndergroundRegion.ToLink(true, this)}";
            }
            return linkedString;
        }
        return Name;
    }

    private string GetTitle()
    {
        string title = Type;
        title += "&#13";
        title += "Target: ";
        title += TargetEntity != null ? TargetEntity.ToLink(false) : "UNKNOWN";
        return title;
    }

    public override string ToString()
    {
        return $"the {Name} against {TargetEntity?.Name}";
    }
}


