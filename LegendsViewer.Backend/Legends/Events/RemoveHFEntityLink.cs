using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class RemoveHfEntityLink : WorldEvent
{
    public Entity? Entity { get; set; }
    public HistoricalFigure? HistoricalFigure { get; set; }
    public HfEntityLinkType LinkType { get; set; }
    public string? Position { get; set; }
    public int PositionId { get; set; }

    public RemoveHfEntityLink(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        LinkType = HfEntityLinkType.Unknown;
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "civ":
                case "civ_id":
                    Entity = world.GetEntity(Convert.ToInt32(property.Value));
                    break;
                case "hfid":
                case "histfig":
                    HistoricalFigure = world.GetHistoricalFigure(property.ValueAsInt());
                    break;
                case "link":
                case "link_type":
                    switch (property.Value.Replace("_", " "))
                    {
                        case "position":
                            LinkType = HfEntityLinkType.Position;
                            break;
                        case "prisoner":
                            LinkType = HfEntityLinkType.Prisoner;
                            break;
                        case "enemy":
                            LinkType = HfEntityLinkType.Enemy;
                            break;
                        case "member":
                            LinkType = HfEntityLinkType.Member;
                            break;
                        case "slave":
                            LinkType = HfEntityLinkType.Slave;
                            break;
                        case "squad":
                            LinkType = HfEntityLinkType.Squad;
                            break;
                        case "former member":
                            LinkType = HfEntityLinkType.FormerMember;
                            break;
                        default:
                            world.ParsingErrors.Report("Unknown HfEntityLinkType: " + property.Value);
                            break;
                    }
                    break;
                case "position":
                    Position = property.Value;
                    break;
                case "position_id":
                    PositionId = Convert.ToInt32(property.Value);
                    break;
            }
        }

        if (HistoricalFigure != null)
        {
            HistoricalFigure.AddEvent(this);
            if(PositionId != -1)
            {
                HistoricalFigure.EndPositionAssignment(Entity, Year, PositionId, Position ?? string.Empty);
            }
        }
        Entity.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        if (HistoricalFigure != null)
        {
            sb.Append(HistoricalFigure.ToLink(link, pov, this));
        }
        else
        {
            sb.Append("UNKNOWN HISTORICAL FIGURE");
        }
        switch (LinkType)
        {
            case HfEntityLinkType.Prisoner:
                sb.Append(" escaped from the prisons of ");
                break;
            case HfEntityLinkType.Slave:
                sb.Append(" fled from ");
                break;
            case HfEntityLinkType.Enemy:
                sb.Append(" stopped being an enemy of ");
                break;
            case HfEntityLinkType.Member:
                sb.Append(" left ");
                break;
            case HfEntityLinkType.Squad:
            case HfEntityLinkType.Position:
                EntityPosition? position = Entity?.EntityPositions.Find(pos => string.Equals(pos.Name, Position, StringComparison.OrdinalIgnoreCase) || pos.Id == PositionId);
                if (position != null)
                {
                    string positionName = position.GetTitleByCaste(HistoricalFigure?.Caste ?? string.Empty);
                    sb.Append(" stopped being the ");
                    sb.Append(positionName);
                    sb.Append(" of ");
                }
                else if (!string.IsNullOrWhiteSpace(Position))
                {
                    sb.Append(" stopped being the ");
                    sb.Append(Position);
                    sb.Append(" of ");
                }
                else
                {
                    sb.Append(" stopped being an unspecified position of ");
                }
                break;
            default:
                sb.Append(" stopped being linked to ");
                break;
        }

        sb.Append(Entity?.ToLink(link, pov, this));
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}

