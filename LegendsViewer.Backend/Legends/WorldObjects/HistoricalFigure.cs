using System.Text.Json.Serialization;
using LegendsViewer.Backend.Contracts;
using LegendsViewer.Backend.Extensions;
using LegendsViewer.Backend.Legends.Cytoscape;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.EventCollections;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldLinks;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.WorldObjects;

/// <summary>
/// Represents a historical figure in the world.
/// This class acts as a facade, delegating specialized concerns to dedicated service classes:
/// - HistoricalFigurePropertyParser: Handles parsing of World properties
/// - HistoricalFigureFormatting: Handles all display and formatting logic
/// - HistoricalFigureRelationships: Generates UI lists for relationships
/// - HistoricalFigureBattleInfo: Manages battle-related data and statistics
/// </summary>
public class HistoricalFigure : WorldObject
{
    public static readonly string ForceNatureIcon = HtmlStyleUtil.GetIconString("leaf");
    public static readonly string DeityIcon = HtmlStyleUtil.GetIconString("weather-sunset");
    public static readonly string NeuterIcon = HtmlStyleUtil.GetIconString("gender-non-binary");
    public static readonly string FemaleIcon = HtmlStyleUtil.GetIconString("gender-female");
    public static readonly string MaleIcon = HtmlStyleUtil.GetIconString("gender-male");

    // Lazy-initialized service instances
    private HistoricalFigureFormatting? _formatting;
    private HistoricalFigureRelationships? _relationships;
    private HistoricalFigureBattleInfo? _battleInfo;

    private HistoricalFigureFormatting Formatting => _formatting ??= new HistoricalFigureFormatting(this);
    private HistoricalFigureRelationships Relationships => _relationships ??= new HistoricalFigureRelationships(this);
    private HistoricalFigureBattleInfo BattleInfo => _battleInfo ??= new HistoricalFigureBattleInfo(this);

    public override string Type { get => LegendsViewer.Backend.Utilities.Formatting.InitCaps(Formatting.GetRaceString()); set => base.Type = value; }

    private string ShortName => Formatting.GetShortName();

    private string RaceString => Formatting.GetRaceString();

    public string TitleRaceString => Formatting.GetTitleRaceString();

    private string Title => Formatting.GetAnchorTitle();

    public CreatureInfo Race { get; set; } = CreatureInfo.Unknown;
    public string Caste { get; set; } = string.Empty;
    public string AssociatedType { get; set; } = string.Empty;
    public string PreviousRace { get; set; } = string.Empty;

    [JsonIgnore]
    public int EntityPopulationId { get; set; }

    [JsonIgnore]
    public EntityPopulation? EntityPopulation { get; set; }
    public HfState CurrentState { get; set; } = HfState.None;

    [JsonIgnore]
    public List<int> UsedIdentityIds { get; set; } = [];
    [JsonIgnore]
    public int CurrentIdentityId { get; set; }

    [JsonIgnore]
    public List<Artifact> HoldingArtifacts { get; set; } = [];
    public List<string> HoldingArtifactLinks => HoldingArtifacts.ConvertAll(x => x.ToLink(true, this));

    public List<State> States { get; set; } = [];

    public List<CreatureType> CreatureTypes { get; set; } = [];
    public List<CreatureType> UndeadTypes { get; set; } = [];

    [JsonIgnore]
    public List<HistoricalFigureLink> RelatedHistoricalFigures { get; set; } = [];
    public List<ListItemDto> RelatedHistoricalFigureList => Relationships.GenerateRelatedHistoricalFigureList();

    public List<ListItemDto> WorshippedDeities => Relationships.GenerateWorshippedDeities();

    [JsonIgnore]
    public List<(HistoricalFigure Worshipper, int Strength)>? WorshippingFigures { get; set; }
    public List<ListItemDto> WorshippingFiguresList => Relationships.GenerateWorshippingFiguresList();

    [JsonIgnore]
    public List<Entity>? WorshippingEntities { get; set; }
    public List<ListItemDto> WorshippingEntitiesList => Relationships.GenerateWorshippingEntitiesList();

    public List<SiteProperty> SiteProperties { get; set; } = [];
    public List<EntityReputation> Reputations { get; set; } = [];
    public List<RelationshipProfileHf> RelationshipProfiles { get; set; } = [];

    [JsonIgnore]
    public Dictionary<int, RelationshipProfileHf> RelationshipProfilesOfIdentities { get; set; } = []; // TODO not used in Legends Mode

