using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class AttackedSite : WorldEvent
{
    public Entity? Attacker { get; set; }
    public Entity? Defender { get; set; }
    public Entity? SiteEntity { get; set; }
    public Entity? AttackerMercenaries { get; set; }
    public Entity? DefenderMercenaries { get; set; }
    public Entity? AttackerSupportMercenaries { get; set; }
    public Entity? DefenderSupportMercenaries { get; set; }
    public Site? Site { get; set; }
    public HistoricalFigure? AttackerGeneral { get; set; }
    public HistoricalFigure? DefenderGeneral { get; set; }

    public AttackedSite(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "attacker_civ_id": Attacker = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "defender_civ_id": Defender = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_civ_id": SiteEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "attacker_general_hfid": AttackerGeneral = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "defender_general_hfid": DefenderGeneral = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "attacker_merc_enid": AttackerMercenaries = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "defender_merc_enid": DefenderMercenaries = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "a_support_merc_enid": AttackerSupportMercenaries = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "d_support_merc_enid": DefenderSupportMercenaries = world.GetEntity(Convert.ToInt32(property.Value)); break;
            }
        }

        Attacker?.AddEvent(this);
        if (Defender != Attacker)
        {
            Defender?.AddEvent(this);
        }
        if (SiteEntity != Defender && SiteEntity != Attacker)
        {
            SiteEntity?.AddEvent(this);
        }
        Site?.AddEvent(this);
        if (AttackerGeneral != null)
        {
            AttackerGeneral?.AddEvent(this);
        }
        if (DefenderGeneral != null)
        {
            DefenderGeneral?.AddEvent(this);
        }
        if (AttackerMercenaries != Defender && AttackerMercenaries != Attacker)
        {
            AttackerMercenaries?.AddEvent(this);
        }
        if (DefenderMercenaries != Defender && DefenderMercenaries != Attacker)
        {
            DefenderMercenaries?.AddEvent(this);
        }
        AttackerSupportMercenaries?.AddEvent(this);
        DefenderSupportMercenaries?.AddEvent(this);
    }
    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Attacker?.PrintEntity(true, pov) ?? "an unknown civilization");
        sb.Append(" attacked ");
        if (SiteEntity != null)
        {
            sb.Append(SiteEntity.PrintEntity(true, pov));
        }
        else
        {
            sb.Append(Defender?.PrintEntity(true, pov) ?? "an unknown civilization");
        }
        sb.Append(" at ");
        sb.Append(Site?.ToLink(link, pov, this));
        sb.Append(". ");
        if (AttackerGeneral != null)
        {
            sb.Append("Leader of the attack was ");
            sb.Append(AttackerGeneral.ToLink(link, pov, this));
        }
        if (DefenderGeneral != null)
        {
            sb.Append(", and the defenders were led by ");
            sb.Append(DefenderGeneral.ToLink(link, pov, this));
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append('.');
        if (AttackerMercenaries != null)
        {
            sb.Append(" ");
            sb.Append(AttackerMercenaries.ToLink(true, pov));
            sb.Append(" were hired by the attackers.");
        }
        if (DefenderMercenaries != null)
        {
            sb.Append(" ");
            sb.Append(DefenderMercenaries.ToLink(true, pov));
            sb.Append(" were hired by the defenders.");
        }
        if (AttackerSupportMercenaries != null)
        {
            sb.Append(" ");
            sb.Append(AttackerSupportMercenaries.ToLink(true, pov));
            sb.Append(" were hired as scouts by the attackers.");
        }
        if (DefenderSupportMercenaries != null)
        {
            sb.Append(" ");
            sb.Append(DefenderSupportMercenaries.ToLink(true, pov));
            sb.Append(" were hired as scouts by the defenders.");
        }
        return sb.ToString();
    }
}

