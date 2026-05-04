using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class Gamble : WorldEvent
{
    public int StructureId { get; set; }
    public Structure? Structure { get; set; }
    public Site? Site { get; set; }
    public WorldRegion? Region { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }
    public HistoricalFigure? Gambler { get; set; }
    public int OldAccount { get; set; }
    public int NewAccount { get; set; }

    public Gamble(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
                case "old_account": OldAccount = Convert.ToInt32(property.Value); break;
                case "new_account": NewAccount = Convert.ToInt32(property.Value); break;
                case "gambler_hfid": Gambler = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "structure_id": StructureId = Convert.ToInt32(property.Value); break;
            }
        }

        if (Site != null)
        {
            Structure = Site.Structures.Find(structure => structure.LocalId == StructureId);
        }

        Site.AddEvent(this);
        Region.AddEvent(this);
        UndergroundRegion.AddEvent(this);
        Gambler.AddEvent(this);
        Structure.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Gambler?.ToLink(link, pov, this));

        // same ranges like in "trade" event
        var balance = NewAccount - OldAccount;
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
        sb.Append(" gambling");
        if (Site != null)
        {
            sb.Append(" in ");
            sb.Append(Site.ToLink(link, pov, this));
        }
        else if (Region != null)
        {
            sb.Append(" in ");
            sb.Append(Region.ToLink(link, pov, this));
        }
        else if (UndergroundRegion != null)
        {
            sb.Append(" in ");
            sb.Append(UndergroundRegion.ToLink(link, pov, this));
        }
        if (Structure != null)
        {
            sb.Append(" at ");
            sb.Append(Structure.ToLink(link, pov, this));
        }

        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}
