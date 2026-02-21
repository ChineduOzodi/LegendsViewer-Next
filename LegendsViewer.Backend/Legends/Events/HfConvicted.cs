using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class HfConvicted : WorldEvent
{
    public HistoricalFigure? TargetHf { get; set; }
    public HistoricalFigure? ConvictedHf { get; set; }
    public Entity? ConvicterEntity { get; set; }
    public string? Crime { get; set; }
    public HistoricalFigure? FooledHf { get; set; }
    public HistoricalFigure? FramerHf { get; set; }
    public int PrisonMonth { get; set; }
    public bool DeathPenalty { get; set; }
    public bool Beating { get; set; }
    public int Hammerstrokes { get; set; }
    public bool WrongfulConviction { get; set; }
    public HistoricalFigure? CorruptConvictorHf { get; set; }
    public HistoricalFigure? PlotterHf { get; set; }
    public bool Exiled { get; set; }

    public bool SurveiledConvicted { get; set; }
    public bool HeldFirmInInterrogation { get; set; }
    public HistoricalFigure? CoConspiratorHf { get; set; }
    public bool SurveiledCoConspirator { get; set; }
    public bool ConvictIsContact { get; set; }
    public HistoricalFigure? ImplicatedHf { get; set; }
    public Entity? ConfessedAfterApbArrestEntity { get; set; }
    public HistoricalFigure? InterrogatorHf { get; set; }
    public HistoricalFigure? ContactHf { get; set; }
    public bool SurveiledContact { get; set; }
    public bool SurveiledTarget { get; set; }

    public HfConvicted(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "target_hfid": TargetHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "convicted_hfid": ConvictedHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "convicter_enid": ConvicterEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "crime": Crime = property.Value; break;
                case "prison_months": PrisonMonth = Convert.ToInt32(property.Value); break;
                case "fooled_hfid": FooledHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "framer_hfid": FramerHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "death_penalty": property.Known = true; DeathPenalty = true; break;
                case "beating": property.Known = true; Beating = true; break;
                case "hammerstrokes": Hammerstrokes = Convert.ToInt32(property.Value); break;
                case "wrongful_conviction": property.Known = true; WrongfulConviction = true; break;
                case "corrupt_convicter_hfid": CorruptConvictorHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "plotter_hfid": PlotterHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "exiled": property.Known = true; Exiled = true; break;

                case "held_firm_in_interrogation": property.Known = true; HeldFirmInInterrogation = true; break;
                case "convict_is_contact": property.Known = true; ConvictIsContact = true; break;
                case "surveiled_convicted": property.Known = true; SurveiledConvicted = true; break;
                case "surveiled_coconspirator": property.Known = true; SurveiledCoConspirator = true; break;
                case "surveiled_contact": property.Known = true; SurveiledContact = true; break;
                case "surveiled_target": property.Known = true; SurveiledContact = true; break;
                case "confessed_after_apb_arrest_enid": ConfessedAfterApbArrestEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "coconspirator_hfid": CoConspiratorHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "implicated_hfid": ImplicatedHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "interrogator_hfid": InterrogatorHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "contact_hfid": ContactHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
            }
        }
        TargetHf.AddEvent(this);
        ConvictedHf.AddEvent(this);
        ConvicterEntity.AddEvent(this);
        if (FooledHf != ConvictedHf)
        {
            FooledHf.AddEvent(this);
        }
        FramerHf.AddEvent(this);
        CorruptConvictorHf.AddEvent(this);
        if (PlotterHf != CorruptConvictorHf)
        {
            PlotterHf.AddEvent(this);
        }

        if (ConvicterEntity != ConfessedAfterApbArrestEntity)
        {
            ConfessedAfterApbArrestEntity.AddEvent(this);
        }
        CoConspiratorHf.AddEvent(this);
        ImplicatedHf.AddEvent(this);
        InterrogatorHf.AddEvent(this);
        if (ImplicatedHf != ContactHf)
        {
            ContactHf.AddEvent(this);
        }
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        if (HeldFirmInInterrogation)
        {
            sb.Append("due to ongoing surveillance");
            if (SurveiledContact & ContactHf != null)
            {
                sb.Append(" on the contact ");
                sb.Append(ContactHf?.ToLink(link, pov, this));
            }
            if (SurveiledCoConspirator & CoConspiratorHf != null)
            {
                sb.Append(" on a coconspirator ");
                sb.Append(CoConspiratorHf?.ToLink(link, pov, this));
            }
            if (SurveiledTarget & TargetHf != null)
            {
                sb.Append(" on a target ");
                sb.Append(TargetHf?.ToLink(link, pov, this));
            }
            sb.Append(" as the plot unfolded, ");
        }
        sb.Append(ConvictedHf?.ToLink(link, pov, this));
        sb.Append($" was {(WrongfulConviction ? "wrongfully " : "")}convicted ");
        if (ConvictIsContact)
        {
            sb.Append($"as a go-between in a conspiracy to commit {Crime} ");
        }
        else
        {
            sb.Append($"of {Crime} ");
        }
        if (ConvicterEntity != null)
        {
            sb.Append("by ");
            sb.Append(ConvicterEntity?.ToLink(link, pov, this));
        }

        if (CorruptConvictorHf != null)
        {
            sb.Append(" and ");
            sb.Append(CorruptConvictorHf?.ToLink(link, pov, this));
        }
        if (PlotterHf != null && PlotterHf != CorruptConvictorHf)
        {
            sb.Append(" plotted by ");
            sb.Append(PlotterHf?.ToLink(link, pov, this));
        }
        if (FooledHf != null && FramerHf != null)
        {
            sb.Append(" after ");
            sb.Append(FramerHf?.ToLink(link, pov, this));
            sb.Append(" fooled ");
            sb.Append(FooledHf?.ToLink(link, pov, this));
            sb.Append(" with fabricated evidence");
        }

        if (Beating)
        {
            sb.Append(", beaten");
        }
        else if (Hammerstrokes > 0)
        {
            sb.Append($", sentenced to {Hammerstrokes} hammerstrokes");
        }
        if (PrisonMonth > 0)
        {
            sb.Append($" and imprisoned for a term of {(PrisonMonth > 12 ? PrisonMonth / 12 + " years" : PrisonMonth + " month")}");
        }
        else if (DeathPenalty)
        {
            sb.Append(" and sentenced to death");
        }
        if (Exiled)
        {
            sb.Append(" and exiled");
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(". ");
        if (ImplicatedHf != null)
        {
            sb.Append(ConvictedHf?.ToLink(link, pov, this));
            sb.Append(" implicated ");
            sb.Append(ImplicatedHf.ToLink(link, pov, this));
            sb.Append(" during interrogation. ");
        }

        if (InterrogatorHf != null)
        {
            sb.Append("Interrogation was led by ");
            sb.Append(InterrogatorHf.ToLink(link, pov, this));
            sb.Append(". ");
        }
        return sb.ToString();
    }
}
