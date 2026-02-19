using LegendsViewer.Backend.Extensions;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;
using System.Text.Json.Serialization;

namespace LegendsViewer.Backend.Legends.EventCollections;

public class Occasion : EventCollection, IHasComplexSubtype
{
    public int Ordinal { get; set; } = -1;
    [JsonIgnore]
    public Entity? Civ { get; set; }
    [JsonIgnore]
    public int OccasionId { get; set; }
    [JsonIgnore]
    public EntityOccasion? EntityOccasion { get; set; }

    public Occasion(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "civ_id": Civ = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "ordinal": Ordinal = Convert.ToInt32(property.Value); break;
                case "occasion_id": OccasionId = Convert.ToInt32(property.Value); break;
            }
        }
        if (Civ != null)
        {
            Civ.EntityType = EntityType.Civilization;
            Civ.IsCiv = true;
        }
        Civ?.AddEventCollection(this);
        if (Civ?.Occassions.Count > 0)
        {
            EntityOccasion = Civ.Occassions.ElementAt(OccasionId);
        }

        Name = $"{Formatting.AddOrdinal(Ordinal)} occasion";
        if (EntityOccasion != null && !string.IsNullOrWhiteSpace(EntityOccasion.Name))
        {
            Name += $" of {EntityOccasion.Name}";
        }
        Icon = HtmlStyleUtil.GetIconString("calendar-star");
    }

    public void GenerateComplexSubType()
    {
        if (string.IsNullOrEmpty(Subtype) && Civ != null)
        {
            Subtype = Civ.ToLink(true, this);
        }
    }

    public override string ToLink(bool link = true, DwarfObject? pov = null, WorldEvent? worldEvent = null)
    {
        if (link)
        {
            string title = GetTitle();
            string linkedString = "the ";
            linkedString += pov != this
                ? HtmlStyleUtil.GetAnchorString(Icon, "occasion", Id, title, Name)
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

