using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class HfsFormedReputationRelationship : WorldEvent
{
    public HistoricalFigure? HistoricalFigure1 { get; set; }
    public HistoricalFigure? HistoricalFigure2 { get; set; }
    public int IdentityId1 { get; set; }
    public int IdentityId2 { get; set; }
    public ReputationType HfRep1Of2 { get; set; }
    public ReputationType HfRep2Of1 { get; set; }
    public Site? Site { get; set; }
    public WorldRegion? Region { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }

    public HfsFormedReputationRelationship(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "hfid1": HistoricalFigure1 = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "hfid2": HistoricalFigure2 = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "identity_id1": IdentityId1 = Convert.ToInt32(property.Value); break;
                case "identity_id2": IdentityId2 = Convert.ToInt32(property.Value); break;
                case "hf_rep_1_of_2":
                    switch (property.Value)
                    {
                        case "information source": HfRep1Of2 = ReputationType.InformationSource; break;
                        default: property.Known = false; break;
                    }
                    break;
                case "hf_rep_2_of_1":
                    switch (property.Value)
                    {
                        case "information source": HfRep2Of1 = ReputationType.InformationSource; break;
                        case "buddy": HfRep2Of1 = ReputationType.Buddy; break;
                        case "friendly": HfRep2Of1 = ReputationType.Friendly; break;
                        default: property.Known = false; break;
                    }
                    break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
            }
        }
        HistoricalFigure1.AddEvent(this);
        if (HistoricalFigure1 != HistoricalFigure2)
        {
            HistoricalFigure2.AddEvent(this);
        }
        Site.AddEvent(this);
        Region.AddEvent(this);
        UndergroundRegion.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(HistoricalFigure1?.ToLink(link, pov, this));
        Identity? identity1 = HistoricalFigure1?.Identities.Find(i => i.Id == IdentityId1);
        if (identity1 != null)
        {
            sb.Append(" as '");
            sb.Append(identity1.Print(link, pov, this));
            sb.Append("'");
        }
        sb.Append(", formed a false friendship with ");
        sb.Append(HistoricalFigure2?.ToLink(link, pov, this));
        Identity? identity2 = HistoricalFigure2?.Identities.Find(i => i.Id == IdentityId2);
        if (identity2 != null)
        {
            sb.Append(" as '");
            sb.Append(identity2.Print(link, pov, this));
            sb.Append("'");
        }
        if (HfRep2Of1 == ReputationType.Buddy || HfRep2Of1 == ReputationType.Friendly)
        {
            sb.Append(" in order to extract information");
        }
        else if (HfRep2Of1 == ReputationType.InformationSource)
        {
            sb.Append(" where each used the other for information");
        }
        sb.Append(" in ");
        if (Site != null)
        {
            sb.Append(Site.ToLink(link, pov, this));
        }
        else if (Region != null)
        {
            sb.Append(Region.ToLink(link, pov, this));
        }
        else if (UndergroundRegion != null)
        {
            sb.Append(UndergroundRegion.ToLink(link, pov, this));
        }
        else
        {
            sb.Append("UNKNOWN LOCATION");
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}
