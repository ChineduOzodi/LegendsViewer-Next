using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class ArtifactGiven : WorldEvent
{
    public Artifact? Artifact { get; set; }
    public HistoricalFigure? HistoricalFigureGiver { get; set; }
    public HistoricalFigure? HistoricalFigureReceiver { get; set; }
    public Entity? EntityGiver { get; set; }
    public Entity? EntityReceiver { get; set; }
    public ArtifactReason ArtifactReason { get; set; }

    // http://www.bay12games.com/dwarves/mantisbt/view.php?id=11350
    // 0011350: "artifact given" event sometimes has the same <giver_hist_figure_id> and <receiver_hist_figure_id>
    public ArtifactGiven(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "artifact_id": Artifact = world.GetArtifact(Convert.ToInt32(property.Value)); break;
                case "giver_hist_figure_id": HistoricalFigureGiver = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "receiver_hist_figure_id": HistoricalFigureReceiver = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "giver_entity_id": EntityGiver = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "receiver_entity_id": EntityReceiver = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "reason":
                    switch (property.Value)
                    {
                        case "cement bonds of friendship":
                            ArtifactReason = ArtifactReason.CementBondsOfFriendship;
                            break;
                        case "part of trade negotiation":
                            ArtifactReason = ArtifactReason.PartOfTradeNegotiation;
                            break;
                        default:
                            property.Known = false;
                            break;
                    }
                    break;
            }
        }

        Artifact?.AddEvent(this);
        HistoricalFigureGiver?.AddEvent(this);
        if (HistoricalFigureGiver != HistoricalFigureReceiver)
        {
            HistoricalFigureReceiver?.AddEvent(this);
        }
        EntityGiver?.AddEvent(this);
        EntityReceiver?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Artifact?.ToLink(link, pov, this));
        sb.Append(" was offered to ");
        if (HistoricalFigureReceiver != null)
        {
            sb.Append(HistoricalFigureReceiver.ToLink(link, pov, this));
            if (EntityReceiver != null)
            {
                sb.Append(" of ");
            }
        }
        if (EntityReceiver != null)
        {
            sb.Append(EntityReceiver.ToLink(link, pov, this));
        }

        sb.Append(" by ");
        if (HistoricalFigureGiver != null)
        {
            sb.Append(HistoricalFigureGiver.ToLink(link, pov, this));
            if (EntityGiver != null)
            {
                sb.Append(" of ");
            }
        }
        if (EntityGiver != null)
        {
            sb.Append(EntityGiver.ToLink(link, pov, this));
        }
        if (ArtifactReason != ArtifactReason.Unknown)
        {
            switch (ArtifactReason)
            {
                case ArtifactReason.CementBondsOfFriendship:
                    sb.Append(" in order to cement the bonds of friendship");
                    break;
                case ArtifactReason.PartOfTradeNegotiation:
                    sb.Append(" as part of a trade negotiation");
                    break;
            }
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append('.');
        return sb.ToString();
    }
}