    [JsonIgnore]
    public List<EntityLink> RelatedEntities { get; set; } = [];
    public List<ListItemDto> RelatedEntityList => Relationships.GenerateRelatedEntityList();

    [JsonIgnore]
    public List<SiteLink> RelatedSites { get; set; } = [];
    public List<ListItemDto> RelatedSiteList => Relationships.GenerateRelatedSiteList();

    [JsonIgnore]
    // Forces can have related regions
    public List<WorldRegion> RelatedRegions { get; set; } = [];

    [JsonIgnore]
    public List<Skill> Skills { get; set; } = [];
    public List<SkillDescription> SkillDescriptions => [.. Skills.Select(SkillDictionary.LookupSkill).OrderByDescending(d => d.Points)];

    public List<VagueRelationship> VagueRelationships { get; set; } = [];
    public List<ListItemDto> VagueRelationshipList => Relationships.GenerateVagueRelationshipList();

    [JsonIgnore]
    public List<Structure> DedicatedStructures { get; set; } = [];
    public List<string> DedicatedStructuresLinks => DedicatedStructures.ConvertAll(x => x.ToLink(true, this));

    public int Age { get; set; } = -1;
    public int Appeared { get; set; } = -1;
    public int BirthYear { get; set; } = -1;
    public int BirthSeconds72 { get; set; } = -1;
    public int DeathYear { get; set; } = -1;
    public int DeathSeconds72 { get; set; } = -1;

    [JsonIgnore]
    public HfDied? DeathEvent { get; set; }
    public DeathCause DeathCause { get; set; } = DeathCause.None;
    public List<string> ActiveInteractions { get; set; } = [];
    public List<string> InteractionKnowledge { get; set; } = [];
    public string Goal { get; set; } = string.Empty;
    public string Interaction { get; set; } = string.Empty;

    public List<ListItemDto> MiscList
    {
        get
        {
            var list = new List<ListItemDto>();
            if (!string.IsNullOrEmpty(Goal))
            {
                list.Add(new ListItemDto
                {
                    Title = "Goal",
                    Subtitle = Goal
                });
            }

            if (Positions?.Count > 0)
            {
                var lastPosition = Positions[^1];
                list.Add(new ListItemDto
                {
                    Title = lastPosition.PrintTitle(true, this),
                    Subtitle = lastPosition.PrintReign(this),
                });
            }

            if (Age > -1)
            {
                list.Add(new ListItemDto
                {
                    Title = "Age",
                    Subtitle = Age.ToString() + (IsAlive ? "" : " ✝")
                });
            }
            if (BirthYear != -1)
            {
                list.Add(new ListItemDto
                {
                    Title = "Born",
                    Subtitle = LegendsViewer.Backend.Utilities.Formatting.YearPlusSeconds72ToProsa(BirthYear, BirthSeconds72)
                });
            }
            if (DeathYear > -1)
            {
                list.Add(new ListItemDto
                {
                    Title = "Death",
                    Subtitle = $"{LegendsViewer.Backend.Utilities.Formatting.YearPlusSeconds72ToProsa(DeathYear, DeathSeconds72)} {(DeathEvent != null ? DeathEvent.GetDeathString(true, this) : "")}"
                });
            }
            if (Spheres.Count > 0)
            {
                list.Add(new ListItemDto
                {
                    Title = "Spheres",
                    Subtitle = string.Join(", ", Spheres)
                });
            }
            if (RelatedRegions.Count > 0)
            {
                list.Add(new ListItemDto
                {
                    Title = "Related Regions",
                    Subtitle = string.Join(", ", RelatedRegions.ConvertAll(region => region.ToLink()))
                });
            }
            if (WorshippingFigures != null)
            {
                list.Add(new ListItemDto
                {
                    Title = "Worshipped By",
                    Subtitle = $"{WorshippingFigures.Count} historical figures"
                });
            }
            if (LineageCurseParent != null)
            {
                string curse = "Curse";
                if (!string.IsNullOrWhiteSpace(Interaction))
                {
                    curse = LegendsViewer.Backend.Utilities.Formatting.InitCaps(Interaction);
                }
                list.Add(new ListItemDto
                {
                    Title = $"Lineage {curse} Parent",
                    Subtitle = LineageCurseParent.ToLink(true, this)
                });
            }
            return list;
        }
    }

