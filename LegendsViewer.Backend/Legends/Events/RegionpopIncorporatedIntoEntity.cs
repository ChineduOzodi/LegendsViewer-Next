using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

//dwarf mode eventsList


// new 0.42.XX events


public class RegionpopIncorporatedIntoEntity : WorldEvent
{
    public Site? Site { get; set; }
    public Entity? JoinEntity { get; set; }
    public string? PopRace { get; set; }
    public int PopNumberMoved { get; set; }
    public WorldRegion? PopSourceRegion { get; set; }
    public string? PopFlId { get; set; }

    public RegionpopIncorporatedIntoEntity(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "join_entity_id":
                    JoinEntity = world.GetEntity(Convert.ToInt32(property.Value));
                    break;
                case "site_id":
                    Site = world.GetSite(Convert.ToInt32(property.Value));
                    break;
                case "pop_race":
                    PopRace = property.Value;
                    break;
                case "pop_number_moved":
                    PopNumberMoved = Convert.ToInt32(property.Value);
                    break;
                case "pop_srid":
                    PopSourceRegion = world.GetRegion(Convert.ToInt32(property.Value));
                    break;
                case "pop_flid":
                    PopFlId = property.Value;
                    break;
            }
        }

        Site.AddEvent(this);
        JoinEntity.AddEvent(this);
        PopSourceRegion.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        if (PopNumberMoved > 200)
        {
            sb.Append(" hundreds of ");
        }
        else if (PopNumberMoved > 24)
        {
            sb.Append(" dozens of ");
        }
        else
        {
            sb.Append(" several ");
        }
        sb.Append("UNKNOWN RACE");
        sb.Append(" from ");
        sb.Append(PopSourceRegion != null ? PopSourceRegion.ToLink(link, pov, this) : "UNKNOWN REGION");
        sb.Append(" joined with ");
        sb.Append(JoinEntity != null ? JoinEntity.ToLink(link, pov, this) : "UNKNOWN ENTITY");
        sb.Append(" at ");
        sb.Append(Site != null ? Site.ToLink(link, pov, this) : "UNKNOWN SITE");
        sb.Append(".");
        return sb.ToString();
    }
}


