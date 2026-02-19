using LegendsViewer.Backend.Legends.Interfaces;
using LegendsViewer.Backend.Legends.Enums;
using LegendsViewer.Backend.Legends.Events;
using LegendsViewer.Backend.Legends.Parser;
using LegendsViewer.Backend.Utilities;

namespace LegendsViewer.Backend.Legends.WorldObjects;

public class ArtForm : WorldObject
{
    public string Description { get; set; } = string.Empty;
    public FormType FormType { get; set; }

    public ArtForm(List<Property> properties, IWorld world)
        : base(properties, world)
    {
        foreach (Property property in properties)
        {
            switch (property.Name)
            {
                case "name":
                    Name = Formatting.InitCaps(property.Value);
                    break;
                case "description":
                    var index = property.Value.IndexOf(" is a ", StringComparison.Ordinal);
                    if (index != -1 && string.IsNullOrEmpty(Name))
                    {
                        Name = property.Value[..index];
                    }
                    Description = property.Value;
                    break;
            }
        }
        if (string.IsNullOrEmpty(Name))
        {
            Name = "Untitled";
        }
    }

    public override string ToString() { return Name; }

    public override string ToLink(bool link = true, DwarfObject? pov = null, WorldEvent? worldEvent = null) { return Name; }
}


