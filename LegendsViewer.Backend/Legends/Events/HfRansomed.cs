using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.Events;

public class HfRansomed : WorldEvent
{
    public HistoricalFigure? RansomedHf { get; set; }
    public HistoricalFigure? RansomerHf { get; set; }
    public HistoricalFigure? PayerHf { get; set; }
    public Entity? PayerEntity { get; set; }
    public Site? MovedToSite { get; set; }

    public HfRansomed(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "ransomed_hfid": RansomedHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "ransomer_hfid": RansomerHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "payer_hfid": PayerHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "payer_entity_id": PayerEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "moved_to_site_id": MovedToSite = world.GetSite(Convert.ToInt32(property.Value)); break;
            }
        }

        PayerHf.AddEvent(this);
        PayerEntity.AddEvent(this);
        RansomedHf.AddEvent(this);
        RansomerHf.AddEvent(this);
        MovedToSite.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(RansomerHf?.ToLink(link, pov, this));
        sb.Append(" ransomed ");
        sb.Append(RansomedHf?.ToLink(link, pov, this));
        if (PayerHf != null)
        {
            sb.Append(" to ");
            sb.Append(PayerHf.ToLink(link, pov, this));
            if (PayerEntity != null)
            {
                sb.Append(" of ");
                sb.Append(PayerEntity.ToLink(link, pov, this));
            }
        }
        else if (PayerEntity != null)
        {
            sb.Append(" to ");
            sb.Append(PayerEntity.ToLink(link, pov, this));
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(". ");
        if (MovedToSite != null)
        {
            sb.Append(RansomedHf?.ToLink(link, pov, this).ToUpperFirstLetter());
            sb.Append(" was sent to ");
            sb.Append(MovedToSite.ToLink(link, pov, this));
        }
        return sb.ToString();
    }
}
