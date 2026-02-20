using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using System.Text;

namespace LegendsViewer.Backend.Legends.Events;

public class ArtifactClaimFormed : WorldEvent
{
    public Artifact? Artifact { get; set; }
    public HistoricalFigure? HistoricalFigure { get; set; }
    public Entity? Entity { get; set; }
    public Claim Claim { get; set; }
    public int PositionProfileId { get; set; }
    public string? Circumstance { get; set; }

    public ArtifactClaimFormed(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "artifact_id":
                    Artifact = world.GetArtifact(Convert.ToInt32(property.Value));
                    break;
                case "hist_figure_id":
                    HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value));
                    break;
                case "entity_id":
                    Entity = world.GetEntity(Convert.ToInt32(property.Value));
                    break;
                case "claim":
                    switch (property.Value)
                    {
                        case "symbol":
                            Claim = Claim.Symbol;
                            break;
                        case "heirloom":
                        case "treasure":
                            Claim = Claim.Heirloom;
                            break;
                        default:
                            property.Known = false;
                            break;
                    }

                    break;
                case "position_profile_id":
                    PositionProfileId = Convert.ToInt32(property.Value);
                    break;
                case "circumstance":
                    Circumstance = property.Value;
                    break;
            }
        }

        Artifact?.AddEvent(this);
        HistoricalFigure?.AddEvent(this);
        Entity?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var eventString = new StringBuilder();
        eventString.Append(GetYearTime());
        eventString.Append(Artifact?.ToLink(link, pov, this));
        if ((Claim == Claim.Symbol ||
            Claim == Claim.Heirloom && HistoricalFigure != null) && Circumstance != "from afar")
        {
            eventString.Append(" was made a ");
            eventString.Append(Claim.GetDescription());
            if (PositionProfileId > -1 && Entity != null)
            {
                eventString.Append(" of the ");
                bool foundPosition = false;
                foreach (EntityPositionAssignment assignment in Entity.EntityPositionAssignments)
                {
                    EntityPosition? position =
                        Entity?.EntityPositions.Find(pos => pos.Id == assignment.PositionId);
                    if (position != null && assignment.HistoricalFigure != null)
                    {
                        string positionName = position.GetTitleByCaste(assignment.HistoricalFigure.Caste);
                        eventString.Append(positionName);
                        foundPosition = true;
                        break;
                    }
                }
                if (!foundPosition)
                {
                    eventString.Append("Position Title '").Append(PositionProfileId).Append("'");
                }
            }
        }
        else
        {
            eventString.Append(" was claimed");
        }
        if (Entity != null)
        {
            eventString.Append(" by ");
            eventString.Append(Entity.ToLink(link, pov, this));
        }
        if (HistoricalFigure != null)
        {
            eventString.Append(" by ");
            eventString.Append(HistoricalFigure.ToLink(link, pov, this));
        }

        if (!string.IsNullOrWhiteSpace(Circumstance))
        {
            eventString.Append(' ');
            eventString.Append(Circumstance);
        }
        eventString.Append(PrintParentCollection(link, pov));
        eventString.Append('.');
        return eventString.ToString();
    }
}

