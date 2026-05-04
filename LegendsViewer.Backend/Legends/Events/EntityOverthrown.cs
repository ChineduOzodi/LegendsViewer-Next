using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class EntityOverthrown : WorldEvent
{
    public Entity? Entity { get; set; }
    public int PositionProfileId { get; set; }
    public HistoricalFigure? OverthrownHistoricalFigure { get; set; }
    public Site? Site { get; set; }
    public HistoricalFigure? PositionTaker { get; set; }
    public HistoricalFigure? Instigator { get; set; }
    public List<HistoricalFigure> Conspirators { get; set; } = [];

    public EntityOverthrown(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "entity_id": Entity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "position_profile_id": PositionProfileId = Convert.ToInt32(property.Value); break;
                case "overthrown_hfid": OverthrownHistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "pos_taker_hfid": PositionTaker = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "instigator_hfid": Instigator = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "conspirator_hfid":
                    HistoricalFigure? conspirator = world.GetHistoricalFigure(Convert.ToInt32(property.Value));
                    if (conspirator != null)
                    {
                        Conspirators.Add(conspirator);
                    }
                    break;
            }
        }

        Entity?.AddEvent(this);
        OverthrownHistoricalFigure?.AddEvent(this);
        Site?.AddEvent(this);
        PositionTaker?.AddEvent(this);
        if (Instigator != PositionTaker)
        {
            Instigator?.AddEvent(this);
        }
        foreach (HistoricalFigure conspirator in Conspirators)
        {
            if (conspirator != PositionTaker && conspirator != Instigator)
            {
                conspirator.AddEvent(this);
            }
        }
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Instigator?.ToLink(link, pov, this));
        sb.Append(" toppled the government of ");
        sb.Append(OverthrownHistoricalFigure?.ToLink(link, pov, this));
        sb.Append(" of ");
        sb.Append(Entity?.ToLink(link, pov, this));
        if (PositionTaker != Instigator)
        {
            sb.Append(" placed ");
            sb.Append(PositionTaker?.ToLink(link, pov, this));
            sb.Append(" in power");
        }
        else
        {
            sb.Append(" and assumed control");
        }
        if (Site != null)
        {
            sb.Append(" in ");
            sb.Append(Site.ToLink(link, pov, this));
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        if (Conspirators.Count > 0)
        {
            sb.Append(" The support of ");
            for (int i = 0; i < Conspirators.Count; i++)
            {
                var conspirator = Conspirators[i];
                sb.Append(conspirator.ToLink(link, pov, this));
                if (Conspirators.Count - i > 2)
                {
                    sb.Append(", ");
                }
                else if (Conspirators.Count - i == 2)
                {
                    sb.Append(" and ");
                }
            }
            sb.Append("was crucial to the coup.");
        }
        return sb.ToString();
    }
}


