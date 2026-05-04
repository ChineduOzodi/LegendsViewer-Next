using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class CreatedWorldConstruction : WorldEvent
{
    public Entity? Civ { get; set; }
    public Entity? SiteEntity { get; set; }
    public Site? Site1 { get; set; }
    public Site? Site2 { get; set; }
    public WorldConstruction? WorldConstruction { get; set; }
    public WorldConstruction? MasterWorldConstruction { get; set; }
    public CreatedWorldConstruction(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "civ_id": Civ = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_civ_id": SiteEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_id1": Site1 = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "site_id2": Site2 = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "wcid": WorldConstruction = world.GetWorldConstruction(Convert.ToInt32(property.Value)); break;
                case "master_wcid": MasterWorldConstruction = world.GetWorldConstruction(Convert.ToInt32(property.Value)); break;
            }
        }

        Civ?.AddEvent(this);
        SiteEntity?.AddEvent(this);

        WorldConstruction?.AddEvent(this);
        MasterWorldConstruction?.AddEvent(this);

        Site1?.AddEvent(this);
        Site2?.AddEvent(this);

        if (Site2 != null)
        {
            Site1?.AddConnection(Site2);
        }
        if (Site1 != null)
        {
            Site2?.AddConnection(Site1);
        }

        if (WorldConstruction != null)
        {
            WorldConstruction.Site1 = Site1;
            WorldConstruction.Site2 = Site2;
            if (MasterWorldConstruction != null)
            {
                MasterWorldConstruction.Sections.Add(WorldConstruction);
                WorldConstruction.MasterConstruction = MasterWorldConstruction;
            }
        }
    }
    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(SiteEntity != null ? SiteEntity.ToLink(link, pov, this) : "UNKNOWN ENTITY");
        sb.Append(" of ");
        sb.Append(Civ != null ? Civ.ToLink(link, pov, this) : "UNKNOWN CIV");
        sb.Append(" constructed ");
        sb.Append(WorldConstruction != null ? WorldConstruction.ToLink(link, pov, this) : "UNKNOWN CONSTRUCTION");
        if (MasterWorldConstruction != null)
        {
            sb.Append(" as part of ");
            sb.Append(MasterWorldConstruction.ToLink(link, pov, this));
        }
        sb.Append(" connecting ");
        sb.Append(Site1 != null ? Site1.ToLink(link, pov, this) : "UNKNOWN SITE");
        sb.Append(" and ");
        sb.Append(Site2 != null ? Site2.ToLink(link, pov, this) : "UNKNOWN SITE");
        sb.Append('.');
        return sb.ToString();
    }
}

