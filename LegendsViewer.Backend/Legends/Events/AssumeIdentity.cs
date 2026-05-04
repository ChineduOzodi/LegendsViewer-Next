using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.Events;

public class AssumeIdentity : WorldEvent
{
    public HistoricalFigure? Trickster { get; set; }
    public int IdentityId { get; set; }
    public Entity? Target { get; set; }

    public Identity? Identity { get; set; }

    public string? IdentityName { get; set; }
    public CreatureInfo IdentityRace { get; set; } = CreatureInfo.Unknown;
    public string? IdentityCaste { get; set; }

    public AssumeIdentity(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "identity_id":
                    IdentityId = Convert.ToInt32(property.Value);
                    break;
                case "target_enid":
                    Target = world.GetEntity(Convert.ToInt32(property.Value));
                    break;
                case "identity_histfig_id":
                case "identity_nemesis_id":
                case "trickster_hfid":
                case "trickster":
                    if (Trickster == null)
                    {
                        Trickster = world.GetHistoricalFigure(Convert.ToInt32(property.Value));
                    }
                    else
                    {
                        property.Known = true;
                    }
                    break;
                case "target":
                    if (Target == null)
                    {
                        Target = world.GetEntity(Convert.ToInt32(property.Value));
                    }
                    else
                    {
                        property.Known = true;
                    }
                    break;
                case "identity_name":
                    IdentityName = property.Value;
                    break;
                case "identity_race":
                    IdentityRace = world.GetCreatureInfo(property.Value);
                    break;
                case "identity_caste":
                    IdentityCaste = Formatting.InitCaps(property.Value);
                    break;
            }
        }

        Trickster?.AddEvent(this);
        Target?.AddEvent(this);
        if (!string.IsNullOrEmpty(IdentityName))
        {
            Identity = new Identity(IdentityName, IdentityRace, IdentityCaste);
        }
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Trickster?.ToLink(link, pov, this) ?? "an unknown creature");
        if (Target != null)
        {
            sb.Append(" fooled ");
            sb.Append(Target?.ToLink(link, pov, this) ?? "an unknown civilization");
            sb.Append(" into believing ");
            sb.Append(Trickster?.ToLink(link, pov, this) ?? "an unknown creature");
            sb.Append(" was ");
        }
        else
        {
            sb.Append(" assumed the identity of ");
        }
        Identity? identity = Trickster?.Identities.Find(i => i.Id == IdentityId) ?? Identity;
        if (identity != null)
        {
            sb.Append(identity.Print(link, pov, this));
        }
        else
        {
            sb.Append("someone else");
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append('.');
        return sb.ToString();
    }
}

