using System.Text;
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
            var sb = new StringBuilder();
            string title = GetTitle();
            sb.Append("the ");
            sb.Append(pov != this
                ? HtmlStyleUtil.GetAnchorString(Icon, "occasion", Id, title, Name)
                : HtmlStyleUtil.GetAnchorCurrentString(Icon, title, HtmlStyleUtil.CurrentDwarfObject(Name)));

            if (Site != null && pov != Site)
            {
                sb.Append(" in ");
                sb.Append(Site.ToLink(true, this));
            }
            else if (Region != null && pov != Region)
            {
                sb.Append(" in ");
                sb.Append(Region.ToLink(true, this));
            }
            else if (UndergroundRegion != null && pov != UndergroundRegion)
            {
                sb.Append(" in ");
                sb.Append(UndergroundRegion.ToLink(true, this));
            }
            return sb.ToString();
        }
        return ToString();
    }

    private string GetTitle()
    {
        var sb = new StringBuilder();
        sb.Append(Type);
        if (Site != null)
        {
            sb.Append("&#13");
            sb.Append("Site: ");
            sb.Append(Site.ToLink(false));
        }
        else if (Region != null)
        {
            sb.Append("&#13");
            sb.Append("Region: ");
            sb.Append(Region.ToLink(false));
        }
        else if (UndergroundRegion != null)
        {
            sb.Append("&#13");
            sb.Append("Underground Region: ");
            sb.Append(UndergroundRegion.ToLink(false));
        }
        return sb.ToString();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("the ");
        sb.Append(Name);

        if (Site != null)
        {
            sb.Append(" in ");
            sb.Append(Site.ToLink(true, this));
        }
        else if (Region != null)
        {
            sb.Append(" in ");
            sb.Append(Region.ToLink(true, this));
        }
        else if (UndergroundRegion != null)
        {
            sb.Append(" in ");
            sb.Append(UndergroundRegion.ToLink(true, this));
        }
        return sb.ToString();
    }
}