    [JsonIgnore]
    public HistoricalFigure? LineageCurseParent { get; set; }

    [JsonIgnore]
    public List<HistoricalFigure> LineageCurseChilds { get; set; } = [];
    public List<string> LineageCurseChildLinks => LineageCurseChilds.ConvertAll(x => x.ToLink(true, this));

    public List<string> Spheres { get; set; } = [];

    public List<ListItemDto> JourneyPets { get; set; } = [];

    [JsonIgnore]
    public List<HfDied> NotableKills { get; set; } = [];
    public List<ListItemDto> NotableKillList => Relationships.GenerateNotableKillList();

    [JsonIgnore]
    public List<HistoricalFigure> SnatchedHfs => Events
            .OfType<HfAbducted>()
            .Where(abduction => abduction.Snatcher == this && abduction.Target != null)
            .Select(abduction => abduction.Target!)
            .ToList();
    public List<string> SnatchedHfLinks => SnatchedHfs.ConvertAll(x => x.ToLink(true, this));

    [JsonIgnore]
    public List<Battle> Battles { get; set; } = [];
    public List<string> BattleLinks => BattleInfo.GetAllBattles().ConvertAll(x => x.ToLink(true, this));

    [JsonIgnore]
    public List<Battle> BattlesAttacking => BattleInfo.GetBattlesAttacking();
    public List<string> BattlesAttackingLinks => BattlesAttacking.ConvertAll(x => x.ToLink(true, this));

    [JsonIgnore]
    public List<Battle> BattlesDefending => BattleInfo.GetBattlesDefending();
    public List<string> BattlesDefendingLinks => BattlesDefending.ConvertAll(x => x.ToLink(true, this));

    [JsonIgnore]
    public List<Battle> BattlesNonCombatant => BattleInfo.GetBattlesNonCombatant();
    public List<string> BattlesNonCombatantLinks => BattlesNonCombatant.ConvertAll(x => x.ToLink(true, this));

    [JsonIgnore]
    public List<HfPosition>? Positions { get; set; }
    public List<ListItemDto> PositionList => Relationships.GeneratePositionList();

    [JsonIgnore]
    public Entity? WorshippedBy { get; set; }

    [JsonIgnore]
    public List<BeastAttack> BeastAttacks { get; set; } = [];
    public List<string> BeastAttackLinks => BeastAttacks.ConvertAll(x => x.ToLink(true, this));

    [JsonIgnore]
    public HonorEntity? HonorEntity { get; set; }
    [JsonIgnore]
    public List<IntrigueActor> IntrigueActors { get; set; } = [];
    [JsonIgnore]
    public List<IntriguePlot> IntriguePlots { get; set; } = [];
    [JsonIgnore]
    public List<Identity> Identities { get; set; } = [];

    private CytoscapeData? _familyTreeData;
    public CytoscapeData? FamilyTreeData
    {
        get
        {
            if (_familyTreeData == null &&
            RelatedHistoricalFigures.Any(rel => rel.Type == HistoricalFigureLinkType.Mother ||
                                                rel.Type == HistoricalFigureLinkType.Father ||
                                                rel.Type == HistoricalFigureLinkType.Child))
            {
                _familyTreeData = this.CreateFamilyTreeElements();
            }
            return _familyTreeData;
        }

        set => _familyTreeData = value;
    }

    public bool IsAlive
    {
        get => DeathYear == -1;
        set { }
    }

    public bool IsDeity { get; set; }
    public bool Skeleton { get; set; }
    public bool Force { get; set; }
    public bool Zombie { get; set; }
    public bool Ghost { get; set; }
    public string GhostType { get; set; } = string.Empty;
    public bool Animated { get; set; }
    public string AnimatedType { get; set; } = string.Empty;
    public bool Adventurer { get; set; }
    public string? BreedId { get; set; }
    public bool IsMainCivLeader { get; internal set; }

    public HistoricalFigure()
    {
        Name = "an unknown creature";
    }

    public override string ToString()
    {
        return Name;
    }

    public HistoricalFigure(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        var parser = new HistoricalFigurePropertyParser(this, world);
        parser.Parse(properties);
    }

    public override string ToLink(bool link = true, DwarfObject? pov = null, WorldEvent? worldEvent = null)
    {
        return Formatting.ToLink(link, pov, worldEvent);
    }

    public override string GetIcon()
    {
        return Formatting.GetIcon();
    }
    private List<HfJob>? _jobs;

