using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.Various;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class ArtifactLost : WorldEvent
{
    public Artifact? Artifact { get; set; }
    public Site? Site { get; set; }
    public WorldRegion? Region { get; set; }
    public UndergroundRegion? UndergroundRegion { get; set; }
    public SiteProperty? SiteProperty { get; set; }

    public ArtifactLost(List<Property> properties, IWorld world) : base(properties, world)
    {
        int sitePropertyId = -1;
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "artifact_id": Artifact = world.GetArtifact(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "subregion_id": Region = world.GetRegion(Convert.ToInt32(property.Value)); break;
                case "feature_layer_id": UndergroundRegion = world.GetUndergroundRegion(Convert.ToInt32(property.Value)); break;
                case "site_property_id": sitePropertyId = Convert.ToInt32(property.Value); break;
            }
        }

        if (Site != null && sitePropertyId != -1)
        {
            SiteProperty = Site.SiteProperties.Find(sp => sp.Id == sitePropertyId);
        }
        Artifact?.AddEvent(this);
        Site?.AddEvent(this);
        Region?.AddEvent(this);
        UndergroundRegion?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Artifact != null ? Artifact.ToLink(link, pov, this) : "UNKNOWN ARTIFACT");
        sb.Append(" was lost");
        if (SiteProperty != null)
        {
            sb.Append(" in ");
            sb.Append(SiteProperty.Print(link, pov));
        }
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

        sb.Append(PrintParentCollection(link, pov));
        sb.Append('.');
        return sb.ToString();
    }
}

