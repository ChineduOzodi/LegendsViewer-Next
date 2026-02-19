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
            string title = GetTitle();

            string linkedString = pov != this
                ? HtmlStyleUtil.GetAnchorString(Icon, "siteconquered", Id, title, Name)
                : HtmlStyleUtil.GetAnchorCurrentString(Icon, title, HtmlStyleUtil.CurrentDwarfObject(Name));

            if (Site != null && pov != Site)
            {
                linkedString += $" in {Site.ToLink(true, this)}";
            }
            if (pov != this && pov != Battle)
            {
                linkedString += " as a result of " + Battle?.ToLink();
            }
            return linkedString;
        }
        return ToString();
    }

    private string GetTitle()
    {
        string title = Type;
        title += "&#13";
        if (Attacker != null)
        {
            title += Attacker.PrintEntity(false) + " (Attacker)(V)";
            title += "&#13";
        }
        if (Defender != null)
        {
            title += Defender.PrintEntity(false) + " (Defender)";
        }
        title += "&#13";
        title += "Site: ";
        title += Site != null ? Site.ToLink(false) : "UNKNOWN";
        return title;
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


