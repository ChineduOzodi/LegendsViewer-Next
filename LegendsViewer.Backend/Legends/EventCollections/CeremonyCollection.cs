using LegendsViewer.Backend.Extensions;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.EventCollections;

public class CeremonyCollection : EventCollection, IHasComplexSubtype
{
    public int Ordinal { get; set; } = -1;

    public CeremonyCollection(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "ordinal": Ordinal = Convert.ToInt32(property.Value); break;
            }
        }

        Name = $"{Formatting.AddOrdinal(Ordinal)} ceremony";
        Icon = HtmlStyleUtil.GetIconString("human-greeting-variant");
    }

    public void GenerateComplexSubType()
    {
        this.EnrichWithOccasionEventData();
    }

    public override string ToLink(bool link = true, DwarfObject? pov = null, WorldEvent? worldEvent = null)
    {
        if (link)
        {
            string title = GetTitle();
            string linkedString = "the ";
            linkedString += pov != this
                ? HtmlStyleUtil.GetAnchorString(Icon, "ceremony", Id, title, Name)
                : HtmlStyleUtil.GetAnchorCurrentString(Icon, title, HtmlStyleUtil.CurrentDwarfObject(Name));

            if (Site != null && pov != Site)
            {
                linkedString += $" in {Site.ToLink(true, this)}";
            }
            else if (Region != null && pov != Region)
            {
                linkedString += $" in {Region.ToLink(true, this)}";
            }
            else if (UndergroundRegion != null && pov != UndergroundRegion)
            {
                linkedString += $" in {UndergroundRegion.ToLink(true, this)}";
            }
            return linkedString;
        }
        return ToString();
    }

    private string GetTitle()
    {
        string title = Type;
        if (Site != null)
        {
            title += "&#13";
            title += "Site: ";
            title += Site.ToLink(false);
        }
        else if (Region != null)
        {
            title += "&#13";
            title += "Region: ";
            title += Region.ToLink(false);
        }
        else if (UndergroundRegion != null)
        {
            title += "&#13";
            title += "Underground Region: ";
            title += UndergroundRegion.ToLink(false);
        }
        return title;
    }

    public override string ToString()
    {
        string text = "the ";
        text += Name;

        if (Site != null)
        {
            text += $" in {Site.ToLink(true, this)}";
        }
        else if (Region != null)
        {
            text += $" in {Region.ToLink(true, this)}";
        }
        else if (UndergroundRegion != null)
        {
            text += $" in {UndergroundRegion.ToLink(true, this)}";
        }
        return text;
    }
}

