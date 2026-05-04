using System.Globalization;
using System.Text;
using LegendsViewer.Backend.Legends.EventCollections;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.WorldObjects;

/// <summary>
/// Handles all formatting, display, and HTML generation for HistoricalFigure.
/// Converts HistoricalFigure data to display-friendly formats (HTML links, text representations, etc.).
/// </summary>
public class HistoricalFigureFormatting
{
    private readonly HistoricalFigure _historicalFigure;
    private string? _cachedShortName;
    private string? _cachedRaceString;
    private string? _cachedTitleRaceString;
    private string? _cachedAnchorTitle;

    public HistoricalFigureFormatting(HistoricalFigure historicalFigure)
    {
        _historicalFigure = historicalFigure;
    }

    /// <summary>
    /// Gets the short name (first name) of the historical figure.
    /// </summary>
    public string GetShortName()
    {
        if (string.IsNullOrEmpty(_cachedShortName))
        {
            _cachedShortName = _historicalFigure.Name.IndexOf(" ", StringComparison.Ordinal) >= 2 && !_historicalFigure.Name.StartsWith("The ")
                ? _historicalFigure.Name.Substring(0, _historicalFigure.Name.IndexOf(" ", StringComparison.Ordinal))
                : _historicalFigure.Name;
        }
        return _cachedShortName;
    }

    /// <summary>
    /// Gets the race string (e.g., "dwarf", "elf", "zombie dwarf", etc.).
    /// </summary>
    public string GetRaceString()
    {
        if (string.IsNullOrEmpty(_cachedRaceString))
        {
            _cachedRaceString = FormatRaceString();
        }
        return _cachedRaceString;
    }

    /// <summary>
    /// Gets the title-cased race string for display purposes.
    /// </summary>
    public string GetTitleRaceString()
    {
        if (string.IsNullOrEmpty(_cachedTitleRaceString))
        {
            _cachedTitleRaceString = GetRaceString();
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            _cachedTitleRaceString = textInfo.ToTitleCase(_cachedTitleRaceString);
        }
        return _cachedTitleRaceString;
    }

    /// <summary>
    /// Gets the anchor/tooltip title for the historical figure.
    /// </summary>
    public string GetAnchorTitle()
    {
        if (string.IsNullOrEmpty(_cachedAnchorTitle))
        {
            _cachedAnchorTitle = GenerateAnchorTitle();
        }
        return _cachedAnchorTitle;
    }

    /// <summary>
    /// Converts the historical figure to an HTML link or text representation.
    /// </summary>
    public string ToLink(bool link = true, DwarfObject? pov = null, Events.WorldEvent? worldEvent = null)
    {
        if (link)
        {
            if (pov == null || pov != _historicalFigure)
            {
                if (pov != null && pov.GetType() == typeof(BeastAttack) && (pov as BeastAttack)?.Beast == _historicalFigure)
                {
                    return $"{HtmlStyleUtil.GetAnchorString(_historicalFigure.Icon, "hf", _historicalFigure.Id, GetAnchorTitle(), GetShortName())}";
                }

                return worldEvent != null
                    ? $"the {GetRaceStringByWorldEvent(worldEvent)} {HtmlStyleUtil.GetAnchorString(_historicalFigure.Icon, "hf", _historicalFigure.Id, GetAnchorTitle(), _historicalFigure.Name)}"
                    : $"the {GetRaceString()} {HtmlStyleUtil.GetAnchorString(_historicalFigure.Icon, "hf", _historicalFigure.Id, GetAnchorTitle(), _historicalFigure.Name)}";
            }
            return $"{HtmlStyleUtil.GetAnchorString("", "hf", _historicalFigure.Id, GetAnchorTitle(), HtmlStyleUtil.CurrentDwarfObject(GetShortName()))}";
        }
        if (pov == null || pov != _historicalFigure)
        {
            return worldEvent != null ? $"{GetRaceStringByWorldEvent(worldEvent)} {_historicalFigure.Name}" : $"{GetRaceString()} {_historicalFigure.Name}";
        }
        return GetShortName();
    }

    /// <summary>
    /// Gets the appropriate icon for the historical figure based on their status and caste.
    /// </summary>
    public string GetIcon()
    {
        if (_historicalFigure.Force)
        {
            return HistoricalFigure.ForceNatureIcon;
        }
        if (_historicalFigure.IsDeity)
        {
            return HistoricalFigure.DeityIcon;
        }
        if (_historicalFigure.Caste == "Female")
        {
            return HistoricalFigure.FemaleIcon;
        }
        if (_historicalFigure.Caste == "Male")
        {
            return HistoricalFigure.MaleIcon;
        }
        return _historicalFigure.Caste == "Default" ? HistoricalFigure.NeuterIcon : "";
    }

    /// <summary>
    /// Gets the highest skill as a formatted string.
    /// </summary>
    public string GetHighestSkillAsString()
    {
        if (_historicalFigure.Skills.Count > 0)
        {
            var highestSkill = _historicalFigure.Skills.OrderBy(skill => skill.Points).Last();
            var highestSkillDescription = SkillDictionary.LookupSkill(highestSkill);
            return $"{highestSkillDescription.Rank} {highestSkillDescription.Name}";
        }
        return string.Empty;
    }

