using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;
using System.Text.Json.Serialization;

namespace LegendsViewer.Backend.Legends.WorldLinks;

public class HistoricalFigureLink
{
    [JsonIgnore]
    public HistoricalFigure? HistoricalFigure { get; set; }
    public string? HfLink => HistoricalFigure?.ToLink(true);

    public HistoricalFigureLinkType Type { get; set; }
    public int Strength { get; set; }

    public HistoricalFigureLink(List<Property> properties, IWorld world)
    {
        Strength = 0;
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "hfid": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "link_strength": Strength = Convert.ToInt32(property.Value); break;
                case "link_type":
                    HistoricalFigureLinkType linkType;
                    if (Enum.TryParse(Formatting.InitCaps(property.Value).Replace(" ", ""), out linkType))
                    {
                        Type = linkType;
                    }
                    else
                    {
                        Type = HistoricalFigureLinkType.Unknown;
                        world.ParsingErrors.Report("Unknown HF HF Link Type: " + property.Value);
                    }
                    break;
            }
        }

        //XML states that deity links, that should be 100, are 0.
        if (Type == HistoricalFigureLinkType.Deity && Strength == 0)
        {
            Strength = 100;
        }
    }

    public HistoricalFigureLink(HistoricalFigure historicalFigureTarget, HistoricalFigureLinkType type, int strength = 0)
    {
        HistoricalFigure = historicalFigureTarget;
        Type = type;
        Strength = strength;
    }
}

