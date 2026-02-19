using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class Competition : OccasionEvent
{
    private HistoricalFigure? Winner { get; set; }
    private List<HistoricalFigure> Competitors { get; set; } = [];

    public Competition(List<Property> properties, IWorld world) : base(properties, world)
    {
        OccasionType = OccasionType.Competition;

        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "winner_hfid":
                    Winner = world.GetHistoricalFigure(Convert.ToInt32(property.Value));
                    break;
                case "competitor_hfid":
                    HistoricalFigure? competitor = world.GetHistoricalFigure(Convert.ToInt32(property.Value));
                    if (competitor != null)
                    {
                        Competitors.Add(competitor);
                    }
                    break;
            }
        }

        Winner?.AddEvent(this);
        Competitors.ForEach(competitor =>
        {
            if (competitor != Winner && competitor != null)
            {
                competitor.AddEvent(this);
            }
        });
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        string eventString = base.Print(link, pov);
        if (Competitors.Count > 0)
        {
            eventString += "</br>";
            eventString += "Competing were ";
            for (int i = 0; i < Competitors.Count; i++)
            {
                HistoricalFigure competitor = Competitors.ElementAt(i);
                if (i == 0)
                {
                    eventString += competitor.ToLink(link, pov, this);
                }
                else if (i == Competitors.Count - 1)
                {
                    eventString += " and " + competitor.ToLink(link, pov, this);
                }
                else
                {
                    eventString += ", " + competitor.ToLink(link, pov, this);
                }
            }
            eventString += ". ";
        }
        if (Winner != null)
        {
            eventString += "The winner was ";
            eventString += Winner.ToLink(link, pov, this);
            eventString += ".";
        }
        return eventString;
    }
}

