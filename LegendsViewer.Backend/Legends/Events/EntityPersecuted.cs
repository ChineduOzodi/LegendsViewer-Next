using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.Events;

public class EntityPersecuted : WorldEvent
{
    public HistoricalFigure? PersecutorHf { get; set; }
    public Entity? PersecutorEntity { get; set; }
    public Entity? TargetEntity { get; set; }
    public Site? Site { get; set; }
    public int DestroyedStructureId { get; set; }
    public Structure? DestroyedStructure { get; set; }
    public int ShrineAmountDestroyed { get; set; }
    public List<HistoricalFigure> ExpelledHfs { get; set; } = [];
    public List<HistoricalFigure> PropertyConfiscatedFromHfs { get; set; } = [];
    public List<int> ExpelledCreatures { get; set; } = [];
    public List<int> ExpelledPopIds { get; set; } = [];
    public List<int> ExpelledNumbers { get; set; } = [];

    public EntityPersecuted(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "persecutor_hfid": PersecutorHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "persecutor_enid": PersecutorEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "target_enid": TargetEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "shrine_amount_destroyed": ShrineAmountDestroyed = Convert.ToInt32(property.Value); break;
                case "destroyed_structure_id": DestroyedStructureId = Convert.ToInt32(property.Value); break;
                case "property_confiscated_from_hfid":
                    HistoricalFigure? propertyConfiscatedFromHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value));
                    if (propertyConfiscatedFromHf != null)
                    {
                        PropertyConfiscatedFromHfs.Add(propertyConfiscatedFromHf);
                    }
                    break;
                case "expelled_hfid":
                    HistoricalFigure? expelledHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value));
                    if (expelledHf != null)
                    {
                        ExpelledHfs.Add(expelledHf);
                    }

                    break;
                case "expelled_creature": ExpelledCreatures.Add(Convert.ToInt32(property.Value)); break;
                case "expelled_pop_id": ExpelledPopIds.Add(Convert.ToInt32(property.Value)); break;
                case "expelled_number": ExpelledNumbers.Add(Convert.ToInt32(property.Value)); break;
            }
        }
        if (Site != null)
        {
            DestroyedStructure = Site.Structures.Find(structure => structure.LocalId == DestroyedStructureId);
            DestroyedStructure?.AddEvent(this);
        }
        PersecutorHf?.AddEvent(this);
        PersecutorEntity?.AddEvent(this);
        TargetEntity?.AddEvent(this);
        Site?.AddEvent(this);
        foreach (HistoricalFigure expelledHf in ExpelledHfs.Where(eHf => eHf != null))
        {
            expelledHf.AddEvent(this);
        }
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(PersecutorHf?.ToLink(link, pov, this));
        sb.Append(" of ");
        sb.Append(PersecutorEntity?.ToLink(link, pov, this));
        sb.Append(" persecuted ");
        sb.Append(TargetEntity?.ToLink(link, pov, this));
        sb.Append(" in ");
        sb.Append(Site?.ToLink(link, pov, this));
        if (ExpelledHfs.Count > 0)
        {
            sb.Append(". ");
            if (ExpelledHfs.Count == 1)
            {
                sb.Append(ExpelledHfs[0].ToLink(link, pov, this).ToUpperFirstLetter());
                sb.Append(" was");
            }
            else
            {
                sb.Append(ExpelledHfs[0].ToLink(link, pov, this).ToUpperFirstLetter());
                for (int i = 1; i < ExpelledHfs.Count; i++)
                {
                    if (i == ExpelledHfs.Count - 1)
                    {
                        sb.Append(" and ");
                    }
                    else
                    {
                        sb.Append(", ");
                    }
                    sb.Append(ExpelledHfs[i].ToLink(link, pov, this));
                }
                sb.Append(" were");
            }
            sb.Append(" expelled");
            if (ShrineAmountDestroyed > 0 || DestroyedStructure != null)
            {
                sb.Append(" and");
            }
        }
        else
        {
            sb.Append(" and");
        }

        if (DestroyedStructure != null)
        {
            sb.Append(" ");
            sb.Append(DestroyedStructure.ToLink(link, pov, this));
            sb.Append(" was destroyed");
            if (ShrineAmountDestroyed > 0)
            {
                sb.Append(" along with several smaller sacred sites");
            }
        }
        else if (ShrineAmountDestroyed > 0)
        {
            sb.Append(" and some sacred sites were desecrated");
        }
        sb.Append(".");
        return sb.ToString();
    }
}


