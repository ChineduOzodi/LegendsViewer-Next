namespace LegendsViewer.Backend.Legends.Bookmarks;

using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

public class BookmarkService : IBookmarkService
{
    public const string TimestampPlaceholder = "{TIMESTAMP}";
    public const string FileIdentifierLegendsXml = "-legends.xml";
    public const string FileIdentifierLegendsPlusXml = "-legends_plus.xml";
    private const string BookmarkFileName = "bookmarks.json";

    private readonly ConcurrentDictionary<string, Bookmark> _bookmarks = new();
    private readonly string _bookmarkFilePath;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

    /// <summary>
    /// Creates a BookmarkService that stores bookmarks at the specified file path.
    /// </summary>
    /// <param name="bookmarkFilePath">Full path to the bookmarks.json file.</param>
    public BookmarkService(string bookmarkFilePath)
    {
        if (string.IsNullOrWhiteSpace(bookmarkFilePath))
            throw new ArgumentException("Bookmark file path cannot be null or empty.", nameof(bookmarkFilePath));

        _bookmarkFilePath = bookmarkFilePath;

        string? directory = Path.GetDirectoryName(_bookmarkFilePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        LoadBookmarksFromFile();
    }

    /// <summary>
    /// Creates a BookmarkService using the platform-specific AppData folder.
    /// </summary>
    public BookmarkService() : this(GetDefaultBookmarkFilePath())
    {
    }

    private static string GetDefaultBookmarkFilePath()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string appFolder = Path.Combine(appDataPath, "LegendsViewer");
        Directory.CreateDirectory(appFolder);
        return Path.Combine(appFolder, BookmarkFileName);
    }

    private void LoadBookmarksFromFile()
    {
        if (!File.Exists(_bookmarkFilePath))
            return;

        string json = File.ReadAllText(_bookmarkFilePath);
        var loadedBookmarks = JsonSerializer.Deserialize<Dictionary<string, Bookmark>>(json) ?? [];
        foreach (var kvp in loadedBookmarks)
        {
            var bookmark = kvp.Value;
            ResetBookmark(bookmark);
            if (string.IsNullOrEmpty(bookmark.RegionId))
            {
                bookmark.RegionId = ExtractRegionIdFromFilePath(bookmark.FilePath, worldTimestamps: bookmark.WorldTimestamps.ToArray());
            }
            _bookmarks[bookmark.RegionId] = bookmark;
        }
    }

