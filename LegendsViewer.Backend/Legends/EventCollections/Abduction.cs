using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.EventCollections;

public class Abduction : EventCollection
{
    public int Ordinal { get; set; } = -1;
    public Location? Coordinates { get; set; }

    public HistoricalFigure? Abductee { get; set; }
    public Entity? Attacker { get; set; }
    public Entity? Defender { get; set; }

    public Abduction(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "ordinal": Ordinal = Convert.ToInt32(property.Value); break;
                case "coords": Coordinates = Formatting.ConvertToLocation(property.Value, world); break;
                case "parent_eventcol": ParentCollection = world.GetEventCollection(Convert.ToInt32(property.Value)); break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "attacking_enid": Attacker = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "defending_enid": Defender = world.GetEntity(Convert.ToInt32(property.Value)); break;
            }
        }

        Abductee?.AddEventCollection(this);
        Attacker?.AddEventCollection(this);
        Defender?.AddEventCollection(this);

        Name = $"{Formatting.AddOrdinal(Ordinal)} abduction";
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
                ? HtmlStyleUtil.GetAnchorString(Icon, "abduction", Id, title, Name)
                : HtmlStyleUtil.GetAnchorCurrentString(Icon, title, HtmlStyleUtil.CurrentDwarfObject(Name)));
            if (Abductee == null)
            {
                var abductionEvent = GetSubEvents().OfType<HfAbducted>().FirstOrDefault();
                if (abductionEvent != null)
                {
                    Abductee = abductionEvent.Target;
                }
            }
            if (Abductee != null && pov != Abductee)
            {
                sb.Append(" of ");
                sb.Append(Abductee.ToLink(true, this));
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
        sb.Append("Abductee: ");
        sb.Append(Abductee != null ? Abductee.ToLink(false) : "UNKNOWN");
        sb.Append("&#13");
        sb.Append("Site: ");
        sb.Append(Site != null ? Site.ToLink(false) : "UNKNOWN");
        return sb.ToString();
    }

    public override string ToString()
    {
        return $"the {Name} of {Abductee?.Name} in {Site}";
    }
}


