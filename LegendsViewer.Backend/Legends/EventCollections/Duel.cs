using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;
using System.Text.Json.Serialization;

namespace LegendsViewer.Backend.Legends.EventCollections;

public class Duel : EventCollection
{
    public int Ordinal { get; set; } = -1;
    public Location? Coordinates;
    [JsonIgnore]
    public HistoricalFigure? Attacker;
    [JsonIgnore]
    public HistoricalFigure? Defender;
    public Duel(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "ordinal": Ordinal = Convert.ToInt32(property.Value); break;
                case "coords": Coordinates = Formatting.ConvertToLocation(property.Value, world); break;
                case "parent_eventcol": ParentCollection = world.GetEventCollection(Convert.ToInt32(property.Value)); break;
                case "attacking_hfid": Attacker = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "defending_hfid": Defender = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
            }
        }
        //foreach (WorldEvent collectionEvent in Collection) this.AddEvent(collectionEvent);
        if (ParentCollection != null && ParentCollection.GetType() == typeof(Battle))
        {
            foreach (HfDied death in Events.OfType<HfDied>())
            {
                if (death.HistoricalFigure == null)
                {
                    continue;
                }
                Battle? battle = ParentCollection as Battle;
                War? parentWar = battle?.ParentCollection as War;
                if (battle != null && battle.NotableAttackers.Contains(death.HistoricalFigure))
                {
                    battle.AttackerDeathCount++;
                    battle.Attackers.Single(squad => squad.Race == death.HistoricalFigure.Race).Deaths++;

                    if (parentWar != null)
                    {
                        parentWar.AttackerDeathCount++;
                    }
                }
                else if (battle != null && battle.NotableDefenders.Contains(death.HistoricalFigure))
                {
                    battle.DefenderDeathCount++;
                    battle.Defenders.Single(squad => squad.Race == death.HistoricalFigure.Race).Deaths++;
                    if (parentWar != null)
                    {
                        parentWar.DefenderDeathCount++;
                    }
                }

                if (parentWar != null)
                {
                    parentWar.DeathCount++;
                }
            }
        }
        Attacker?.AddEventCollection(this);
        Defender?.AddEventCollection(this);

        Name = $"{Formatting.AddOrdinal(Ordinal)} duel";

        Icon = HtmlStyleUtil.GetIconString("fencing");
    }

    public override string ToLink(bool link = true, DwarfObject? pov = null, WorldEvent? worldEvent = null)
    {
        if (link)
        {
            string title = GetTitle();

            string linkedString = pov != this
                ? HtmlStyleUtil.GetAnchorString(Icon, "duel", Id, title, Name)
                : HtmlStyleUtil.GetAnchorCurrentString(Icon, title, HtmlStyleUtil.CurrentDwarfObject(Name));

            if (Attacker != null && pov != Attacker)
            {
                linkedString += $" of {Attacker.ToLink(true, this)}";
            }
            if (Defender != null && pov != Defender)
            {
                linkedString += $" against {Defender.ToLink(true, this)}";
            }

            if (Site != null && pov != Site)
            {
                linkedString += $" in {Site.ToLink(true, this)}";
            }

            if (Region != null && pov != Region)
            {
                linkedString += $" in {Region.ToLink(true, this)}";
            }

            if (UndergroundRegion != null && pov != UndergroundRegion)
            {
                linkedString += $" in {UndergroundRegion.ToLink(true, this)}";
            }
            return linkedString;
        }
        return Name;
    }

    private string GetTitle()
    {
        string title = Type;
        title += "&#13";
        title += "Attacker: ";
        title += Attacker != null ? Attacker.ToLink(false) : "UNKNOWN";
        title += "&#13";
        title += "Defender: ";
        title += Defender != null ? Defender.ToLink(false) : "UNKNOWN";
        title += "&#13";
        title += "Site: ";
        title += Site != null ? Site.ToLink(false) : "UNKNOWN";
        return title;
    }

    public override string ToString()
    {
        return $"the {Name} between {Attacker?.Name} and {Defender?.Name} in {Site}";
    }
}


