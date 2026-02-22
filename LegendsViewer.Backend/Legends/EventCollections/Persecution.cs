using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;
using System.Text.Json.Serialization;

namespace LegendsViewer.Backend.Legends.EventCollections;

public class Persecution : EventCollection
{
    public int Ordinal { get; set; } = -1;
    public Location? Coordinates { get; set; }

    public Entity? TargetEntity { get; set; }

    [JsonIgnore]
    public List<HistoricalFigure> Deaths => GetSubEvents().OfType<HfDied>().Where(death => death.HistoricalFigure != null).Select(death => death.HistoricalFigure!).ToList();
    public int DeathCount => Deaths.Count;

    public Persecution(List<Property> properties, IWorld world)
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

        Name = $"{Formatting.AddOrdinal(Ordinal)} persecution";

        Icon = HtmlStyleUtil.GetIconString("gavel");
    }

    public override string ToLink(bool link = true, DwarfObject? pov = null, WorldEvent? worldEvent = null)
    {
        if (link)
        {
            var sb = new StringBuilder();
            string title = GetTitle();
            sb.Append("the ");
            sb.Append(pov != this
                ? HtmlStyleUtil.GetAnchorString(Icon, "persecution", Id, title, Name)
                : HtmlStyleUtil.GetAnchorCurrentString(Icon, title, HtmlStyleUtil.CurrentDwarfObject(Name)));

            if (TargetEntity != null && pov != TargetEntity)
            {
                sb.Append(" of ");
                sb.Append(TargetEntity.ToLink(true, this));
            }

            if (Site != null && pov != Site)
            {
                sb.Append(" in ");
                sb.Append(Site.ToLink(true, this));
            }
            return sb.ToString();
        }
        return ToString();
    }

    private string GetTitle()
    {
        var sb = new StringBuilder();
        sb.Append(Type);
        sb.Append("&#13");
        sb.Append("Of Entity: ");
        sb.Append(TargetEntity != null ? TargetEntity.ToLink(false) : "UNKNOWN");
        sb.Append("&#13");
        sb.Append("Site: ");
        sb.Append(Site != null ? Site.ToLink(false) : "UNKNOWN");
        return sb.ToString();
    }

    public override string ToString()
    {
        return $"the {Name} in {Site}";
    }
}


