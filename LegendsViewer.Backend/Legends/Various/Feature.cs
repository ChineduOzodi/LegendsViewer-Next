using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Parser;

namespace LegendsViewer.Backend.Legends.Various;

public class Feature
{
    public string Type { get; set; } = string.Empty;
    public int Reference { get; set; } // legends_plus.xml

    public Feature(List<Property> properties, IWorld world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "type": Type = property.Value; break;
                case "reference": Reference = Convert.ToInt32(property.Value); break;
            }
        }
    }
}


