using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using System.Text.Json.Serialization;

namespace LegendsViewer.Backend.Legends.Various;

public class SiteProperty
{
    public int Id { get; set; }
    public SitePropertyType Type { get; set; }

    [JsonIgnore]
    private int OwnerId { get; set; }

    [JsonIgnore]
    public HistoricalFigure? Owner { get; set; }
    public string? OwnerLink => Owner?.ToLink(true);

    [JsonIgnore]
    public Structure? Structure { get; set; }
    public string? StructureLink => Structure?.ToLink(true);

    [JsonIgnore]
    public Site? Site { get; set; }
    public string? SiteLink => Site?.ToLink(true);

    public SiteProperty(List<Property> properties, IWorld world, Site site)
    {
        Id = -1;
        OwnerId = -1;
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "id":
                    Id = Convert.ToInt32(property.Value);
                    break;
                case "type":
                    switch (property.Value)
                    {
                        case "house": Type = SitePropertyType.House; break;
                        default:
                            property.Known = false;
                            break;
                    }
                    break;
                case "owner_hfid":
                    OwnerId = Convert.ToInt32(property.Value);
                    break;
                case "structure_id":
                    Structure = site.Structures.Find(structure => structure.LocalId == Convert.ToInt32(property.Value));
                    break;
                default:
                    property.Known = false;
                    break;
            }
        }

        Site = site;
    }

    public string Print(bool link = true, DwarfObject? pov = null)
    {
        return Structure != null ? Structure.ToLink(link, pov) : "a " + Type.GetDescription();
    }

    public void Resolve(IWorld world)
    {
        Owner = world.GetHistoricalFigure(OwnerId);
        Owner?.SiteProperties.Add(this);
        if (Structure != null && Owner != null)
        {
            Structure.Owner = Owner;
        }
    }
}



