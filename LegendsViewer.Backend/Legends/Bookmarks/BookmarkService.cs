namespace LegendsViewer.Backend.Legends.Bookmarks;

using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;

public class BookmarkService : IBookmarkService
{
    public const string TimestampPlaceholder = "{TIMESTAMP}";
    public const string FileIdentifierLegendsXml = "-legends.xml";
    public const string FileIdentifierLegendsPlusXml = "-legends_plus.xml";
    private const string BookmarkFileName = "bookmarks.json";

    private readonly ConcurrentDictionary<string, Bookmark> _bookmarks = new();
    private readonly ConcurrentDictionary<string, string> _filePathToId = new();
    private readonly string _bookmarkFilePath;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

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
            
            // Ensure Id is set from WorldName + WorldRegionName
            bookmark.Id = $"{bookmark.WorldName}_{bookmark.WorldRegionName}";
            
            string normalizedPath = Path.GetFullPath(bookmark.FilePath);
            _bookmarks[bookmark.Id] = bookmark;
            _filePathToId[normalizedPath] = bookmark.Id;
        }
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
        
        // Ensure Id is set from WorldName + WorldRegionName
        bookmark.Id = $"{bookmark.WorldName}_{bookmark.WorldRegionName}";
        
        if (_bookmarks.TryGetValue(bookmark.Id, out var existingBookmark))
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
            existingBookmark.FilePath = bookmark.FilePath;
            
            string normalizedPath = Path.GetFullPath(bookmark.FilePath);
            _filePathToId[normalizedPath] = bookmark.Id;
            
            SaveBookmarksToFile();
            return existingBookmark;
        }
        else
        {
            bookmark.State = BookmarkState.Loaded;
            _bookmarks[bookmark.Id] = bookmark;
            
            string normalizedPath = Path.GetFullPath(bookmark.FilePath);
            _filePathToId[normalizedPath] = bookmark.Id;
            
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

        string normalizedPath = Path.GetFullPath(filePath);
        
        if (_filePathToId.TryGetValue(normalizedPath, out var id))
        {
            return _bookmarks.TryGetValue(id, out var bookmark) ? bookmark : null;
        }

        return null;
    }

    public bool DeleteBookmarkTimestamp(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return false;
        }

        string normalizedPath = Path.GetFullPath(filePath);
        
        if (!_filePathToId.TryGetValue(normalizedPath, out var id))
        {
            return false;
        }

        string timestamp = ExtractTimestampFromFilePath(filePath);
        if (string.IsNullOrWhiteSpace(timestamp))
        {
            return false;
        }

        if (!_bookmarks.TryGetValue(id, out var bookmark))
        {
            return false;
        }

        bookmark.WorldTimestamps.Remove(timestamp);
        if (bookmark.WorldTimestamps.Count == 0)
        {
            _bookmarks.TryRemove(id, out _);
            _filePathToId.TryRemove(normalizedPath, out _);
        }
        else
        {
            bookmark.LatestTimestamp = bookmark.WorldTimestamps.Order().LastOrDefault();
        }
        SaveBookmarksToFile();
        return true;
    }

    public static string ExtractTimestampFromFilePath(string filePath, string legendsXmlSuffix = FileIdentifierLegendsXml, string legendsPlusXmlSuffix = FileIdentifierLegendsPlusXml)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return string.Empty;
        }

        string fileName = Path.GetFileName(filePath);
        
        string regionId = fileName
            .Replace(legendsXmlSuffix, "")
            .Replace(legendsPlusXmlSuffix, "");
        
        // Extract YYYYY-MM-DD pattern at end of filename
        int lastHyphenIndex = regionId.LastIndexOf('-');
        if (lastHyphenIndex > 0 && lastHyphenIndex < regionId.Length - 6)
        {
            string potentialTimestamp = regionId[(lastHyphenIndex - 4)..];
            if (potentialTimestamp.Length == 10 && potentialTimestamp[4] == '-')
            {
                return potentialTimestamp;
            }
        }
        
        return string.Empty;
    }

    private void SaveBookmarksToFile()
    {
        var bookmarksDict = new Dictionary<string, Bookmark>(_bookmarks);
        string json = JsonSerializer.Serialize(bookmarksDict, _jsonSerializerOptions);
        File.WriteAllText(_bookmarkFilePath, json);
    }

    public static string ReplaceLastOccurrence(string source, string find, string replace)
    {
        int lastIndex = source.LastIndexOf(find, StringComparison.Ordinal);

        if (lastIndex == -1)
        {
            return source;
        }

        return source.Remove(lastIndex, find.Length).Insert(lastIndex, replace);
    }

    public static (string RegionName, string Timestamp) GetRegionNameAndTimestampByRegionId(string regionId)
    {
        if (string.IsNullOrWhiteSpace(regionId))
        {
            return ("", "");
        }

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

        var regionName = string.Join('-', array);
        var timestamp = $"{year}-{month}-{day}";
        return (regionName, timestamp);
    }
}