    /// <summary>
    /// Gets the last noble position as a formatted string.
    /// </summary>
    public string GetLastNoblePosition()
    {
        string title = "";
        if (_historicalFigure.Positions?.Count > 0)
        {
            var lastHfposition = _historicalFigure.Positions[^1];
            title += lastHfposition.PrintTitle(false, _historicalFigure);
        }
        return title;
    }

    private string FormatRaceString()
    {
        var race = _historicalFigure.Race;
        if (race == null)
        {
            race = CreatureInfo.Unknown;
            _historicalFigure.Race = race;
        }

        if (_historicalFigure.IsDeity)
        {
            return race.NameSingular.ToLower() + " deity";
        }

        if (_historicalFigure.Force)
        {
            return "force";
        }

        string raceString = "";

        if (_historicalFigure.Ghost)
        {
            raceString += $"{_historicalFigure.GhostType ?? "ghost"} ";
        }
        else if (_historicalFigure.Skeleton)
        {
            raceString += "skeleton ";
        }
        else if (_historicalFigure.Zombie)
        {
            raceString += "zombie ";
        }

        if (!string.IsNullOrWhiteSpace(_historicalFigure.PreviousRace))
        {
            raceString += _historicalFigure.PreviousRace.ToLower() + " turned ";
        }
        else if (!string.IsNullOrWhiteSpace(_historicalFigure.AnimatedType) && !_historicalFigure.Name.Contains("Corpse"))
        {
            raceString += _historicalFigure.AnimatedType.ToLower();
        }
        else
        {
            raceString += race.NameSingular.ToLower();
        }

        foreach (var creatureType in _historicalFigure.CreatureTypes)
        {
            raceString += " " + creatureType.Type;
        }

        return raceString;
    }

    private string GetRaceStringByWorldEvent(Events.WorldEvent worldEvent)
    {
        return GetRaceStringForTimeStamp(worldEvent.Year, worldEvent.Month, worldEvent.Day);
    }

    private string GetRaceStringForTimeStamp(int year, int month, int day)
    {
        if (_historicalFigure.CreatureTypes.Count == 0 && _historicalFigure.UndeadTypes.Count == 0)
        {
            return GetRaceString();
        }

        string raceString = "";

        foreach (var undeadType in GetRelevantCreatureTypesByTimeStamp(_historicalFigure.UndeadTypes, year, month, day))
        {
            raceString += undeadType.Type + " ";
        }

        if (!string.IsNullOrWhiteSpace(_historicalFigure.PreviousRace))
        {
            raceString += _historicalFigure.PreviousRace.ToLower();
        }
        else if (!string.IsNullOrWhiteSpace(_historicalFigure.AnimatedType))
        {
            raceString += _historicalFigure.AnimatedType.ToLower();
        }
        else
        {
            raceString += _historicalFigure.Race.NameSingular.ToLower();
        }

        foreach (var creatureType in GetRelevantCreatureTypesByTimeStamp(_historicalFigure.CreatureTypes, year, month, day))
        {
            raceString += " " + creatureType.Type;
        }

        return raceString;
    }

    private List<HistoricalFigure.CreatureType> GetRelevantCreatureTypesByTimeStamp(List<HistoricalFigure.CreatureType> creatureTypes, int year, int month, int day)
    {
        List<HistoricalFigure.CreatureType> relevantCreatureTypes = [];
        foreach (var creatureType in creatureTypes)
        {
            if (creatureType.StartYear < year)
            {
                relevantCreatureTypes.Add(creatureType);
            }
            else if (creatureType.StartYear == year)
            {
                if (creatureType.StartMonth < month)
                {
                    relevantCreatureTypes.Add(creatureType);
                }
                else if (creatureType.StartMonth == month && creatureType.StartDay < day)
                {
                    relevantCreatureTypes.Add(creatureType);
                }
            }
        }
        return relevantCreatureTypes;
    }

    private string GenerateAnchorTitle()
    {
        var sb = new StringBuilder();

        var lastNoblePosition = GetLastNoblePosition();
        if (!string.IsNullOrWhiteSpace(lastNoblePosition))
        {
            sb.Append(lastNoblePosition);
            sb.Append("&#13");
        }
        else
        {
            var assignmentString = _historicalFigure.GetLastAssignmentString();
            if (!string.IsNullOrWhiteSpace(assignmentString))
            {
                sb.Append(assignmentString);
                sb.Append("&#13");
            }
        }
        if (!string.IsNullOrWhiteSpace(_historicalFigure.AssociatedType) && _historicalFigure.AssociatedType != "Standard")
        {
            sb.Append(_historicalFigure.AssociatedType);
            sb.Append("&#13");
        }
        sb.Append(!string.IsNullOrWhiteSpace(_historicalFigure.Caste) && _historicalFigure.Caste != "Default" ? _historicalFigure.Caste + " " : "");
        sb.Append(Formatting.InitCaps(GetRaceString()));
        if (_historicalFigure.BirthYear != -1)
        {
            sb.Append("&#13");
            sb.Append($"Born: {_historicalFigure.BirthYear}");
        }
        if (!_historicalFigure.IsAlive)
        {
            sb.Append("&#13");
            sb.Append($"Died: {_historicalFigure.DeathYear}");
        }
        if (_historicalFigure.Age > -1)
        {
            sb.Append("&#13");
            sb.Append($"Age: {_historicalFigure.Age} years {(_historicalFigure.IsAlive ? "" : "✝")}");
        }
        sb.Append("&#13");
        sb.Append("Events: ");
        sb.Append(_historicalFigure.Events.Count);
        return sb.ToString();
    }
}
