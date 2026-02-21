using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class ModifiedBuilding : WorldEvent
{
    public HistoricalFigure? ModifierHf { get; set; }
    public Site? Site { get; set; }
    public int StructureId { get; set; }
    public Structure? Structure { get; set; }
    public string? Modification { get; set; }

    public ModifiedBuilding(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "modifier_hfid": ModifierHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "structure_id": StructureId = Convert.ToInt32(property.Value); break;
                case "modification": Modification = property.Value; break;
            }
        }

        if (Site != null)
        {
            Structure = Site.Structures.Find(structure => structure.LocalId == StructureId);
        }
        ModifierHf.AddEvent(this);
        Site.AddEvent(this);
        Structure.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(ModifierHf?.ToLink(link, pov, this));
        sb.Append(" had a ");
        sb.Append(Modification);
        sb.Append(" added to ");
        sb.Append(Structure?.ToLink(link, pov, this));
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