    public void StartPositionAssignment(Entity? entity, int? startYear, int positionId, string title)
    {
        if (Positions == null)
        {
            Positions = [];
        }
        if (Positions.Exists(p => string.Equals(p.Title, title, StringComparison.OrdinalIgnoreCase) && p.StartYear == startYear && p.Entity == entity))
        {
            return;
        }
        Positions.Add(new HfPosition(entity, startYear, null, positionId, title));
        if (entity != null && entity.IsCiv && positionId == 0)
        {
            IsMainCivLeader = true;
        }
    }

    public void EndPositionAssignment(Entity? entity, int? endYear, int positionId, string title)
    {
        if (Positions == null)
        {
            Positions = [];
        }
        var position = Positions.LastOrDefault(p => p.Entity == entity && p.PositionId == positionId);
        if (position != null)
        {
            position.EndYear = endYear;
        }
        else if(!string.IsNullOrEmpty(title) && title != "-1")
        {
            Positions.Add(new HfPosition(entity, null, endYear, positionId, title));
            if (entity != null && entity.IsCiv && positionId == 0)
            {
                IsMainCivLeader = true;
            }
        }
    }

    public string GetLastAssignmentString()
    {
        var lastAssignmentString = Formatting.GetLastNoblePosition();
        if (!string.IsNullOrEmpty(lastAssignmentString))
        {
            return lastAssignmentString;
        }
        if (_jobs == null)
        {
            _jobs = [];
            foreach (var relevantEvent in Events.OfType<ChangeHfJob>())
            {
                var lastJob = _jobs.LastOrDefault();
                if (!string.Equals(relevantEvent.NewJob, "UNKNOWN JOB", StringComparison.OrdinalIgnoreCase))
                {
                    if (lastJob != null)
                    {
                        lastJob.EndYear = relevantEvent.Year;
                    }
                    _jobs.Add(new HfJob(relevantEvent.Site, relevantEvent.Year, null, LegendsViewer.Backend.Utilities.Formatting.InitCaps(relevantEvent.NewJob)));
                }
                else if (lastJob != null && relevantEvent.OldJob == lastJob.Title)
                {
                    lastJob.EndYear = relevantEvent.Year;
                }
            }
        }
        HfJob? lastAssignment = _jobs?.LastOrDefault();
        if (lastAssignment != null)
        {
            if (lastAssignment.EndYear != null)
            {
                return $"Former {lastAssignment.Title}";
            }
            return lastAssignment.Title;
        }
        return AssociatedType ?? string.Empty;
    }

    public string GetHighestSkillAsString()
    {
        return Formatting.GetHighestSkillAsString();
    }

    public string GetLastNoblePosition()
    {
        string title = "";
        if (Positions?.Count > 0)
        {
            var lastHfposition = Positions[^1];
            title += lastHfposition.PrintTitle(false, this);
        }
        return title;
    }

    public class State
    {
        public HfState HfState { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }

        public State(HfState state, int start)
        {
            HfState = state;
            StartYear = start;
            EndYear = -1;
        }
    }

    public class CreatureType
    {
        public string Type { get; set; }
        public int StartYear { get; set; }
        public int StartMonth { get; set; }
        public int StartDay { get; set; }

        public CreatureType(string type, int startYear, int startMonth, int startDay)
        {
            Type = type;
            StartYear = startYear;
            StartMonth = startMonth;
            StartDay = startDay;
        }

        public CreatureType(string type, WorldEvent worldEvent) : this(type, worldEvent.Year, worldEvent.Month, worldEvent.Day)
        {
        }
    }

    public string CasteNoun(bool owner = false)
    {
        if (string.Equals(Caste, "male", StringComparison.OrdinalIgnoreCase))
        {
            return owner ? "his" : "he";
        }

        if (string.Equals(Caste, "female", StringComparison.OrdinalIgnoreCase))
        {
            return owner ? "her" : "she";
        }

        return owner ? "its" : "it";
    }

