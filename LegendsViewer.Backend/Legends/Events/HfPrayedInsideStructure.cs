using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class HfPrayedInsideStructure : WorldEvent
{
    public HistoricalFigure? HistoricalFigure { get; set; }
    public Site? Site { get; set; }
    public int StructureId { get; set; }
    public Structure? Structure { get; set; }
    public string? Action { get; set; }

    public HfPrayedInsideStructure(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "histfig":
                case "hist_fig_id":
                    if (HistoricalFigure == null)
                    {
                        HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value));
                    }
                    else
                    {
                        property.Known = true;
                    }
                    break;
                case "site":
                case "site_id":
                    if (Site == null)
                    {
                        Site = world.GetSite(Convert.ToInt32(property.Value));
                    }
                    else
                    {
                        property.Known = true;
                    }
                    break;
                case "structure":
                case "structure_id":
                    StructureId = Convert.ToInt32(property.Value);
                    break;
                case "action":
                    if (property.Value == "prayedinside")
                    {
                        Action = property.Value;
                    }
                    else
                    {
                        property.Known = false;
                    }
                    break;
            }
        }

        if (Site != null)
        {
            Structure = Site.Structures.Find(structure => structure.LocalId == StructureId);
        }
        HistoricalFigure.AddEvent(this);
        Site.AddEvent(this);
        Structure.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(HistoricalFigure?.ToLink(link, pov, this));
        sb.Append(" prayed");
        if (Structure != null)
        {
            sb.Append(" inside ");
            sb.Append(Structure.ToLink(link, pov, this));
        }
        if (Site != null)
        {
            sb.Append(" in ");
            sb.Append(Site.ToLink(link, pov, this));
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}
