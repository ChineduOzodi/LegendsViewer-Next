using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;
using System.Text.Json.Serialization;

namespace LegendsViewer.Backend.Legends.EventCollections;

public class Insurrection : EventCollection
{
    public int Ordinal { get; set; } = -1;
    public Entity? TargetEntity { get; set; }

    [JsonIgnore]
    public List<HistoricalFigure> Deaths => GetSubEvents().OfType<HfDied>().Where(death => death.HistoricalFigure != null).Select(death => death.HistoricalFigure!).ToList();
    public int DeathCount => Deaths.Count;

    public Insurrection(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "target_enid":
                    TargetEntity = world.GetEntity(Convert.ToInt32(property.Value));
                    break;
                case "ordinal":
                    Ordinal = Convert.ToInt32(property.Value);
                    break;
            }
        }

        var insurrectionStart = Events.OfType<InsurrectionStarted>().FirstOrDefault();
        if (insurrectionStart != null)
        {
            insurrectionStart.ActualStart = true;
        }
        TargetEntity?.AddEventCollection(this);

        Name = $"{Formatting.AddOrdinal(Ordinal)} insurrection";
        Icon = HtmlStyleUtil.GetIconString("map-marker-alert");
    }

    public override string ToLink(bool link = true, DwarfObject? pov = null, WorldEvent? worldEvent = null)
    {
        if (link)
        {
            var sb = new StringBuilder();
            string title = GetTitle();
            sb.Append("the ");
            sb.Append(pov != this
                ? HtmlStyleUtil.GetAnchorString(Icon, "insurrection", Id, title, Name)
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