    public string GetRaceString()
    {
        if (Race == null)
        {
            Race = CreatureInfo.Unknown;
        }
        if (IsDeity)
        {
            return Race.NameSingular.ToLower() + " deity";
        }

        if (Force)
        {
            return "force";
        }

        string raceString = "";

        if (Ghost)
        {
            raceString += $"{GhostType ?? "ghost"} ";
        }
        else if (Skeleton)
        {
            raceString += "skeleton ";
        }
        else if (Zombie)
        {
            raceString += "zombie ";
        }

        if (!string.IsNullOrWhiteSpace(PreviousRace))
        {
            raceString += PreviousRace.ToLower() + " turned ";
        }
        else if (!string.IsNullOrWhiteSpace(AnimatedType) && !Name.Contains("Corpse"))
        {
            raceString += AnimatedType.ToLower();
        }
        else
        {
            raceString += Race.NameSingular.ToLower();
        }

        foreach (var creatureType in CreatureTypes)
        {
            raceString += " " + creatureType.Type;
        }

        return raceString;
    }

    private string GetRaceStringByWorldEvent(WorldEvent worldEvent)
    {
        return GetRaceStringForTimeStamp(worldEvent.Year, worldEvent.Month, worldEvent.Day);
    }

    private string GetRaceStringForTimeStamp(int year, int month, int day)
    {
        if (CreatureTypes.Count == 0 && UndeadTypes.Count == 0)
        {
            return RaceString;
        }

        string raceString = "";

        foreach (var undeadType in GetRelevantCreatureTypesByTimeStamp(UndeadTypes, year, month, day))
        {
            raceString += undeadType.Type + " ";
        }

        if (!string.IsNullOrWhiteSpace(PreviousRace))
        {
            raceString += PreviousRace.ToLower();
        }
        else if (!string.IsNullOrWhiteSpace(AnimatedType))
        {
            raceString += AnimatedType.ToLower();
        }
        else
        {
            raceString += Race.NameSingular.ToLower();
        }

        foreach (var creatureType in GetRelevantCreatureTypesByTimeStamp(CreatureTypes, year, month, day))
        {
            raceString += " " + creatureType.Type;
        }

        return raceString;
    }

    private List<CreatureType> GetRelevantCreatureTypesByTimeStamp(List<CreatureType> creatureTypes, int year, int month, int day)
    {
        List<CreatureType> relevantCreatureTypes = [];
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

    public override bool MatchesFilterCriteria(WorldObjectFilterDto filter)
    {
        if (!base.MatchesFilterCriteria(filter))
        {
            return false;
        }

        foreach (var rule in filter.Filters)
        {
            if (rule.PropertyName.Equals(nameof(IsAlive), StringComparison.InvariantCultureIgnoreCase) &&
                rule.ViolatesBooleanCriteria(IsAlive))
            {
                return false;
            }
            if (rule.PropertyName.Equals(nameof(IsDeity), StringComparison.InvariantCultureIgnoreCase) &&
                rule.ViolatesBooleanCriteria(IsDeity))
            {
                return false;
            }
            if (rule.PropertyName.Equals(nameof(HistoricalFigureExtensions.IsVampire), StringComparison.InvariantCultureIgnoreCase) &&
                rule.ViolatesBooleanCriteria(HistoricalFigureExtensions.IsVampire(this)))
            {
                return false;
            }
            if (rule.PropertyName.Equals(nameof(HistoricalFigureExtensions.IsWerebeast), StringComparison.InvariantCultureIgnoreCase) &&
                rule.ViolatesBooleanCriteria(HistoricalFigureExtensions.IsWerebeast(this)))
            {
                return false;
            }
            if (rule.PropertyName.Equals(nameof(HistoricalFigureExtensions.IsNecromancer), StringComparison.InvariantCultureIgnoreCase) &&
                rule.ViolatesBooleanCriteria(HistoricalFigureExtensions.IsNecromancer(this)))
            {
                return false;
            }
            if (rule.PropertyName.Equals(nameof(Age), StringComparison.InvariantCultureIgnoreCase) && int.TryParse(rule.Value, out int ruleAgeValue) &&
                rule.ViolatesIntegerCriteria(Age, ruleAgeValue))
            {
                return false;
            }
            if (rule.PropertyName.Equals(nameof(BirthYear), StringComparison.InvariantCultureIgnoreCase) && int.TryParse(rule.Value, out int ruleBirthYearValue) &&
                rule.ViolatesIntegerCriteria(BirthYear, ruleBirthYearValue))
            {
                return false;
            }
            if (rule.PropertyName.Equals(nameof(DeathYear), StringComparison.InvariantCultureIgnoreCase) && int.TryParse(rule.Value, out int ruleDeathYearValue) &&
                rule.ViolatesIntegerCriteria(DeathYear, ruleDeathYearValue))
            {
                return false;
            }
        }

        return true;
    }
}

