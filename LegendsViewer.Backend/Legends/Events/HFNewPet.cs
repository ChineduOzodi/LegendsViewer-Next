using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Contracts;
using LegendsViewer.Backend.Legends.EventCollections;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.Events;

public class HfNewPet : WorldEvent
{
    public string Pet { get; set; } = string.Empty;
    public HistoricalFigure? HistoricalFigure { get; set; }
    public Site? Site { get; set; }
    public WorldRegion? Region { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }
    public Location? Coordinates { get; set; }

    public HfNewPet(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "coords": Coordinates = Formatting.ConvertToLocation(property.Value, world); break;
                case "group_hfid": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
                case "site": if (Site == null) { Site = world.GetSite(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
                case "group": if (HistoricalFigure == null) { HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
                case "pets":
                    var creatureInfo = world.GetCreatureInfo(property.Value);
                    if (creatureInfo != CreatureInfo.Unknown)
                    {
                        Pet = creatureInfo.NameSingular;
                    }
                    else
                    {
                        Pet = Formatting.FormatRace(property.Value.Replace("_", " ").Replace("2", "two"));
                    }
                    break;
            }
        }

        HistoricalFigure?.AddEvent(this);
        Site?.AddEvent(this);
        Region?.AddEvent(this);
        UndergroundRegion?.AddEvent(this);

        if (!string.IsNullOrWhiteSpace(Pet) && HistoricalFigure != null)
        {
            var journeyPet = HistoricalFigure.JourneyPets.Find(pet => pet.Title == Pet);
            var tameLocation = Site?.ToLink() ?? Region?.ToLink() ?? UndergroundRegion?.ToLink();
            if (journeyPet != null && tameLocation != null)
            {
                journeyPet.Subtitle = $"tamed in {tameLocation}";
            }
            else
            {
                HistoricalFigure.JourneyPets.Add(new ListItemDto
                {
                    Title = Pet,
                    Subtitle = tameLocation != null ? $"tamed in {tameLocation}" : null
                });
            }
        }
    }
    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        string eventString = GetYearTime() + HistoricalFigure?.ToLink(link, pov, this) + " tamed the creatures named ";
        if (!string.IsNullOrWhiteSpace(Pet))
        {
            eventString += Pet;
        }
        else
        {
            eventString += "UNKNOWN";
        }
        if (Site != null)
        {
            eventString += " in " + Site.ToLink(link, pov, this);
        }
        else if (Region != null)
        {
            eventString += " in " + Region.ToLink(link, pov, this);
        }
        else if (UndergroundRegion != null)
        {
            eventString += " in " + UndergroundRegion.ToLink(link, pov, this);
        }
        if (!(ParentCollection is Journey))
        {
            eventString += PrintParentCollection(link, pov);
        }
        eventString += ".";
        return eventString;
    }
}

