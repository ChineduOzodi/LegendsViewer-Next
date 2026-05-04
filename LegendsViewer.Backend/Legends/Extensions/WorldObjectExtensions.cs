using LegendsViewer.Backend.Legends.EventCollections;
using LegendsViewer.Backend.Legends.Events;

namespace LegendsViewer.Backend.Legends.Extensions;

public static class WorldObjectExtensions
{
    public static void AddEvent(this WorldObject? worldObject, WorldEvent? worldEvent)
    {
        if (worldObject == null || worldEvent == null || worldObject.Id == -1 || worldEvent.Id == -1)
        {
            return;
        }
        if (worldObject.Events.GetLegendsObject(worldEvent.Id) == null)
        {
            worldObject.Events.Add(worldEvent);
        }
        else
        {
#if DEBUG
            worldEvent.World?.ParsingErrors.Report($"Already added event {worldEvent.Id} '{worldEvent.Type}' to object {worldObject.Id} '{worldObject.GetType()}'");
#endif
        }
    }

    public static void AddEventCollection(this WorldObject? worldObject, EventCollection? eventCollection)
    {
        if (worldObject == null || eventCollection == null || worldObject.Id == -1 || eventCollection.Id == -1)
        {
            return;
        }
        if (worldObject.EventCollections.GetLegendsObject(eventCollection.Id) == null)
        {
            worldObject.EventCollections.Add(eventCollection);
        }
        else
        {
#if DEBUG
            eventCollection.World?.ParsingErrors.Report($"Already added eventCollection {eventCollection.Id} '{eventCollection.Type}' to object {worldObject.Id} '{worldObject.GetType()}'");
#endif
        }
    }

    public static T? GetLegendsObject<T>(this List<T> list, int id) where T : DwarfObject
    {
        if (id < 0)
        {
            return null;
        }
        if (id < list.Count && list[id].Id == id)
        {
            return list[id];
        }
        int min = 0;
        int max = list.Count - 1;
        while (min <= max)
        {
            int mid = min + (max - min) / 2;
            if (id > list[mid].Id)
            {
                min = mid + 1;
            }
            else if (id < list[mid].Id)
            {
                max = mid - 1;
            }
            else
            {
                return list[mid];
            }
        }
        return null;
    }
}