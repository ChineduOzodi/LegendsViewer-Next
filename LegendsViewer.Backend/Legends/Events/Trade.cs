using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class Trade : WorldEvent
{
    public HistoricalFigure? Trader { get; set; }
    public Entity? TraderEntity { get; set; }
    public Site? SourceSite { get; set; }
    public Site? DestSite { get; set; }
    public int AccountShift { get; set; }

    // TODO find out what these properties do
    public int ProductionZoneId { get; set; }
    public int Allotment { get; set; }
    public int AllotmentIndex { get; set; }

    public Trade(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "trader_hfid": Trader = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "trader_entity_id": TraderEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "source_site_id": SourceSite = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "dest_site_id": DestSite = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "production_zone_id": ProductionZoneId = Convert.ToInt32(property.Value); break;
                case "allotment": Allotment = Convert.ToInt32(property.Value); break;
                case "allotment_index": AllotmentIndex = Convert.ToInt32(property.Value); break;
                case "account_shift": AccountShift = Convert.ToInt32(property.Value); break;
            }
        }


        SourceSite.AddEvent(this);
        DestSite.AddEvent(this);
        Trader.AddEvent(this);
        TraderEntity.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Trader?.ToLink(link, pov, this));
        if (TraderEntity != null)
        {
            sb.Append(" of ");
            sb.Append(TraderEntity.ToLink(link, pov, this));
        }
        // same ranges like in "gamble" event
        var balance = AccountShift;
        if (balance >= 5000)
        {
            sb.Append(" made a fortune");
        }
        else if (balance >= 1000)
        {
            sb.Append(" did well");
        }
        else if (balance <= -1000)
        {
            sb.Append(" did poorly");
        }
        else if (balance <= -5000)
        {
            sb.Append(" lost a fortune");
        }
        else
        {
            sb.Append(" broke even");
        }
        sb.Append(" trading goods");
        if (SourceSite != null)
        {
            sb.Append(" from ");
            sb.Append(SourceSite.ToLink(link, pov, this));
        }

        if (DestSite != null)
        {
            sb.Append(" to ");
            sb.Append(DestSite.ToLink(link, pov, this));
        }

        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}


