using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class ArtifactDestroyed : WorldEvent
{
    public Artifact? Artifact { get; set; }
    public Site? Site { get; set; }
    public HistoricalFigure? Destroyer { get; set; }

    public ArtifactDestroyed(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "artifact_id": Artifact = world.GetArtifact(Convert.ToInt32(property.Value)); break;
                case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "destroyer_enid": Destroyer = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
            }
        }

        Site?.AddEvent(this);
        Artifact?.AddEvent(this);
        Destroyer?.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(Artifact?.ToLink(link, pov, this));
        sb.Append(" was destroyed");
        if (Destroyer != null)
        {
            sb.Append(" by ");
            sb.Append(Destroyer?.ToLink(link, pov, this));
        }
        sb.Append(" in ");
        sb.Append(Site?.ToLink(link, pov, this));
        sb.Append('.');
        return sb.ToString();
    }
}

