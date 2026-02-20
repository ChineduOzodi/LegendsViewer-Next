using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using System.Text;

namespace LegendsViewer.Backend.Legends.Events;

public class AddHfSiteLink : WorldEvent
{
    public int StructureId { get; set; }
    public Structure? Structure { get; set; }
    public Entity? Civ { get; set; }
    public HistoricalFigure? HistoricalFigure { get; set; }
    public Site? Site { get; set; }
    public SiteLinkType LinkType { get; set; }

    public AddHfSiteLink(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            LinkType = SiteLinkType.Unknown;
            switch (property.Name)
            {
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "structure": StructureId = Convert.ToInt32(property.Value); break;
                case "civ": Civ = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "histfig": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "link_type":
                    switch (property.Value.Replace("_", " "))
                    {
                        case "lair": LinkType = SiteLinkType.Lair; break;
                        case "hangout": LinkType = SiteLinkType.Hangout; break;
                        case "home site building": LinkType = SiteLinkType.HomeSiteBuilding; break;
                        case "home site underground": LinkType = SiteLinkType.HomeSiteUnderground; break;
                        case "home structure": LinkType = SiteLinkType.HomeStructure; break;
                        case "seat of power": LinkType = SiteLinkType.SeatOfPower; break;
                        case "occupation": LinkType = SiteLinkType.Occupation; break;
                        case "home site realization building": LinkType = SiteLinkType.HomeSiteRealizationBuilding; break;
                        case "home site abstract building": LinkType = SiteLinkType.HomeSiteAbstractBuilding; break;
                        case "prison site building profile": LinkType = SiteLinkType.PrisonSiteBuildingProfile; break;
                        default:
                            property.Known = false;
                            break;
                    }
                    break;
                case "site": if (Site == null) { Site = world.GetSite(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
            }
        }
        HistoricalFigure?.AddEvent(this);
        Civ?.AddEvent(this);
        Site?.AddEvent(this);
        Structure?.AddEvent(this);
    }
    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var eventString = new StringBuilder();
        eventString.Append(GetYearTime());
        eventString.Append(HistoricalFigure != null ? HistoricalFigure.ToLink(link, pov, this) : "UNKNOWN HISTORICAL FIGURE");
        switch (LinkType)
        {
            case SiteLinkType.HomeSiteAbstractBuilding:
            case SiteLinkType.HomeSiteRealizationBuilding:
                eventString.Append(" took up residence in ");
                break;
            case SiteLinkType.Hangout:
                eventString.Append(" ruled from ");
                break;
            case SiteLinkType.SeatOfPower:
                eventString.Append(" started working from ");
                break;
            case SiteLinkType.Occupation:
                eventString.Append(" started working at ");
                break;
            default:
                eventString.Append(" UNKNOWN LINKTYPE (").Append(LinkType).Append(") ");
                break;
        }
        if (Site != null)
        {
            Structure = Site.Structures.Find(structure => structure.LocalId == StructureId);
        }
        eventString.Append(Structure != null ? Structure.ToLink(link, pov, this) : "UNKNOWN STRUCTURE");
        if (Civ != null)
        {
            eventString.Append(" of ").Append(Civ.ToLink(link, pov, this));
        }
        if (Site != null)
        {
            eventString.Append(" in ").Append(Site.ToLink(link, pov, this));
        }
        eventString.Append(PrintParentCollection(link, pov));
        eventString.Append('.');
        return eventString.ToString();
    }
}

