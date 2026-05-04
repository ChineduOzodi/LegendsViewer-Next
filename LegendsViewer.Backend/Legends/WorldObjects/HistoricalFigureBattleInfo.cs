using LegendsViewer.Backend.Legends.EventCollections;

namespace LegendsViewer.Backend.Legends.WorldObjects;

/// <summary>
/// Manages all battle-related information for a HistoricalFigure.
/// Tracks participation in battles, their roles, and provides battle-related statistics.
/// </summary>
public class HistoricalFigureBattleInfo
{
    private readonly HistoricalFigure _historicalFigure;

    public HistoricalFigureBattleInfo(HistoricalFigure historicalFigure)
    {
        _historicalFigure = historicalFigure;
    }

    /// <summary>
    /// Gets all battles this figure participated in.
    /// </summary>
    public List<Battle> GetAllBattles()
    {
        return _historicalFigure.Battles;
    }

    /// <summary>
    /// Gets all battles where this figure was attacking.
    /// </summary>
    public List<Battle> GetBattlesAttacking()
    {
        return _historicalFigure.Battles.Where(battle => battle.NotableAttackers.Contains(_historicalFigure)).ToList();
    }

    /// <summary>
    /// Gets all battles where this figure was defending.
    /// </summary>
    public List<Battle> GetBattlesDefending()
    {
        return _historicalFigure.Battles.Where(battle => battle.NotableDefenders.Contains(_historicalFigure)).ToList();
    }

    /// <summary>
    /// Gets all battles where this figure was a non-combatant.
    /// </summary>
    public List<Battle> GetBattlesNonCombatant()
    {
        return _historicalFigure.Battles.Where(battle => battle.NonCombatants.Contains(_historicalFigure)).ToList();
    }

    /// <summary>
    /// Gets the total number of battles this figure participated in.
    /// </summary>
    public int GetBattleCount()
    {
        return _historicalFigure.Battles.Count;
    }

    /// <summary>
    /// Gets the count of battles where this figure was attacking.
    /// </summary>
    public int GetBattleAttackingCount()
    {
        return GetBattlesAttacking().Count;
    }

    /// <summary>
    /// Gets the count of battles where this figure was defending.
    /// </summary>
    public int GetBattleDefendingCount()
    {
        return GetBattlesDefending().Count;
    }

    /// <summary>
    /// Gets the count of battles where this figure was a non-combatant.
    /// </summary>
    public int GetBattleNonCombatantCount()
    {
        return GetBattlesNonCombatant().Count;
    }

    /// <summary>
    /// Gets the percentage of battles where this figure was attacking.
    /// </summary>
    public double GetBattleAttackingPercentage()
    {
        if (GetBattleCount() == 0)
            return 0;

        return (GetBattleAttackingCount() / (double)GetBattleCount()) * 100;
    }

    /// <summary>
    /// Gets the percentage of battles where this figure was defending.
    /// </summary>
    public double GetBattleDefendingPercentage()
    {
        if (GetBattleCount() == 0)
            return 0;

        return (GetBattleDefendingCount() / (double)GetBattleCount()) * 100;
    }
}
