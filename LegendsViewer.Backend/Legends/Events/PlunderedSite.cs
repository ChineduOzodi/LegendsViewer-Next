using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class PlunderedSite : WorldEvent
{
    public Entity? Attacker { get; set; }
    public Entity? Defender { get; set; }
    public Entity? SiteEntity { get; set; }
    public Site? Site { get; set; }
    public bool Detected { get; set; }
    public bool NoDefeatMention { get; set; }
    public bool WasRaid { get; set; }
    public bool TookLiveStock { get; set; }
    public bool TookItems { get; set; }

    public PlunderedSite(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "attacker_civ_id": Attacker = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "defender_civ_id": Defender = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_civ_id": SiteEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "detected":
                    if (string.IsNullOrEmpty(property.Value))
                    {
                        property.Known = true;
                        Detected = true;
                    }
                    break;
                case "no_defeat_mention":
                    if (string.IsNullOrEmpty(property.Value))
                    {
                        property.Known = true;
                        NoDefeatMention = true;
                    }
                    break;
                case "was_raid":
                    if (string.IsNullOrEmpty(property.Value))
                    {
                        property.Known = true;
                        WasRaid = true;
                    }
                    break;
                case "took_livestock":
                    if (string.IsNullOrEmpty(property.Value))
                    {
                        property.Known = true;
                        TookLiveStock = true;
                    }
                    break;
                case "took_items":
                    if (string.IsNullOrEmpty(property.Value))
                    {
                        property.Known = true;
                        TookItems = true;
                    }
                    break;
            }
        }

        Attacker.AddEvent(this);
        Defender.AddEvent(this);
        if (Defender != SiteEntity)
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
        if (TookLiveStock || TookItems)
        {
            sb.Append(" stole ");
            if (TookLiveStock)
            {
                sb.Append("livestock ");
            }
            else if (TookItems)
            {
                sb.Append("treasure ");
            }

            if (SiteEntity != null || Defender != null)
            {
                sb.Append("from ");
            }
            if (SiteEntity != null)
            {
                sb.Append(SiteEntity.ToLink(link, pov, this));
                if (Defender != SiteEntity && Defender != null)
                {
                    sb.Append(" of ");
                }
            }
            if (Defender != null)
            {
                sb.Append(Defender.ToLink(link, pov, this));
            }
            sb.Append(" in ");
            sb.Append(Site?.ToLink(link, pov, this));
        }
        else
        {
            sb.Append(" defeated ");
            if (SiteEntity != null && Defender != SiteEntity)
            {
                sb.Append(SiteEntity.ToLink(link, pov, this));
                if (Defender != SiteEntity && Defender != null)
                {
                    sb.Append(" of ");
                }
            }
            if (Defender != null)
            {
                sb.Append(Defender.ToLink(link, pov, this));
            }
            sb.Append(" and pillaged ");
            sb.Append(Site?.ToLink(link, pov, this));
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}

