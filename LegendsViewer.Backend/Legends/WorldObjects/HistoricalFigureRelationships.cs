using LegendsViewer.Backend.Contracts;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldLinks;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.WorldObjects;

/// <summary>
/// Manages all relationships and connections of a HistoricalFigure.
/// Handles relationships with other historical figures, entities, sites, and deities.
/// Generates UI list representations of these relationships.
/// </summary>
public class HistoricalFigureRelationships
{
    private const int MaxListItemCount = 100;

    private readonly HistoricalFigure _historicalFigure;

    public HistoricalFigureRelationships(HistoricalFigure historicalFigure)
    {
        _historicalFigure = historicalFigure;
    }

    /// <summary>
    /// Generates the list of related historical figures (excluding deities).
    /// </summary>
    public List<ListItemDto> GenerateRelatedHistoricalFigureList()
    {
        var list = new List<ListItemDto>();
        foreach (HistoricalFigureLink link in _historicalFigure.RelatedHistoricalFigures.Where(f => f.Type != HistoricalFigureLinkType.Deity))
        {
            if (link.HistoricalFigure == null)
            {
                continue;
            }

            list.Add(new ListItemDto
            {
                Title = $"{link.Type.GetDescription()}",
                Subtitle = $"{link.HistoricalFigure?.ToLink(true, _historicalFigure)}",
            });
        }
        return list;
    }

    /// <summary>
    /// Generates the list of worshipped deities.
    /// </summary>
    public List<ListItemDto> GenerateWorshippedDeities()
    {
        var list = new List<ListItemDto>();
        foreach (HistoricalFigureLink link in _historicalFigure.RelatedHistoricalFigures.Where(f => f.Type == HistoricalFigureLinkType.Deity).OrderByDescending(f => f.Strength))
        {
            if (link.HistoricalFigure == null)
            {
                continue;
            }
            string associatedSpheres = string.Join(", ", link.HistoricalFigure.Spheres);
            list.Add(new ListItemDto
            {
                Title = link.HistoricalFigure.ToLink(true, _historicalFigure),
                Subtitle = associatedSpheres,
                Append = HtmlStyleUtil.GetChipString(link.Strength.ToString())
            });
        }
        return list;
    }

    /// <summary>
    /// Generates the list of figures that worship this figure.
    /// </summary>
    public List<ListItemDto> GenerateWorshippingFiguresList()
    {
        var list = new List<ListItemDto>();
        if (_historicalFigure.WorshippingFigures != null)
        {
            foreach (var (worshipper, strength) in _historicalFigure.WorshippingFigures.OrderByDescending(w => w.Strength).Take(MaxListItemCount))
            {
                list.Add(new ListItemDto
                {
                    Title = worshipper?.ToLink(true, _historicalFigure),
                    Subtitle = worshipper?.GetLastAssignmentString(),
                    Append = HtmlStyleUtil.GetChipString(strength.ToString())
                });
            }
            if (_historicalFigure.WorshippingFigures.Count > MaxListItemCount)
            {
                list.Add(new ListItemDto
                {
                    Subtitle = $"... and {_historicalFigure.WorshippingFigures.Count - MaxListItemCount} more!",
                });
            }
        }

        return list;
    }

    /// <summary>
    /// Generates the list of entities that worship this figure.
    /// </summary>
    public List<ListItemDto> GenerateWorshippingEntitiesList()
    {
        var list = new List<ListItemDto>();
        if (_historicalFigure.WorshippingEntities != null)
        {
            foreach (var worshipper in _historicalFigure.WorshippingEntities)
            {
                list.Add(new ListItemDto
                {
                    Title = worshipper?.ToLink(true, _historicalFigure),
                    Subtitle = worshipper?.EntityType.GetDescription()
                });
            }
        }

        return list;
    }

    /// <summary>
    /// Generates the list of related entities with position information.
    /// </summary>
    public List<ListItemDto> GenerateRelatedEntityList()
    {
        var list = new List<ListItemDto>();
        foreach (EntityLink link in _historicalFigure.RelatedEntities)
        {
            if (link.Entity == null)
            {
                continue;
            }
            string subtitle = $"{link.Type.GetDescription()} of the {link.Entity?.Type.GetDescription()}";
            if (link.PositionId >= 0)
            {
                var assignment = link.Entity?.EntityPositionAssignments.ElementAtOrDefault(link.PositionId);
                if (assignment != null)
                {
                    Various.EntityPosition? position = link.Entity?.EntityPositions.Find(pos => pos.Id == assignment.PositionId);
                    if (position is not null)
                    {
                        string positionTitle = position.GetTitleByCaste(_historicalFigure.Caste);
                        if (link.EndYear > -1)
                        {
                            subtitle = $"Former {positionTitle} of the {link.Entity?.Type.GetDescription()}";
                        }
                        else
                        {
                            subtitle = $"{positionTitle} of the {link.Entity?.Type.GetDescription()}";
                        }
                    }
                }
            }

            list.Add(new ListItemDto
            {
                Title = subtitle,
                Subtitle = $"{link.Entity?.ToLink(true, _historicalFigure)}",
                Append = HtmlStyleUtil.GetChipString(link.Strength.ToString())
            });
        }
        return list;
    }

    /// <summary>
    /// Generates the list of related sites.
    /// </summary>
    public List<ListItemDto> GenerateRelatedSiteList()
    {
        var list = new List<ListItemDto>();
        foreach (SiteLink link in _historicalFigure.RelatedSites)
        {
            if (link.Site == null)
            {
                continue;
            }
            list.Add(new ListItemDto
            {
                Title = $"{link.Type.GetDescription()} ({link.Site?.Type.GetDescription()})",
                Subtitle = $"{link.Site?.ToLink(true, _historicalFigure)}",
            });
        }
        return list;
    }

    /// <summary>
    /// Generates the list of vague relationships.
    /// </summary>
    public List<ListItemDto> GenerateVagueRelationshipList()
    {
        var list = new List<ListItemDto>();
        foreach (VagueRelationship link in _historicalFigure.VagueRelationships)
        {
            var hf = _historicalFigure.World?.GetHistoricalFigure(link.HfId);
            if (hf == null)
            {
                continue;
            }
            list.Add(new ListItemDto
            {
                Title = $"{link.Type.GetDescription()}",
                Subtitle = $"{hf.ToLink(true, _historicalFigure)}",
            });
        }
        return list;
    }

    /// <summary>
    /// Generates the list of notable kills.
    /// </summary>
    public List<ListItemDto> GenerateNotableKillList()
    {
        var list = new List<ListItemDto>();
        foreach (var killEvent in _historicalFigure.NotableKills)
        {
            list.Add(new ListItemDto
            {
                Title = $"{killEvent.HistoricalFigure?.ToLink(true, _historicalFigure)}",
                Subtitle = $"{killEvent.GetDeathString(true, _historicalFigure)}",
            });
        }
        return list;
    }

    /// <summary>
    /// Generates the list of position assignments.
    /// </summary>
    public List<ListItemDto> GeneratePositionList()
    {
        var list = new List<ListItemDto>();
        if (_historicalFigure.Positions?.Count > 1)
        {
            foreach (var position in _historicalFigure.Positions)
            {
                list.Add(new ListItemDto
                {
                    Title = position.PrintTitle(true, _historicalFigure),
                    Subtitle = position.PrintReign(_historicalFigure),
                });
            }
        }
        return list;
    }
}
