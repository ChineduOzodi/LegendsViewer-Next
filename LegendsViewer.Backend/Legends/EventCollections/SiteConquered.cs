using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;
using System.Text.Json.Serialization;

namespace LegendsViewer.Backend.Legends.EventCollections;

public class SiteConquered : EventCollection
{
    public int Ordinal { get; set; } = -1;
    public SiteConqueredType ConquerType { get; set; }
    [JsonIgnore]
    public Entity? Attacker { get; set; }
    [JsonIgnore]
    public Entity? Defender { get; set; }
    [JsonIgnore]
    public Battle? Battle { get; set; }
    [JsonIgnore]
    public List<HistoricalFigure> Deaths => GetSubEvents().OfType<HfDied>().Where(death => death.HistoricalFigure != null).Select(death => death.HistoricalFigure!).ToList();
    public int DeathCount => Deaths.Count;

    public SiteConquered(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "ordinal": Ordinal = Convert.ToInt32(property.Value); break;
                case "war_eventcol": ParentCollection = world.GetEventCollection(Convert.ToInt32(property.Value)); break;
                case "attacking_enid": Attacker = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "defending_enid": Defender = world.GetEntity(Convert.ToInt32(property.Value)); break;
            }
        }

        if (Events.OfType<PlunderedSite>().Any())
        {
            ConquerType = SiteConqueredType.Pillaging;
        }
        else if (Events.OfType<DestroyedSite>().Any())
        {
            ConquerType = SiteConqueredType.Destruction;
        }
        else if (Events.OfType<NewSiteLeader>().Any() || Events.OfType<SiteTakenOver>().Any())
        {
            ConquerType = SiteConqueredType.Conquest;
        }
        else
        {
            ConquerType = Events.OfType<SiteTributeForced>().Any() ? SiteConqueredType.TributeEnforcement : SiteConqueredType.Invasion;
        }

        if (ConquerType == SiteConqueredType.Pillaging ||
            ConquerType == SiteConqueredType.Invasion ||
            ConquerType == SiteConqueredType.TributeEnforcement)
        {
            Notable = false;
        }
        if (ParentCollection is War)
        {
            War? war = ParentCollection as War;
            if (war != null)
            {
                war.DeathCount += Events.OfType<HfDied>().Count();
            }

            if (Attacker == war?.Attacker)
            {
                war?.AttackerVictories.Add(this);
            }
            else
            {
                war?.DefenderVictories.Add(this);
            }
        }
        Attacker?.AddEventCollection(this);
        Defender?.AddEventCollection(this);

        Name = $"{Formatting.AddOrdinal(Ordinal)} {ConquerType.GetDescription()}";
        Subtype = ConquerType.GetDescription();
        Icon = HtmlStyleUtil.GetIconString("flag-variant");
    }

    public override string ToLink(bool link = true, DwarfObject? pov = null, WorldEvent? worldEvent = null)
    {
        if (link)
        {
            var sb = new StringBuilder();
            string title = GetTitle();
            sb.Append(pov != this
                ? HtmlStyleUtil.GetAnchorString(Icon, "siteconquered", Id, title, Name)
                : HtmlStyleUtil.GetAnchorCurrentString(Icon, title, HtmlStyleUtil.CurrentDwarfObject(Name)));

            if (Site != null && pov != Site)
            {
                sb.Append(" in ");
                sb.Append(Site.ToLink(true, this));
            }
            if (pov != this && pov != Battle)
            {
                sb.Append(" as a result of ");
                sb.Append(Battle?.ToLink());
            }
            return sb.ToString();
        }
        return ToString();
    }

    private string GetTitle()
    {
        var sb = new StringBuilder();
        sb.Append(Type);
        sb.Append("&#13");
        if (Attacker != null)
        {
            sb.Append(Attacker.PrintEntity(false));
            sb.Append(" (Attacker)(V)");
            sb.Append("&#13");
        }
        if (Defender != null)
        {
            sb.Append(Defender.PrintEntity(false));
            sb.Append(" (Defender)");
        }
        sb.Append("&#13");
        sb.Append("Site: ");
        sb.Append(Site != null ? Site.ToLink(false) : "UNKNOWN");
        return sb.ToString();
    }

    public override string ToString()
    {
        return $"the {Name} in {Site}";
    }

    public override string GetIcon()
    {
        return Icon;
    }
}


