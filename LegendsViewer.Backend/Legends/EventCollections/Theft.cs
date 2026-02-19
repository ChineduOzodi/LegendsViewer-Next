using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.EventCollections;

public class Theft : EventCollection, IHasComplexSubtype
{
    public int Ordinal { get; set; } = -1;
    public Location? Coordinates { get; set; }
    public Entity? Attacker { get; set; }
    public Entity? Defender { get; set; }

    public Theft(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "ordinal": Ordinal = Convert.ToInt32(property.Value); break;
                case "coords": Coordinates = Formatting.ConvertToLocation(property.Value, world); break;
                case "parent_eventcol": ParentCollection = world.GetEventCollection(Convert.ToInt32(property.Value)); break;
                case "attacking_enid": Attacker = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "defending_enid": Defender = world.GetEntity(Convert.ToInt32(property.Value)); break;
            }
        }

        foreach (ItemStolen theft in Events.OfType<ItemStolen>())
        {
            if (theft.Site == null)
            {
                theft.Site = Site;
            }
            if (Site != null && !Site.Events.Contains(theft))
            {
                Site.AddEvent(theft);
                Site.Events = Site.Events.OrderBy(ev => ev.Id).ToList();
            }
            if (Attacker != null && Attacker.SiteHistory.Count == 1)
            {
                if (theft.ReturnSite == null)
                {
                    theft.ReturnSite = Attacker.SiteHistory[0].Site;
                }
                if (!theft.ReturnSite.Events.Contains(theft))
                {
                    theft.ReturnSite.AddEvent(theft);
                    theft.ReturnSite.Events = theft.ReturnSite.Events.OrderBy(ev => ev.Id).ToList();
                }
            }
        }
        Attacker?.AddEventCollection(this);
        Defender?.AddEventCollection(this);

        Name = $"{Formatting.AddOrdinal(Ordinal)} theft";
        Icon = HtmlStyleUtil.GetIconString("handcuffs");
    }

    public void GenerateComplexSubType()
    {
        if (string.IsNullOrEmpty(Subtype))
        {
            Subtype = $"{Attacker?.ToLink(true, this)} => {Defender?.ToLink(true, this)}";
        }
    }

    public override string ToLink(bool link = true, DwarfObject? pov = null, WorldEvent? worldEvent = null)
    {
        if (link)
        {
            string title = GetTitle();
            string linkedString = "the ";
            linkedString += pov != this
                ? HtmlStyleUtil.GetAnchorString(Icon, "theft", Id, title, Name)
                : HtmlStyleUtil.GetAnchorCurrentString(Icon, title, HtmlStyleUtil.CurrentDwarfObject(Name));

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
        title += Attacker?.PrintEntity(false) + " (Attacker)";
        title += "&#13";
        title += Defender?.PrintEntity(false) + " (Defender)";
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

