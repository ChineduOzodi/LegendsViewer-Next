using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class SiteTributeForced : WorldEvent
{
    public Entity? Attacker { get; set; }
    public Entity? Defender { get; set; }
    public Entity? SiteEntity { get; set; }
    public Site? Site { get; set; }
    public string? Season { get; set; }

    public SiteTributeForced(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "attacker_civ_id":
                    Attacker = world.GetEntity(Convert.ToInt32(property.Value));
                    break;
                case "defender_civ_id":
                    Defender = world.GetEntity(Convert.ToInt32(property.Value));
                    break;
                case "site_civ_id":
                    SiteEntity = world.GetEntity(Convert.ToInt32(property.Value));
                    break;
                case "site_id":
                    Site = world.GetSite(Convert.ToInt32(property.Value));
                    break;
                case "season":
                    Season = property.Value;
                    break;
            }
        }

        Attacker.AddEvent(this);
        Defender.AddEvent(this);
        if (SiteEntity != Defender)
        {
            SiteEntity.AddEvent(this);
        }
        Site.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Attacker?.ToLink(link, pov, this));
        sb.Append(" secured tribute from ");
        sb.Append(SiteEntity?.ToLink(link, pov, this));
        if (Defender != null)
        {
            sb.Append(" of ");
            sb.Append(Defender?.ToLink(link, pov, this));
        }
        sb.Append(", to be delivered from ");
        sb.Append(Site?.ToLink(link, pov, this));
        if (!string.IsNullOrWhiteSpace(Season))
        {
            sb.Append(" every ");
            sb.Append(Season.Trim());
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}

