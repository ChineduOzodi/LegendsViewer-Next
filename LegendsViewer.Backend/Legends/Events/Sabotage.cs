using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class Sabotage : WorldEvent
{
    public HistoricalFigure? SaboteurHf { get; set; }
    public HistoricalFigure? TargetHf { get; set; }
    public Site? Site { get; set; }

    public Sabotage(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "saboteur_hfid":
                    SaboteurHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value));
                    break;
                case "target_hfid":
                    TargetHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value));
                    break;
                case "site_id":
                    Site = world.GetSite(Convert.ToInt32(property.Value));
                    break;
            }
        }

        SaboteurHf.AddEvent(this);
        TargetHf.AddEvent(this);
        Site.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(SaboteurHf?.ToLink(link, pov, this));
        sb.Append(" sabotaged the activities of ");
        sb.Append(TargetHf?.ToLink(link, pov, this));
        if (Site != null)
        {
            sb.Append(" at ");
            sb.Append(Site.ToLink(link, pov, this));
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}


