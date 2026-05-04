using LegendsViewer.Backend.Legends.EventCollections;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using System.Drawing;
using System.Text;

namespace LegendsViewer.Backend.Legends.Interfaces;

public interface IWorld
{
    string Name { get; set; }
    string AlternativeName { get; set; }

    int Width { get; set; }
    int Height { get; set; }
    int CurrentYear { get; set; }
    int CurrentMonth { get; set; }
    int CurrentDay { get; set; }

    List<WorldRegion> Regions { get; }
    List<UndergroundRegion> UndergroundRegions { get; }
    List<Landmass> Landmasses { get; }
    List<MountainPeak> MountainPeaks { get; }
    List<Identity> Identities { get; }
    List<River> Rivers { get; }
    List<Site> Sites { get; }
    List<HistoricalFigure> HistoricalFigures { get; }
    List<Entity> Entities { get; }
    List<War> Wars { get; }
    List<Battle> Battles { get; }
    List<BeastAttack> BeastAttacks { get; }
    List<Era> Eras { get; }
    List<Artifact> Artifacts { get; }
    List<WorldConstruction> WorldConstructions { get; }
    List<PoeticForm> PoeticForms { get; }
    List<MusicalForm> MusicalForms { get; }
    List<DanceForm> DanceForms { get; }
    List<WrittenContent> WrittenContents { get; }
    List<Structure> Structures { get; }
    List<WorldEvent> Events { get; }
    List<EventCollection> EventCollections { get; }
    List<EntityPopulation> EntityPopulations { get; }
    List<Population> SitePopulations { get; }
    List<Population> CivilizedPopulations { get; }
    List<Population> OutdoorPopulations { get; }
    List<Population> UndergroundPopulations { get; }
    List<Duel> Duels { get; }
    List<Insurrection> Insurrections { get; }
    List<Persecution> Persecutions { get; }
    List<Purge> Purges { get; }
    List<Raid> Raids { get; }
    List<SiteConquered> SiteConquerings { get; }
    List<EntityOverthrownCollection> Coups { get; }
    List<Abduction> Abductions { get; }
    List<Theft> Thefts { get; }
    List<ProcessionCollection> Processions { get; }
    List<PerformanceCollection> Performances { get; }
    List<Journey> Journeys { get; }
    List<CompetitionCollection> Competitions { get; }
    List<CeremonyCollection> Ceremonies { get; }
    List<Occasion> Occasions { get; }
    StringBuilder Log { get; }
    ParsingErrors ParsingErrors { get; }
    List<WorldObject> PlayerRelatedObjects { get; }
    Dictionary<CreatureInfo, Color> MainRaces { get; }
    List<CreatureInfo> CreatureInfos { get; }
    Dictionary<int, WorldEvent> SpecialEventsById { get; }
    Dictionary<Location, WorldRegion> WorldGrid { get; }
    Dictionary<string, List<HistoricalFigure>> Breeds { get; }

    Task ParseAsync(string xmlFile, string? xmlPlusFile, string? historyFile, string? sitesAndPopulationsFile, string? mapFile);
    CreatureInfo GetCreatureInfo(string id);
    void AddCreatureInfo(CreatureInfo creatureInfo);
    void AddPlayerRelatedDwarfObjects(WorldObject dwarfObject);

    void AddHFtoHfLink(HistoricalFigure hf, Property link);
    void AddHFtoEntityLink(HistoricalFigure hf, Property link);
    void AddHFtoSiteLink(HistoricalFigure hf, Property link);
    void AddEntityEntityLink(Entity entity, Property property);
    void AddReputation(HistoricalFigure hf, Property link);

    void ProcessHFtoHfLinks();
    void ProcessHFtoEntityLinks();
    void ProcessHFtoSiteLinks();
    void ProcessEntityEntityLinks();
    void ProcessReputations();

    Artifact? GetArtifact(int id);
    DanceForm? GetDanceForm(int id);
    Entity? GetEntity(int id);
    EntityPopulation? GetEntityPopulation(int id);
    Era? GetEra(int id);
    WorldEvent? GetEvent(int id);
    EventCollection? GetEventCollection(int id);
    HistoricalFigure? GetHistoricalFigure(int id);
    MusicalForm? GetMusicalForm(int id);
    PoeticForm? GetPoeticForm(int id);
    WorldRegion? GetRegion(int id);
    Site? GetSite(int id);
    Structure? GetStructure(int id);
    UndergroundRegion? GetUndergroundRegion(int id);
    WorldConstruction? GetWorldConstruction(int id);
    WrittenContent? GetWrittenContent(int id);
    Landmass? GetLandmass(int id);
    River? GetRiver(int id);
    MountainPeak? GetMountainPeak(int id);

    void Clear();
    T? GetEventCollection<T>(int id) where T : EventCollection;
}