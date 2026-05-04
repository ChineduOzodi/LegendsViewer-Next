using LegendsViewer.Backend.Contracts;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;

namespace LegendsViewer.Backend.Legends.WorldObjects;

/// <summary>
/// Responsible for parsing World properties and populating a HistoricalFigure instance.
/// Separates parsing logic from the main HistoricalFigure class.
/// </summary>
public class HistoricalFigurePropertyParser
{
    private static readonly List<string> KnownEntitySubProperties = ["entity_id", "link_strength", "link_type", "position_profile_id", "start_year", "end_year"];
    private static readonly List<string> KnownSiteLinkSubProperties = ["link_type", "site_id", "sub_id", "entity_id", "occupation_id"];
    private static readonly List<string> KnownEntitySquadLinkProperties = ["squad_id", "squad_position", "entity_id", "start_year", "end_year"];

    private readonly HistoricalFigure _historicalFigure;
    private readonly IWorld _world;

    public HistoricalFigurePropertyParser(HistoricalFigure historicalFigure, IWorld world)
    {
        _historicalFigure = historicalFigure;
        _world = world;
    }

    /// <summary>
    /// Parses all properties and populates the HistoricalFigure instance.
    /// </summary>
    public void Parse(List<Property> properties)
    {
        foreach (Property property in properties)
        {
            ParseProperty(property);
        }

        // Post-parsing adjustments
        FinalizeHistoricalFigure();
    }

    private void ParseProperty(Property property)
    {
        switch (property.Name)
        {
            case "appeared": _historicalFigure.Appeared = Convert.ToInt32(property.Value); break;
            case "birth_year": _historicalFigure.BirthYear = Convert.ToInt32(property.Value); break;
            case "birth_seconds72": _historicalFigure.BirthSeconds72 = Convert.ToInt32(property.Value); break;
            case "death_year": _historicalFigure.DeathYear = Convert.ToInt32(property.Value); break;
            case "death_seconds72": _historicalFigure.DeathSeconds72 = Convert.ToInt32(property.Value); break;
            case "name": _historicalFigure.Name = LegendsViewer.Backend.Utilities.Formatting.InitCaps(property.Value.Replace("'", "`")); break;
            case "race": _historicalFigure.Race = _world.GetCreatureInfo(property.Value); break;
            case "caste": _historicalFigure.Caste = LegendsViewer.Backend.Utilities.Formatting.InitCaps(property.Value); break;
            case "associated_type": _historicalFigure.AssociatedType = LegendsViewer.Backend.Utilities.Formatting.InitCaps(property.Value); break;
            case "deity": _historicalFigure.IsDeity = true; property.Known = true; break;
            case "skeleton": _historicalFigure.Skeleton = true; property.Known = true; break;
            case "force": _historicalFigure.Force = true; property.Known = true; _historicalFigure.Race = _world.GetCreatureInfo("Force"); break;
            case "zombie": _historicalFigure.Zombie = true; property.Known = true; break;
            case "ghost": _historicalFigure.Ghost = true; property.Known = true; break;
            case "hf_link": ParseHfLink(property); break;
            case "entity_link":
            case "entity_former_position_link":
            case "entity_position_link":
                ParseEntityLink(property);
                break;
            case "entity_reputation": ParseEntityReputation(property); break;
            case "entity_squad_link":
            case "entity_former_squad_link":
                ParseEntitySquadLink(property);
                break;
            case "relationship_profile_hf": ParseRelationshipProfileHf(property); break;
            case "relationship_profile_hf_identity": ParseRelationshipProfileHfIdentity(property); break;
            case "relationship_profile_hf_visual": ParseRelationshipProfileHfVisual(property); break;
            case "relationship_profile_hf_historical": ParseRelationshipProfileHfHistorical(property); break;
            case "site_link": ParseSiteLink(property); break;
            case "hf_skill": ParseHfSkill(property); break;
            case "active_interaction": _historicalFigure.ActiveInteractions.Add(string.Intern(property.Value)); break;
            case "interaction_knowledge": _historicalFigure.InteractionKnowledge.Add(string.Intern(property.Value)); break;
            case "animated": _historicalFigure.Animated = true; property.Known = true; break;
            case "animated_string": _historicalFigure.AnimatedType = LegendsViewer.Backend.Utilities.Formatting.InitCaps(property.Value); break;
            case "journey_pet": ParseJourneyPet(property); break;
            case "goal": _historicalFigure.Goal = LegendsViewer.Backend.Utilities.Formatting.InitCaps(property.Value); break;
            case "sphere": _historicalFigure.Spheres.Add(property.Value); break;
            case "current_identity_id": _historicalFigure.CurrentIdentityId = Convert.ToInt32(property.Value); break;
            case "used_identity_id": _historicalFigure.UsedIdentityIds.Add(Convert.ToInt32(property.Value)); break;
            case "ent_pop_id": _historicalFigure.EntityPopulationId = Convert.ToInt32(property.Value); break;
            case "holds_artifact": ParseHoldsArtifact(property); break;
            case "adventurer": _historicalFigure.Adventurer = true; property.Known = true; break;
            case "breed_id": ParseBreedId(property); break;
            case "sex": property.Known = true; break;
            case "site_property": ParseSiteProperty(property); break;
            case "vague_relationship": ParseVagueRelationship(property); break;
            case "honor_entity": ParseHonorEntity(property); break;
            case "intrigue_actor": ParseIntrigueActor(property); break;
            case "intrigue_plot": ParseIntriguePlot(property); break;
        }
    }

