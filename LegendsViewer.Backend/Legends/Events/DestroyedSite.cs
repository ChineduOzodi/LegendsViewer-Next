using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class DestroyedSite : WorldEvent
{
    public Site? Site { get; set; }
    public Entity? SiteEntity { get; set; }
    public Entity? Attacker { get; set; }
    public Entity? Defender { get; set; }
    public bool NoDefeatMention { get; set; }

    public DestroyedSite(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "site_civ_id": SiteEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "attacker_civ_id": Attacker = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "defender_civ_id": Defender = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "no_defeat_mention":
                    NoDefeatMention = true;
                    property.Known = true;
                    break;
            }
        }

        if (Site?.OwnerHistory.Count == 0)
        {
            if (Defender != null && SiteEntity != null)
            {
                SiteEntity.SetParent(Defender);
            }
            Site.OwnerHistory.Add(new OwnerPeriod(Site, SiteEntity, -1, "founded"));
        }

        if(Site != null)
        {
            Site.OwnerHistory.Last().EndCause = "destroyed";
            Site.OwnerHistory.Last().EndYear = Year;
            Site.OwnerHistory.Last().Ender = Attacker;

            Site?.AddEvent(this);
        }
        if (SiteEntity != Defender)
        {
            SiteEntity?.AddEvent(this);
        }

        Attacker?.AddEvent(this);
        Defender?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Attacker?.ToLink(link, pov, this) ?? "an unknown entity");
        if (!NoDefeatMention)
        {
            sb.Append(" defeated ");
            if (SiteEntity != null && SiteEntity != Defender)
            {
                sb.Append(SiteEntity.ToLink(link, pov, this));
                sb.Append(" of ");
            }

            sb.Append(Defender?.ToLink(link, pov, this) ?? "an unknown entity");
            sb.Append(" and");
        }
        sb.Append(" destroyed ");
        sb.Append(Site?.ToLink(link, pov, this) ?? "an unknown site");
        sb.Append(PrintParentCollection(link, pov));
        sb.Append('.');
        return sb.ToString();
    }
}

