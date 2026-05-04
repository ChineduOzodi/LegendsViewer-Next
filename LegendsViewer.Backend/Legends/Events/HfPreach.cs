using System.Text;
using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Extensions;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Legends.WorldObjects;

namespace LegendsViewer.Backend.Legends.Events;

public class HfPreach : WorldEvent
{
    public HistoricalFigure? SpeakerHf { get; set; }
    public Site? Site { get; set; }
    public PreachTopic Topic { get; set; }
    public Entity? Entity1 { get; set; }
    public Entity? Entity2 { get; set; }

    public HfPreach(List<Property> properties, IWorld world) : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "speaker_hfid": SpeakerHf = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                case "site_hfid": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                case "topic":
                    switch (property.Value)
                    {
                        case "entity 1 should love entity 2":
                            Topic = PreachTopic.Entity1ShouldLoveEntity2;
                            break;
                        case "set entity 1 against entity 2":
                            Topic = PreachTopic.SetEntity1AgainstEntity2;
                            break;
                        default:
                            property.Known = false;
                            break;
                    }
                    break;
                case "entity_1": Entity1 = world.GetEntity(Convert.ToInt32(property.Value)); break;
                case "entity_2": Entity2 = world.GetEntity(Convert.ToInt32(property.Value)); break;
            }
        }

        SpeakerHf.AddEvent(this);
        Site.AddEvent(this);
        Entity1.AddEvent(this);
        Entity2.AddEvent(this);
    }

    public override string Print(bool link = true, DwarfObject? pov = null)
    {
        var sb = new StringBuilder();
        sb.Append(GetYearTime());
        sb.Append(SpeakerHf?.ToLink(link, pov, this));
        sb.Append(" preached to ");
        sb.Append(Entity1?.ToLink(link, pov, this));
        switch (Topic)
        {
            case PreachTopic.SetEntity1AgainstEntity2:
                sb.Append(", inveighing against ");
                break;
            case PreachTopic.Entity1ShouldLoveEntity2:
                sb.Append(", urging love to be shown to ");
                break;
        }
        sb.Append(Entity2?.ToLink(link, pov, this));
        if (Site != null)
        {
            sb.Append(" at ");
            sb.Append(Site.ToLink(link, pov, this));
        }
        sb.Append(PrintParentCollection(link, pov));
        sb.Append(".");
        return sb.ToString();
    }
}