    private void ParseHfLink(Property property)
    {
        _world.AddHFtoHfLink(_historicalFigure, property);
        property.Known = true;
        List<string> knownSubProperties = ["hfid", "link_strength", "link_type"];
        if (property.SubProperties != null)
        {
            foreach (string subPropertyName in knownSubProperties)
            {
                Property? subProperty = property.SubProperties.Find(property1 => property1.Name == subPropertyName);
                if (subProperty != null)
                {
                    subProperty.Known = true;
                }
            }
        }
    }

    private void ParseEntityLink(Property property)
    {
        _world.AddHFtoEntityLink(_historicalFigure, property);
        property.Known = true;
        if (property.SubProperties != null)
        {
            foreach (string subPropertyName in KnownEntitySubProperties)
            {
                Property? subProperty = property.SubProperties.Find(property1 => property1.Name == subPropertyName);
                if (subProperty != null)
                {
                    subProperty.Known = true;
                }
            }
        }
    }

    private void ParseEntityReputation(Property property)
    {
        _world.AddReputation(_historicalFigure, property);
        property.Known = true;
        if (property.SubProperties != null)
        {
            foreach (string subPropertyName in Reputation.KnownReputationSubProperties)
            {
                Property? subProperty = property.SubProperties.Find(property1 => property1.Name == subPropertyName);
                if (subProperty != null)
                {
                    subProperty.Known = true;
                }
            }
        }
    }

    private void ParseEntitySquadLink(Property property)
    {
        property.Known = true;
        if (property.SubProperties != null)
        {
            foreach (string subPropertyName in KnownEntitySquadLinkProperties)
            {
                Property? subProperty = property.SubProperties.Find(property1 => property1.Name == subPropertyName);
                if (subProperty != null)
                {
                    subProperty.Known = true;
                }
            }
        }
    }

    private void ParseRelationshipProfileHf(Property property)
    {
        property.Known = true;
        if (property.SubProperties != null)
        {
            _historicalFigure.RelationshipProfiles.Add(new RelationshipProfileHf(property.SubProperties, RelationShipProfileType.Unknown));
        }
    }

    private void ParseRelationshipProfileHfIdentity(Property property)
    {
        property.Known = true;
        if (property.SubProperties != null)
        {
            var relationshipProfileHfIdentity = new RelationshipProfileHf(property.SubProperties, RelationShipProfileType.Identity);
            _historicalFigure.RelationshipProfilesOfIdentities.Add(relationshipProfileHfIdentity.Id, relationshipProfileHfIdentity);
        }
    }

    private void ParseRelationshipProfileHfVisual(Property property)
    {
        property.Known = true;
        if (property.SubProperties != null)
        {
            _historicalFigure.RelationshipProfiles.Add(new RelationshipProfileHf(property.SubProperties, RelationShipProfileType.Visual));
        }
    }