    /// <summary>
    /// Extracts the stable region ID from a file path.
    /// Example: "/path/to/TheWorld_1253-12-31-legends.xml" -> "TheWorld_1253"
    /// Supports hyphenated world names (e.g., "My-World_1253-12-31-legends.xml" -> "My-World_1253")
    /// </summary>
    /// <param name="filePath">The file path to parse (may contain {TIMESTAMP} placeholder)</param>
    /// <param name="legendsXmlSuffix">Suffix for legends.xml files</param>
    /// <param name="legendsPlusXmlSuffix">Suffix for legends_plus.xml files</param>
    /// <param name="worldTimestamps">Optional: when filePath contains {TIMESTAMP} placeholder, use this to resolve it</param>
    public static string ExtractRegionIdFromFilePath(string filePath, string legendsXmlSuffix = FileIdentifierLegendsXml, string legendsPlusXmlSuffix = FileIdentifierLegendsPlusXml, string[]? worldTimestamps = null)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return string.Empty;
        }

        string fileName = Path.GetFileName(filePath);
        
        // Handle {TIMESTAMP} placeholder for backward compatibility with older bookmarks
        // When the filename contains {TIMESTAMP}, replace it with the first available timestamp
        if (fileName.Contains(TimestampPlaceholder) && worldTimestamps != null && worldTimestamps.Length > 0)
        {
            fileName = fileName.Replace(TimestampPlaceholder, worldTimestamps[0]);
        }
        
        // Handle both legends.xml and legends_plus.xml suffixes
        string regionId = fileName
            .Replace(legendsXmlSuffix, "")
            .Replace(legendsPlusXmlSuffix, "");
        
        // Use regex to find the timestamp pattern: YYYYY-MM-DD (5-digit year)
        // DF files use hyphen between world name and year (e.g., "world-00005-12-31")
        // We use [-_] to handle both hyphen and underscore before year
        var match = Regex.Match(regionId, @"^(.+?)[-_](\d{5}-\d{2}-\d{2})$");
        if (match.Success)
        {
            // Return region name + year: e.g., "My-World_00005"
            return $"{match.Groups[1].Value}_{match.Groups[2].Value[..5]}";
        }
        
        // Fallback for legacy/ill-formed filenames
        var parts = regionId.Split('-');
        if (parts.Length >= 3)
        {
            return $"{parts[0]}-{parts[1]}";
        }
        
        return regionId;
    }

    private static void ResetBookmark(Bookmark bookmark)
    {
        bookmark.State = BookmarkState.Default;
        bookmark.LoadedTimestamp = null;
        bookmark.LatestTimestamp = bookmark.WorldTimestamps.Order().LastOrDefault();
    }

    public Bookmark AddBookmark(Bookmark bookmark)
    {
        ResetAllBookmarks();
        
        // Ensure we have a stable region ID
        if (string.IsNullOrEmpty(bookmark.RegionId))
        {
            bookmark.RegionId = ExtractRegionIdFromFilePath(bookmark.FilePath);
        }

        // Use concurrent dictionary - no manual synchronization needed
        if (_bookmarks.TryGetValue(bookmark.RegionId, out var existingBookmark))
        {
            // Merge timestamps
            foreach (var timestamp in bookmark.WorldTimestamps)
            {
                if (!existingBookmark.WorldTimestamps.Contains(timestamp))
                {
                    existingBookmark.WorldTimestamps.Add(timestamp);
                }
            }
            existingBookmark.WorldName = bookmark.WorldName;
            existingBookmark.WorldAlternativeName = bookmark.WorldAlternativeName;
            existingBookmark.WorldMapImage = bookmark.WorldMapImage;
            existingBookmark.State = BookmarkState.Loaded;
            existingBookmark.LoadedTimestamp = bookmark.LoadedTimestamp;
            existingBookmark.LatestTimestamp = bookmark.LatestTimestamp;
            existingBookmark.FilePath = bookmark.FilePath; // Update to latest file path
            SaveBookmarksToFile();
            return existingBookmark;
        }
        else
        {
            bookmark.State = BookmarkState.Loaded;
            _bookmarks[bookmark.RegionId] = bookmark;
            SaveBookmarksToFile();
            return bookmark;
        }
    }

    private void ResetAllBookmarks()
    {
        foreach (var bookmark in _bookmarks.Values) 
        { 
            ResetBookmark(bookmark); 
        }
    }

    public List<Bookmark> GetAll()
    {
        return [.. _bookmarks.Values];
    }

    public Bookmark? GetBookmark(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return null;
        }

        // Normalize the path for consistent lookup
        string normalizedPath = Path.GetFullPath(filePath);
        
        // Extract region ID from file path
        string regionId = ExtractRegionIdFromFilePath(normalizedPath);
        if (string.IsNullOrEmpty(regionId))
        {
            return null;
        }

        // Try to get bookmark by stable region ID
        return _bookmarks.TryGetValue(regionId, out var bookmark) ? bookmark : null;
    }

    public bool DeleteBookmarkTimestamp(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return false;
        }

        // Extract region ID from file path
        string regionId = ExtractRegionIdFromFilePath(filePath);
        if (string.IsNullOrEmpty(regionId))
        {
            return false;
        }

        // Extract timestamp from file path
        string timestamp = ExtractTimestampFromFilePath(filePath);
        if (string.IsNullOrWhiteSpace(timestamp))
        {
            return false;
        }

        if (!_bookmarks.TryGetValue(regionId, out var bookmark))
        {
            return false;
        }

        bookmark.WorldTimestamps.Remove(timestamp);
        if (bookmark.WorldTimestamps.Count == 0)
        {
            _bookmarks.TryRemove(regionId, out _);
        }
        else
        {
            bookmark.LatestTimestamp = bookmark.WorldTimestamps.Order().LastOrDefault();
        }
        SaveBookmarksToFile();
        return true;
    }

    /// <summary>
    /// Extracts the timestamp from a file path.
    /// Example: "/path/to/TheWorld_00005-12-31-legends.xml" -> "00005-12-31"
    /// </summary>
    public static string ExtractTimestampFromFilePath(string filePath, string legendsXmlSuffix = FileIdentifierLegendsXml, string legendsPlusXmlSuffix = FileIdentifierLegendsPlusXml)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return string.Empty;
        }

        string fileName = Path.GetFileName(filePath);
        
        // Handle both legends.xml and legends_plus.xml suffixes
        string regionId = fileName
            .Replace(legendsXmlSuffix, "")
            .Replace(legendsPlusXmlSuffix, "");
        
        // Use regex to extract timestamp: YYYYY-MM-DD (5-digit year)
        // DF files use hyphen or underscore before the timestamp
        var match = Regex.Match(regionId, @"[-_](\d{5}-\d{2}-\d{2})$");
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        
        return string.Empty;
    }

    private void SaveBookmarksToFile()
    {
        // ConcurrentDictionary is thread-safe for reads/writes, but we need to
        // serialize to a dictionary for JSON
        var bookmarksDict = new Dictionary<string, Bookmark>(_bookmarks);
        string json = JsonSerializer.Serialize(bookmarksDict, _jsonSerializerOptions);
        File.WriteAllText(_bookmarkFilePath, json);
    }

    public static string ReplaceLastOccurrence(string source, string find, string replace)
    {
        int lastIndex = source.LastIndexOf(find, StringComparison.Ordinal);

        if (lastIndex == -1)
        {
            return source; // The string to replace was not found
        }

        return source.Remove(lastIndex, find.Length).Insert(lastIndex, replace);
    }

    /// <summary>
    /// Parses a region ID to extract the region name and timestamp.
    /// This method does NOT mutate any external state.
    /// </summary>
    /// <param name="regionId">The region ID (e.g., "TheWorld_1253-12-31" or "TheWorld_1253")</param>
    /// <returns>A tuple containing the region name and timestamp (e.g., ("TheWorld", "1253-12-31"))</returns>
    public static (string RegionName, string Timestamp) GetRegionNameAndTimestampByRegionId(string regionId)
    {
        if (string.IsNullOrWhiteSpace(regionId))
        {
            return ("", "");
        }

        // Handle both 3-part (Year-Month-Day) and 2-part (Year only) region IDs
        var array = regionId.Split('-').ToList();
        if (array.Count < 3)
        {
            return ("", "");
        }

        string day = array[^1];
        array.RemoveAt(array.Count - 1);
        string month = array[^1];
        array.RemoveAt(array.Count - 1);
        string year = array[^1];
        array.RemoveAt(array.Count - 1);

        // Whatever remains is the region name
        var regionName = string.Join('-', array);
        var timestamp = $"{year}-{month}-{day}";
        return (regionName, timestamp);
    }

    /// <summary>
    /// Extracts just the stable region identifier (region name + year) from a full region ID.
    /// Example: "TheWorld_1253-12-31" -> "TheWorld_1253"
    /// </summary>
    public static string GetStableRegionId(string regionId)
    {
        if (string.IsNullOrWhiteSpace(regionId))
        {
            return "";
        }

        var array = regionId.Split('-').ToList();
        if (array.Count < 4)
        {
            return regionId;
        }

        // Remove month and day, keep region name and year
        array.RemoveAt(array.Count - 1); // Remove day
        array.RemoveAt(array.Count - 1); // Remove month
        
        return string.Join('-', array);
    }
}
