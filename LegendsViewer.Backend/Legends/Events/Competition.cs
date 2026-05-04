using System.Text;
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
        var sb = new StringBuilder();
        sb.Append(base.Print(link, pov));
        if (Competitors.Count > 0)
        {
            sb.Append("</br>");
            sb.Append("Competing were ");
            for (int i = 0; i < Competitors.Count; i++)
            {
                HistoricalFigure competitor = Competitors.ElementAt(i);
                if (i == 0)
                {
                    sb.Append(competitor.ToLink(link, pov, this));
                }
                else if (i == Competitors.Count - 1)
                {
                    sb.Append(" and ");
                    sb.Append(competitor.ToLink(link, pov, this));
                }
                else
                {
                    sb.Append(", ");
                    sb.Append(competitor.ToLink(link, pov, this));
                }
            }
            sb.Append(". ");
        }
        if (Winner != null)
        {
            sb.Append("The winner was ");
            sb.Append(Winner.ToLink(link, pov, this));
            sb.Append(".");
        }
        return sb.ToString();
    }
}