    private void ParseRelationshipProfileHfHistorical(Property property)
    {
        property.Known = true;
        if (property.SubProperties != null)
        {
            _historicalFigure.RelationshipProfiles.Add(new RelationshipProfileHf(property.SubProperties, RelationShipProfileType.Historical));
        }
    }

    private void ParseSiteLink(Property property)
    {
        _world.AddHFtoSiteLink(_historicalFigure, property);
        property.Known = true;
        if (property.SubProperties != null)
        {
            foreach (string subPropertyName in KnownSiteLinkSubProperties)
            {
                Property? subProperty = property.SubProperties.Find(property1 => property1.Name == subPropertyName);
                if (subProperty != null)
                {
                    subProperty.Known = true;
                }
            }
        }
    }

    private void ParseHfSkill(Property property)
    {
        property.Known = true;
        if (property.SubProperties != null)
        {
            var skill = new Skill(property.SubProperties);
            _historicalFigure.Skills.Add(skill);
        }
    }

    private void ParseJourneyPet(Property property)
    {
        var creatureInfo = _world.GetCreatureInfo(property.Value);
        _historicalFigure.JourneyPets.Add(new ListItemDto
        {
            Title = creatureInfo != CreatureInfo.Unknown ? creatureInfo.NameSingular : LegendsViewer.Backend.Utilities.Formatting.FormatRace(property.Value)
        });
    }

    private void ParseHoldsArtifact(Property property)
    {
        var artifact = _world.GetArtifact(Convert.ToInt32(property.Value));
        if (artifact != null)
        {
            _historicalFigure.HoldingArtifacts.Add(artifact);
            artifact.Holder = _historicalFigure;
        }
    }

    private void ParseBreedId(Property property)
    {
        _historicalFigure.BreedId = property.Value;
        if (!string.IsNullOrWhiteSpace(_historicalFigure.BreedId))
        {
            if (_world.Breeds.ContainsKey(_historicalFigure.BreedId))
            {
                _world.Breeds[_historicalFigure.BreedId].Add(_historicalFigure);
            }
            else
            {
                _world.Breeds.Add(_historicalFigure.BreedId, [_historicalFigure]);
            }
        }
    }

    private void ParseSiteProperty(Property property)
    {
        property.Known = true;
        if (property.SubProperties != null)
        {
            foreach (Property subProperty in property.SubProperties)
            {
                switch (subProperty.Name)
                {
                    case "site_id":
                    case "property_id":
                        subProperty.Known = true;
                        break;
                }
            }
        }
    }

    private void ParseVagueRelationship(Property property)
    {
        property.Known = true;
        if (property.SubProperties != null)
        {
            _historicalFigure.VagueRelationships.Add(new VagueRelationship(property.SubProperties));
        }
    }

    private void ParseHonorEntity(Property property)
    {
        property.Known = true;
        if (property.SubProperties != null)
        {
            _historicalFigure.HonorEntity = new HonorEntity(property.SubProperties, _world);
        }
    }

    private void ParseIntrigueActor(Property property)
    {
        property.Known = true;
        if (property.SubProperties != null)
        {
            _historicalFigure.IntrigueActors.Add(new IntrigueActor(property.SubProperties));
        }
    }

    private void ParseIntriguePlot(Property property)
    {
        property.Known = true;
        if (property.SubProperties != null)
        {
            _historicalFigure.IntriguePlots.Add(new IntriguePlot(property.SubProperties));
        }
    }

    private void FinalizeHistoricalFigure()
    {
        if (string.IsNullOrWhiteSpace(_historicalFigure.Name))
        {
            _historicalFigure.Name = !string.IsNullOrWhiteSpace(_historicalFigure.AnimatedType) 
                ? LegendsViewer.Backend.Utilities.Formatting.InitCaps(_historicalFigure.AnimatedType) 
                : "(Unnamed)";
        }
        _historicalFigure.Subtype = _historicalFigure.Caste ?? string.Empty;
        if (_historicalFigure.Adventurer)
        {
            _world.AddPlayerRelatedDwarfObjects(_historicalFigure);
        }
        _historicalFigure.Icon = _historicalFigure.GetIcon();
    }
}
