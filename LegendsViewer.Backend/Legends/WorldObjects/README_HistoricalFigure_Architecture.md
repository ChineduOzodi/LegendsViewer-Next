# HistoricalFigure Refactoring Architecture

## Overview

The `HistoricalFigure` class has been refactored from a monolithic 1243-line class into a more maintainable, modular architecture using the **Facade Pattern** and **Service Classes**.

The main class remains as the public interface, while specialized service classes handle specific concerns:

```
HistoricalFigure (Facade)
├── HistoricalFigurePropertyParser  - Handles parsing of World properties
├── HistoricalFigureFormatting      - Handles all display and formatting logic
├── HistoricalFigureRelationships   - Manages UI lists for relationships
└── HistoricalFigureBattleInfo      - Manages battle-related data and statistics
```

## Service Classes

### HistoricalFigurePropertyParser
**Location**: `HistoricalFigurePropertyParser.cs`

**Responsibility**: Parse world properties and populate the HistoricalFigure instance.

**Features**:
- Parses all known property types (birth/death, race, caste, relationships, etc.)
- Handles sub-properties for complex types (entity links, reputation, etc.)
- Performs post-parsing finalization (name defaults, icon generation, player registration)

**Key Methods**:
- `Parse(List<Property>)` - Main entry point for parsing

### HistoricalFigureFormatting
**Location**: `HistoricalFigureFormatting.cs`

**Responsibility**: Handle all formatting, display, and HTML generation.

**Features**:
- Generates display-friendly text (race strings, names, titles)
- Creates HTML links with proper styling and tooltips
- Caches expensive computations (short name, race string, title)
- Handles time-aware race strings for historical accuracy

**Key Methods**:
- `ToLink()` - Generate HTML link representation
- `GetRaceString()` - Get race string with all modifiers
- `GetTitleRaceString()` - Get title-cased race string
- `GetAnchorTitle()` - Generate tooltip with full information
- `GetIcon()` - Get appropriate icon based on status/caste

### HistoricalFigureRelationships
**Location**: `HistoricalFigureRelationships.cs`

**Responsibility**: Manage all relationships and generate UI list representations.

**Features**:
- Filters and sorts relationships (historical figures, entities, sites, etc.)
- Generates `ListItemDto` objects for UI display
- Handles position information and role descriptions
- Limits results to avoid overwhelming UI (max 100 items with "and X more" fallback)

**Key Methods**:
- `GenerateRelatedHistoricalFigureList()` - Non-deity relationships
- `GenerateWorshippedDeities()` - Deities this figure worships
- `GenerateWorshippingFiguresList()` - Figures that worship this figure
- `GenerateRelatedEntityList()` - Entity associations with positions
- `GenerateRelatedSiteList()` - Site relationships
- `GenerateNotableKillList()` - Notable kills
- `GeneratePositionList()` - Historical positions held

### HistoricalFigureBattleInfo
**Location**: `HistoricalFigureBattleInfo.cs`

**Responsibility**: Manage battle-related data and statistics.

**Features**:
- Filters battles by role (attacking, defending, non-combatant)
- Provides statistics and counts
- Calculates percentages for analysis

**Key Methods**:
- `GetAllBattles()` - All battles participated in
- `GetBattlesAttacking()` - Battles where attacking
- `GetBattlesDefending()` - Battles where defending
- `GetBattlesNonCombatant()` - Battles where non-combatant
- `GetBattleAttackingPercentage()` - Percentage of battles attacking

## Benefits

1. **Single Responsibility Principle (SRP)**: Each class has one clear purpose
2. **Maintainability**: Easier to find and modify specific functionality
3. **Testability**: Service classes can be tested independently
4. **Reusability**: Service classes can be extended or reused in other contexts
5. **Readability**: Reduced main class size (1243 → ~400 lines)
6. **Encapsulation**: Related functionality is grouped together

## Usage

The public API remains unchanged. All service classes are lazily initialized as private properties:

```csharp
var hf = new HistoricalFigure(properties, world);

// All existing code continues to work unchanged
string link = hf.ToLink();
var deities = hf.WorshippedDeities;
var battles = hf.BattleLinks;
```

## Migration Notes

- All public properties and methods remain unchanged
- JSON serialization behavior is preserved
- No breaking changes to the public API
- Service classes are internal to the HistoricalFigure logic
- Database/serialization formats are unaffected

## Future Improvements

Potential enhancements to this architecture:

1. **Dependency Injection**: Make service classes injectable for better testability
2. **Caching Layer**: Add explicit caching strategy for frequently accessed data
3. **Batch Operations**: Add methods for bulk relationship operations
4. **Performance Optimization**: Profile and optimize hot paths in service classes
5. **Extended Statistics**: Add more analytical methods to BattleInfo
6. **Relationship Validation**: Add consistency checking in Relationships class
