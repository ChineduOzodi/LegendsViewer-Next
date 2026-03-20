namespace LegendsViewer.Backend.Legends.Bookmarks;

public class Bookmark
{
    /// <summary>
    /// Stable identifier (e.g., "TheWorld_region18") used as the primary key.
    /// Format: WorldName_RegionName. No timestamp or year is included,
    /// making it stable across different saves of the same world.
    /// </summary>
    public string Id { get; set; } = "";

    public string FilePath { get; set; } = "";
    public string WorldName { get; set; } = "";
    public string WorldAlternativeName { get; set; } = "";
    public string WorldRegionName { get; set; } = "";
    public List<string> WorldTimestamps { get; set; } = [];
    public int WorldWidth { get; set; }
    public int WorldHeight { get; set; }
    public byte[]? WorldMapImage { get; set; }
    public BookmarkState State { get; set; }
    public string? LoadedTimestamp { get; set; }
    public string? LatestTimestamp { get; set; }

}
