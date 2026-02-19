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
            string title = GetTitle();
            string linkedString = "the ";
            linkedString += pov != this
                ? HtmlStyleUtil.GetAnchorString(Icon, "insurrection", Id, title, Name)
                : HtmlStyleUtil.GetAnchorCurrentString(Icon, title, HtmlStyleUtil.CurrentDwarfObject(Name));

            if (TargetEntity != null && pov != TargetEntity)
            {
                linkedString += $" of {TargetEntity.ToLink(true, this)}";
            }

            if (Site != null && pov != Site)
            {
                linkedString += $" in {Site.ToLink(true, this)}";
            }
            return linkedString;
        }
        return ToString();
    }

    private string GetTitle()
    {
        string title = Type;
        title += "&#13";
        title += "Of Entity: ";
        title += TargetEntity != null ? TargetEntity.ToLink(false) : "UNKNOWN";
        title += "&#13";
        title += "Site: ";
        title += Site != null ? Site.ToLink(false) : "UNKNOWN";
        return title;
    }

    public override string ToString()
    {
        return $"the {Name} in {Site}";
    }
}


