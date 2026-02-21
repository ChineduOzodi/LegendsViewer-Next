using System.Text;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.Events;

public class HfDied : WorldEvent, IFeatured
{
    public HistoricalFigure? Slayer { get; set; }
    public HistoricalFigure? HistoricalFigure { get; set; }
    public DeathCause Cause { get; set; }
    private string UnknownCause { get; set; } = string.Empty;
    public Site? Site { get; set; }
    public WorldRegion? Region { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }
    public int SlayerItemId { get; set; }
    public int SlayerShooterItemId { get; set; }
    public string SlayerRace { get; set; } = string.Empty;
    public string SlayerCaste { get; set; } = string.Empty;

    public int ItemId { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public string ItemSubType { get; set; } = string.Empty;
    public string ItemMaterial { get; set; } = string.Empty;
    public Artifact? Artifact { get; set; }

    public int ShooterItemId { get; set; }
    public string ShooterItemType { get; set; } = string.Empty;
    public string ShooterItemSubType { get; set; } = string.Empty;
    public string ShooterItemMaterial { get; set; } = string.Empty;
    public Artifact? ShooterArtifact { get; set; }

    public HfDied(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        ItemId = -1;
        ShooterItemId = -1;
        SlayerItemId = -1;
        SlayerShooterItemId = -1;
        SlayerRace = "UNKNOWN";
        SlayerCaste = "UNKNOWN";
        Cause = DeathCause.Unknown;
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "slayer_item_id": SlayerItemId = Convert.ToInt32(property.Value); break;
                case "slayer_shooter_item_id": SlayerShooterItemId = Convert.ToInt32(property.Value); break;
                case "cause":
                    switch (property.Value)
                    {
                        case "hunger": Cause = DeathCause.Starved; break;
                        case "struck": Cause = DeathCause.Struck; break;
                        case "murdered": Cause = DeathCause.Murdered; break;
                        case "old age": Cause = DeathCause.OldAge; break;
                        case "dragonfire": Cause = DeathCause.DragonsFire; break;
                        case "shot": Cause = DeathCause.Shot; break;
                        case "fire": Cause = DeathCause.Burned; break;
                        case "thirst": Cause = DeathCause.Thirst; break;
                        case "air": Cause = DeathCause.Suffocated; break;
                        case "blood": Cause = DeathCause.Bled; break;
                        case "cold": Cause = DeathCause.Cold; break;
                        case "crushed bridge": Cause = DeathCause.CrushedByABridge; break;
                        case "drown": Cause = DeathCause.Drowned; break;
                        case "infection": Cause = DeathCause.Infection; break;
                        case "obstacle": Cause = DeathCause.CollidedWithAnObstacle; break;
                        case "put to rest": Cause = DeathCause.PutToRest; break;
                        case "quitdead": Cause = DeathCause.StarvedQuit; break;
                        case "trap": Cause = DeathCause.Trap; break;
                        case "crushed": Cause = DeathCause.CaveIn; break;
                        case "cage blasted": Cause = DeathCause.InACage; break;
                        case "freezing water": Cause = DeathCause.FrozenInWater; break;
                        case "exec generic": Cause = DeathCause.ExecutedGeneric; break;
                        case "exec fed to beasts": Cause = DeathCause.ExecutedFedToBeasts; break;
                        case "exec burned alive": Cause = DeathCause.ExecutedBurnedAlive; break;
                        case "exec crucified": Cause = DeathCause.ExecutedCrucified; break;
                        case "exec drowned": Cause = DeathCause.ExecutedDrowned; break;
                        case "exec hacked to pieces": Cause = DeathCause.ExecutedHackedToPieces; break;
                        case "exec buried alive": Cause = DeathCause.ExecutedBuriedAlive; break;
                        case "exec beheaded": Cause = DeathCause.ExecutedBeheaded; break;
                        case "blood drained": Cause = DeathCause.DrainedBlood; break;
                        case "collapsed": Cause = DeathCause.Collapsed; break;
                        case "scared to death": Cause = DeathCause.ScaredToDeath; break;
                        case "scuttled": Cause = DeathCause.Scuttled; break;
                        case "flying object": Cause = DeathCause.FlyingObject; break;
                        case "slaughtered": Cause = DeathCause.Slaughtered; break;
                        case "melt": Cause = DeathCause.Melted; break;
                        case "spikes": Cause = DeathCause.Spikes; break;
                        case "heat": Cause = DeathCause.Heat; break;
                        case "vanish": Cause = DeathCause.Vanish; break;
                        case "cooling magma": Cause = DeathCause.CoolingMagma; break;
                        case "vehicle": Cause = DeathCause.Vehicle; break;
                        case "suicide drowned": Cause = DeathCause.SuicideDrowned; break;
                        case "suicide leaping": Cause = DeathCause.SuicideLeaping; break;
                        case "chasm": Cause = DeathCause.Chasm; break;
                        default: Cause = DeathCause.Unknown; UnknownCause = property.Value; world.ParsingErrors.Report("|==> Events 'hf died'/ \nUnknown Death Cause: " + UnknownCause); break;
                    }
                    break;
                case "slayer_race": SlayerRace = Formatting.FormatRace(property.Value); break;
                case "slayer_caste": SlayerCaste = property.Value; break;
                case "hfid": HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "slayer_hfid": Slayer = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
                case "victim_hf": if (HistoricalFigure == null) { HistoricalFigure = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
                case "slayer_hf": if (Slayer == null) { Slayer = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
                case "site": if (Site == null) { Site = world.GetSite(Convert.ToInt32(property.Value)); } else { property.Known = true; } break;
                case "death_cause": property.Known = true; break;
                case "item": ItemId = Convert.ToInt32(property.Value); break;
                case "item_type": ItemType = property.Value; break;
                case "item_subtype": ItemSubType = property.Value; break;
                case "mat": ItemMaterial = property.Value; break;
                case "artifact_id": Artifact = world.GetArtifact(Convert.ToInt32(property.Value)); break;
                case "shooter_item": ShooterItemId = Convert.ToInt32(property.Value); break;
                case "shooter_item_type": ShooterItemType = property.Value; break;
                case "shooter_item_subtype": ShooterItemSubType = property.Value; break;
                case "shooter_mat": ShooterItemMaterial = property.Value; break;
                case "shooter_artifact_id": ShooterArtifact = world.GetArtifact(Convert.ToInt32(property.Value)); break;
            }
        }
        if (HistoricalFigure != null)
        {
            HistoricalFigure.AddEvent(this);
            if (HistoricalFigure.DeathCause == DeathCause.None)
            {
                HistoricalFigure.DeathEvent = this;
                HistoricalFigure.DeathCause = Cause;
            }
            if (Cause == DeathCause.PutToRest)
            {
                HistoricalFigure.Ghost = false;
            }
            if (HistoricalFigure.DeathYear == -1)
            {
                HistoricalFigure.DeathYear = Year;
                HistoricalFigure.DeathSeconds72 = Seconds72;
            }
            HistoricalFigure.Zombie = false;
        }


        if (Slayer != null)
        {
            if (HistoricalFigure != Slayer)
            {
                Slayer.AddEvent(this);
            }
            Slayer.NotableKills.Add(this);
        }
        Site?.AddEvent(this);
        Region?.AddEvent(this);
        UndergroundRegion?.AddEvent(this);
        Artifact?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(HistoricalFigure?.ToLink(link, pov, this));
        sb.Append(" ");
        sb.Append(GetDeathString(link, pov));
        sb.Append(GetLocationString(link, pov));
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }

    public string PrintFeature(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(HistoricalFigure?.ToLink(link, pov, this));
        sb.Append(" ");
        sb.Append(GetDeathString(link, pov));
        sb.Append(GetLocationString(link, pov));
        sb.Append(" in ");
        sb.Append(Year);
        return sb.ToString();
    }

    public string GetLocationString(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        if (Site != null)
        {
            sb.Append(" in ");
            sb.Append(Site.ToLink(link, pov, this));
        }
        else if (Region != null)
        {
            sb.Append(" in ");
            sb.Append(Region.ToLink(link, pov, this));
        }
        else if (UndergroundRegion != null)
        {
            sb.Append(" in ");
            sb.Append(UndergroundRegion.ToLink(link, pov, this));
        }
        return sb.ToString();
    }

    public string GetDeathString(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();

        if (Slayer != null || SlayerRace != "UNKNOWN" && SlayerRace != "-1")
        {
            string slayerString = Slayer == null ? " a " + SlayerRace.ToLower() : Slayer.ToLink(link, pov, this);
            switch (Cause)
            {
                case DeathCause.DragonsFire:
                    sb.Append("burned up in ");
                    sb.Append(slayerString);
                    sb.Append("'s dragon fire");
                    break;
                case DeathCause.Burned:
                    sb.Append("was burned to death by ");
                    sb.Append(slayerString);
                    sb.Append("'s fire");
                    break;
                case DeathCause.Murdered:
                    sb.Append("was murdered by ");
                    sb.Append(slayerString);
                    break;
                case DeathCause.Shot:
                    sb.Append("was shot and killed by ");
                    sb.Append(slayerString);
                    break;
                case DeathCause.Struck:
                    sb.Append("was struck down by ");
                    sb.Append(slayerString);
                    break;
                case DeathCause.ExecutedGeneric:
                    sb.Append("was executed by ");
                    sb.Append(slayerString);
                    break;
                case DeathCause.ExecutedBuriedAlive:
                    sb.Append("was buried alive by ");
                    sb.Append(slayerString);
                    break;
                case DeathCause.ExecutedBurnedAlive:
                    sb.Append("was burned alive by ");
                    sb.Append(slayerString);
                    break;
                case DeathCause.ExecutedCrucified:
                    sb.Append("was crucified by ");
                    sb.Append(slayerString);
                    break;
                case DeathCause.ExecutedDrowned:
                    sb.Append("was drowned by ");
                    sb.Append(slayerString);
                    break;
                case DeathCause.ExecutedFedToBeasts:
                    sb.Append("was fed to beasts by ");
                    sb.Append(slayerString);
                    break;
                case DeathCause.ExecutedHackedToPieces:
                    sb.Append("was hacked to pieces by ");
                    sb.Append(slayerString);
                    break;
                case DeathCause.ExecutedBeheaded:
                    sb.Append("was beheaded by ");
                    sb.Append(slayerString);
                    break;
                case DeathCause.DrainedBlood:
                    sb.Append("was drained of blood by ");
                    sb.Append(slayerString);
                    break;
                case DeathCause.Collapsed:
                    sb.Append("collapsed, struck down by ");
                    sb.Append(slayerString);
                    break;
                case DeathCause.ScaredToDeath:
                    sb.Append(" was scared to death by ");
                    sb.Append(slayerString);
                    break;
                case DeathCause.Bled:
                    sb.Append(" bled to death, slain by ");
                    sb.Append(slayerString);
                    break;
                case DeathCause.Spikes:
                    sb.Append(" was impaled by ");
                    sb.Append(slayerString);
                    break;
                default:
                    sb.Append(", slain by ");
                    sb.Append(slayerString);
                    break;
            }
        }
        else
        {
            switch (Cause)
            {
                case DeathCause.Thirst:
                    sb.Append("died of thirst");
                    break;
                case DeathCause.OldAge:
                    sb.Append("died of old age");
                    break;
                case DeathCause.Suffocated:
                    sb.Append("suffocated");
                    break;
                case DeathCause.Bled:
                    sb.Append("bled to death");
                    break;
                case DeathCause.Cold:
                    sb.Append("froze to death");
                    break;
                case DeathCause.CrushedByABridge:
                    sb.Append("was crushed by a drawbridge");
                    break;
                case DeathCause.Drowned:
                    sb.Append("drowned");
                    break;
                case DeathCause.Starved:
                    sb.Append("starved to death");
                    break;
                case DeathCause.Infection:
                    sb.Append("succumbed to infection");
                    break;
                case DeathCause.CollidedWithAnObstacle:
                    sb.Append("died after colliding with an obstacle");
                    break;
                case DeathCause.PutToRest:
                    sb.Append("was put to rest");
                    break;
                case DeathCause.StarvedQuit:
                    sb.Append("starved");
                    break;
                case DeathCause.Trap:
                    sb.Append("was killed by a trap");
                    break;
                case DeathCause.CaveIn:
                    sb.Append("was crushed under a collapsing ceiling");
                    break;
                case DeathCause.InACage:
                    sb.Append("died in a cage");
                    break;
                case DeathCause.FrozenInWater:
                    sb.Append("was incased in ice");
                    break;
                case DeathCause.Scuttled:
                    sb.Append("was scuttled");
                    break;
                case DeathCause.Slaughtered:
                    sb.Append("was slaughtered");
                    break;
                case DeathCause.FlyingObject:
                    sb.Append("was killed by a flying object");
                    break;
                case DeathCause.ExecutedGeneric:
                    sb.Append("was executed");
                    break;
                case DeathCause.ExecutedBuriedAlive:
                    sb.Append("was buried alive");
                    break;
                case DeathCause.ExecutedBurnedAlive:
                    sb.Append("was burned alive");
                    break;
                case DeathCause.ExecutedCrucified:
                    sb.Append("was crucified");
                    break;
                case DeathCause.ExecutedDrowned:
                    sb.Append("was drowned");
                    break;
                case DeathCause.ExecutedFedToBeasts:
                    sb.Append("was fed to beasts");
                    break;
                case DeathCause.ExecutedHackedToPieces:
                    sb.Append("was hacked to pieces");
                    break;
                case DeathCause.ExecutedBeheaded:
                    sb.Append("was beheaded");
                    break;
                case DeathCause.Melted:
                    sb.Append("melted");
                    break;
                case DeathCause.Spikes:
                    sb.Append("was impaled");
                    break;
                case DeathCause.Heat:
                    sb.Append("died in the heat");
                    break;
                case DeathCause.Vanish:
                    sb.Append("vanished");
                    break;
                case DeathCause.CoolingMagma:
                    sb.Append("was encased in cooling magma");
                    break;
                case DeathCause.Vehicle:
                    sb.Append("was killed by a vehicle");
                    break;
                case DeathCause.SuicideDrowned:
                    sb.Append("drowned ");
                    switch (HistoricalFigure?.Caste)
                    {
                        case "Female":
                            sb.Append("herself ");
                            break;
                        case "Male":
                            sb.Append("himself ");
                            break;
                        default:
                            sb.Append("itself ");
                            break;
                    }
                    break;
                case DeathCause.SuicideLeaping:
                    sb.Append("leapt from a great height");
                    break;
                case DeathCause.Chasm:
                    sb.Append("fell into a deep chasm");
                    break;
                case DeathCause.Unknown:
                    sb.Append("died (");
                    sb.Append(UnknownCause);
                    sb.Append(")");
                    break;
            }
        }

        if (ItemId >= 0)
        {
            if (Artifact != null)
            {
                sb.Append(" with ");
                sb.Append(Artifact.ToLink(link, pov, this));
            }
            else if (!string.IsNullOrWhiteSpace(ItemType) || !string.IsNullOrWhiteSpace(ItemSubType))
            {
                sb.Append(" with a ");
                sb.Append(!string.IsNullOrWhiteSpace(ItemMaterial) ? ItemMaterial + " " : " ");
                sb.Append(!string.IsNullOrWhiteSpace(ItemSubType) ? ItemSubType : ItemType);
            }
        }
        else if (ShooterItemId >= 0)
        {
            if (ShooterArtifact != null)
            {
                sb.Append(" (shot) with ");
                sb.Append(ShooterArtifact.ToLink(link, pov, this));
            }
            else if (!string.IsNullOrWhiteSpace(ShooterItemType) || !string.IsNullOrWhiteSpace(ShooterItemSubType))
            {
                sb.Append(" (shot) with a ");
                sb.Append(!string.IsNullOrWhiteSpace(ShooterItemMaterial) ? ShooterItemMaterial + " " : " ");
                sb.Append(!string.IsNullOrWhiteSpace(ShooterItemSubType) ? ShooterItemSubType : ShooterItemType);
            }
        }
        else if (SlayerItemId >= 0)
        {
            sb.Append(" with a (");
            sb.Append(SlayerItemId);
            sb.Append(")");
        }
        else if (SlayerShooterItemId >= 0)
        {
            sb.Append(" (shot) with a (");
            sb.Append(SlayerShooterItemId);
            sb.Append(")");
        }

        return sb.ToString();
    }
}
