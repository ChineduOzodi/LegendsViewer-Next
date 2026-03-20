using LegendsViewer.Backend.Legends.Bookmarks;
using System.Collections.Concurrent;
using System.Text.Json;

namespace LegendsViewer.Backend.Tests;

[TestClass]
public class BookmarkServiceTests : IDisposable
{
    private readonly string _testFolder;

    public BookmarkServiceTests()
    {
        _testFolder = Path.Combine(Path.GetTempPath(), $"BookmarkTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testFolder);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_testFolder))
            {
                Directory.Delete(_testFolder, true);
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    private string BookmarkFilePath => Path.Combine(_testFolder, "bookmarks.json");

    private IBookmarkService CreateService() => new BookmarkService(BookmarkFilePath);

    [TestMethod]
    public void AddBookmark_SingleBookmark_ShouldPersist()
    {
        // Arrange
        var service = CreateService();
        var bookmark = CreateTestBookmark(
            "/path/to/world-00001-01-01-legends.xml",
            "TestWorld",
            "00001-01-01"
        );

        // Act
        var result = service.AddBookmark(bookmark);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("TestWorld", result.WorldName);
        var all = service.GetAll();
        Assert.AreEqual(1, all.Count);
    }

    [TestMethod]
    public void AddBookmark_SameWorldTwice_ShouldNotCreateDuplicates()
    {
        // Arrange - simulates loading the same world multiple times (Issue: duplicate detection)
        var service = CreateService();
        var bookmark1 = CreateTestBookmark(
            "/path/to/world-00001-01-01-legends.xml",
            "TestWorld",
            "00001-01-01"
        );
        var bookmark2 = CreateTestBookmark(
            "/path/to/world-00001-01-01-legends.xml",
            "TestWorld",
            "00001-01-01",
            additionalTimestamps: new[] { "2-2-2" }
        );

        // Act
        service.AddBookmark(bookmark1);
        service.AddBookmark(bookmark2);

        // Assert - should have exactly one bookmark with both timestamps merged
        var all = service.GetAll();
        Assert.AreEqual(1, all.Count, "Should not create duplicate bookmarks for the same world");
        Assert.AreEqual(2, all[0].WorldTimestamps.Count, "Should merge timestamps from both loads");
        Assert.IsTrue(all[0].WorldTimestamps.Contains("00001-01-01"));
        Assert.IsTrue(all[0].WorldTimestamps.Contains("2-2-2"));
    }

    [TestMethod]
    public void AddBookmark_DifferentWorlds_ShouldCreateSeparateBookmarks()
    {
        // Arrange
        var service = CreateService();
        var bookmark1 = CreateTestBookmark("/path/to/world1-00001-01-01-legends.xml", "World1", "00001-01-01");
        var bookmark2 = CreateTestBookmark("/path/to/world2-00002-02-02-legends.xml", "World2", "2-2-2");

        // Act
        service.AddBookmark(bookmark1);
        service.AddBookmark(bookmark2);

        // Assert
        var all = service.GetAll();
        Assert.AreEqual(2, all.Count);
    }

    [TestMethod]
    public void GetBookmark_WithValidPath_ShouldRetrieveBookmark()
    {
        // Arrange
        var service = CreateService();
        var bookmark = CreateTestBookmark(
            "/path/to/world-00001-01-01-legends.xml",
            "TestWorld",
            "00001-01-01"
        );
        service.AddBookmark(bookmark);

        // Act
        var result = service.GetBookmark("/path/to/world-00001-01-01-legends.xml");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("TestWorld", result.WorldName);
    }

    [TestMethod]
    public void GetBookmark_WithNonExistentPath_ShouldReturnNull()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = service.GetBookmark("/nonexistent/path-legends.xml");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void DeleteBookmarkTimestamp_WithValidTimestamp_ShouldRemoveTimestamp()
    {
        // Arrange - simulate Issue #25: deleting a specific timestamp from a bookmark
        var service = CreateService();
        var bookmark = CreateTestBookmark(
            "/path/to/world-00001-01-01-legends.xml",
            "TestWorld",
            "00001-01-01",
            additionalTimestamps: new[] { "2-2-2", "3-3-3" }
        );
        service.AddBookmark(bookmark);

        // Act - delete one timestamp
        var result = service.DeleteBookmarkTimestamp("/path/to/world-00001-01-01-legends.xml");

        // Assert
        Assert.IsTrue(result, "Delete should return true for valid timestamp");
        var all = service.GetAll();
        Assert.AreEqual(1, all.Count);
        Assert.IsFalse(all[0].WorldTimestamps.Contains("00001-01-01"), "Deleted timestamp should be removed");
        Assert.IsTrue(all[0].WorldTimestamps.Contains("2-2-2"));
        Assert.IsTrue(all[0].WorldTimestamps.Contains("3-3-3"));
    }

    [TestMethod]
    public void DeleteBookmarkTimestamp_WhenLastTimestamp_ShouldRemoveEntireBookmark()
    {
        // Arrange - Issue #25 scenario: deleting the only timestamp should remove the bookmark
        var service = CreateService();
        var bookmark = CreateTestBookmark(
            "/path/to/world-00001-01-01-legends.xml",
            "TestWorld",
            "00001-01-01"
        );
        service.AddBookmark(bookmark);

        // Act
        var result = service.DeleteBookmarkTimestamp("/path/to/world-00001-01-01-legends.xml");

        // Assert
        Assert.IsTrue(result);
        var all = service.GetAll();
        Assert.AreEqual(0, all.Count, "Bookmark should be completely removed when last timestamp is deleted");
    }

    [TestMethod]
    public void DeleteBookmarkTimestamp_WithMissingFile_ShouldReturnFalse()
    {
        // Arrange - Issue #25 edge case: file no longer exists on disk
        var service = CreateService();

        // Act - try to delete a non-existent bookmark
        var result = service.DeleteBookmarkTimestamp("/nonexistent/world-00001-01-01-legends.xml");

        // Assert
        Assert.IsFalse(result, "Delete should return false for non-existent bookmark");
    }

    [TestMethod]
    public void GetRegionNameAndTimestampById_WithValidId_ShouldParseCorrectly()
    {
        // Arrange
        var regionId = "TheWorld-00001-01-01";

        // Act
        var (regionName, timestamp) = BookmarkService.GetRegionNameAndTimestampById(regionId);

        // Assert
        Assert.AreEqual("TheWorld", regionName);
        Assert.AreEqual("00001-01-01", timestamp);
    }

    [TestMethod]
    public void GetRegionNameAndTimestampById_WithComplexWorldName_ShouldParseCorrectly()
    {
        // Arrange - path encoding test: world names with special characters
        var regionId = "World-With-Special-Name-12345-06-15";

        // Act
        var (regionName, timestamp) = BookmarkService.GetRegionNameAndTimestampById(regionId);

        // Assert
        Assert.AreEqual("World-With-Special-Name", regionName);
        Assert.AreEqual("12345-06-15", timestamp);
    }

    [TestMethod]
    public void ExtractIdFromFilePath_WithHyphenatedWorldName_ShouldParseCorrectly()
    {
        // Arrange - Dwarf Fortress uses 5-digit years (padded with leading zeros)
        // Issue: Dwarf Fortress with DFHack allows hyphenated world names
        // Format: WorldName_Year-Month-Day-legends.xml (Year is always 5 digits, e.g., 00005, 01253)
        var testCases = new (string filePath, string expectedId)[]
        {
            ("/path/to/My-World_01253-12-31-legends.xml", "My-World_01253"),
            ("/path/to/My-Custom-World_01253-12-31-legends.xml", "My-Custom-World_01253"),
            ("/path/to/World-With-Many-Hyphens_02024-01-15-legends.xml", "World-With-Many-Hyphens_02024"),
            ("/path/to/RegularWorld_01253-12-31-legends.xml", "RegularWorld_01253"),
            ("/path/to/RegularWorld_01253-12-31-legends_plus.xml", "RegularWorld_01253"),
            ("/path/to/hyphenated-world_plus_02023-06-15-legends_plus.xml", "hyphenated-world_plus_02023"),
        };

        foreach (var (filePath, expectedId) in testCases)
        {
            // Act
            var result = BookmarkService.ExtractIdFromFilePath(filePath);

            // Assert
            Assert.AreEqual(expectedId, result, $"Failed for path: {filePath}");
        }
    }

    [TestMethod]
    public void ExtractIdFromFilePath_WithLegacyDashFormat_ShouldStillWork()
    {
        // Arrange - legacy format uses hyphen between world name and year
        // This is the DF convention: worldName-Year-Month-Day
        var filePath = "/path/to/world-00001-01-01-legends.xml";

        // Act
        var result = BookmarkService.ExtractIdFromFilePath(filePath);

        // Assert - should use underscore as separator: worldName_Year
        Assert.AreEqual("world_00001", result);
    }

    [TestMethod]
    public void GetRegionNameAndTimestampById_WithInvalidId_ShouldReturnEmpty()
    {
        // Arrange
        var regionId = "invalid";

        // Act
        var (regionName, timestamp) = BookmarkService.GetRegionNameAndTimestampById(regionId);

        // Assert
        Assert.AreEqual("", regionName);
        Assert.AreEqual("", timestamp);
    }

    [TestMethod]
    public void ReplaceLastOccurrence_WithMultipleOccurrences_ShouldReplaceOnlyLast()
    {
        // Arrange
        var source = "/path/world-00001-01-01/world-00001-01-01-legends.xml";
        var find = "00001-01-01";
        var replace = BookmarkService.TimestampPlaceholder;

        // Act
        var result = BookmarkService.ReplaceLastOccurrence(source, find, replace);

        // Assert
        Assert.AreEqual("/path/world-00001-01-01/world-{TIMESTAMP}-legends.xml", result);
    }

    [TestMethod]
    public void ReplaceLastOccurrence_WithNoOccurrence_ShouldReturnOriginal()
    {
        // Arrange
        var source = "/path/world-legends.xml";
        var find = "nonexistent";
        var replace = BookmarkService.TimestampPlaceholder;

        // Act
        var result = BookmarkService.ReplaceLastOccurrence(source, find, replace);

        // Assert
        Assert.AreEqual(source, result);
    }

    [TestMethod]
    public void Concurrent_AddBookmark_FromMultipleThreads_ShouldNotCorruptState()
    {
        // Arrange - thread-safety test: verify concurrent operations don't corrupt data
        var service = CreateService();
        var exceptions = new ConcurrentBag<Exception>();
        var barrier = new Barrier(10); // Sync 10 threads
        var successfulAdds = 0;

        // Act
        Parallel.For(0, 10, i =>
        {
            try
            {
                barrier.SignalAndWait(); // Wait for all threads to be ready
                var bookmark = CreateTestBookmark(
                    $"/path/world{i}-00001-01-01-legends.xml",
                    $"World{i}",
                    "00001-01-01"
                );
                service.AddBookmark(bookmark);
                Interlocked.Increment(ref successfulAdds);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        });

        // Assert - no exceptions should occur
        Assert.AreEqual(0, exceptions.Count, $"Thread-safety issues detected: {string.Join(", ", exceptions.Select(e => e.Message))}");
        Assert.AreEqual(10, successfulAdds, "All 10 bookmark additions should succeed");
    }

    [TestMethod]
    public void Concurrent_AddAndGet_FromMultipleThreads_ShouldHandleGracefully()
    {
        // Arrange - thread-safety test: mixed read/write operations
        var service = CreateService();
        var exceptions = new ConcurrentBag<Exception>();
        var successfulOps = 0;
        
        // Pre-populate some bookmarks
        for (int i = 0; i < 5; i++)
        {
            var bookmark = CreateTestBookmark(
                $"/path/world{i}-00001-01-01-legends.xml",
                $"World{i}",
                "00001-01-01"
            );
            service.AddBookmark(bookmark);
        }

        var barrier = new Barrier(20);

        // Act - mixed operations
        Parallel.For(0, 20, i =>
        {
            try
            {
                barrier.SignalAndWait();
                if (i % 2 == 0)
                {
                    // Add operations
                    var bookmark = CreateTestBookmark(
                        $"/path/newWorld{i}-00001-01-01-legends.xml",
                        $"NewWorld{i}",
                        "00001-01-01"
                    );
                    service.AddBookmark(bookmark);
                    Interlocked.Increment(ref successfulOps);
                }
                else
                {
                    // Read operations
                    var all = service.GetAll();
                    if (all.Count > 0)
                    {
                        var first = all.FirstOrDefault();
                        if (first != null)
                        {
                            service.GetBookmark(first.FilePath);
                        }
                    }
                    Interlocked.Increment(ref successfulOps);
                }
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        });

        // Assert
        Assert.AreEqual(0, exceptions.Count, $"Thread-safety issues: {string.Join("; ", exceptions.Select(e => e.Message))}");
    }

    [TestMethod]
    public void AddBookmark_WithSpecialCharactersInPath_ShouldPersistCorrectly()
    {
        // Arrange - path encoding test: special characters in file paths
        // Each world uses a unique year so they don't merge into the same bookmark
        var service = CreateService();
        
        var testCases = new (string path, string timestamp)[]
        {
            ("/path/with spaces/worldA-00001-01-01-legends.xml", "00001-01-01"),
            ("/path/with+plus/worldB-00002-02-02-legends.xml", "00002-02-02"),
            ("/path/with#hash/worldC-00003-03-03-legends.xml", "00003-03-03"),
            ("/path/with%percent/worldD-00004-04-04-legends.xml", "00004-04-04"),
            ("/path/with&amp;ampersand/worldE-00005-05-05-legends.xml", "00005-05-05"),
            ("/path/with$dollar/worldF-00006-06-06-legends.xml", "00006-06-06"),
        };

        // Act
        foreach (var (path, timestamp) in testCases)
        {
            var bookmark = CreateTestBookmark(path, "SpecialWorld", timestamp);
            var result = service.AddBookmark(bookmark);
            Assert.IsNotNull(result, $"Should add bookmark for path: {path}");
        }

        // Assert
        Assert.AreEqual(testCases.Length, service.GetAll().Count);
    }

    [TestMethod]
    public void ExtractIdFromFilePath_WithTimestampPlaceholder_ShouldResolveCorrectly()
    {
        // Arrange - simulates loading old bookmarks that have {TIMESTAMP} in FilePath
        var testCases = new (string filePath, string[] worldTimestamps, string expectedId)[]
        {
            // {TIMESTAMP} should be replaced with first timestamp from WorldTimestamps
            ("region3-{TIMESTAMP}-legends.xml", new[] { "00100-01-01" }, "region3_00100"),
            ("region8-{TIMESTAMP}-legends.xml", new[] { "00102-03-09" }, "region8_00102"),
            ("region9-{TIMESTAMP}-legends.xml", new[] { "00100-01-18", "00100-01-01" }, "region9_00100"),
            // Full path with {TIMESTAMP}
            ("/path/to/region3-{TIMESTAMP}-legends.xml", new[] { "00100-01-01" }, "region3_00100"),
            // Without worldTimestamps, {TIMESTAMP} remains and regex may not match properly
            // (this is expected behavior - timestamps should be provided for old bookmarks)
        };

        foreach (var (filePath, worldTimestamps, expectedId) in testCases)
        {
            // Act
            var result = BookmarkService.ExtractIdFromFilePath(filePath, worldTimestamps: worldTimestamps);

            // Assert
            Assert.AreEqual(expectedId, result, $"Failed for path: {filePath} with timestamps: {string.Join(",", worldTimestamps)}");
        }
    }

    [TestMethod]
    public void LoadBookmarksFromFile_WithTimestampPlaceholder_ShouldMigrateCorrectly()
    {
        // Arrange - create a bookmarks.json file with {TIMESTAMP} placeholder (old format)
        var testBookmarkFile = Path.Combine(_testFolder, "legacy_bookmarks.json");
        // WorldMapImage is base64 encoded - use empty bytes
        var emptyBase64 = Convert.ToBase64String(Array.Empty<byte>());
        var legacyJson = @"{
    ""region3-{TIMESTAMP}-legends.xml"": {
        ""FilePath"": ""region3-{TIMESTAMP}-legends.xml"",
        ""WorldName"": ""TestWorld"",
        ""WorldAlternativeName"": ""Alt World"",
        ""WorldRegionName"": ""region3"",
        ""WorldTimestamps"": [""00100-01-01""],
        ""WorldWidth"": 128,
        ""WorldHeight"": 128,
        ""WorldMapImage"": """ + emptyBase64 + @""",
        ""State"": 0,
        ""LoadedTimestamp"": null,
        ""LatestTimestamp"": ""00100-01-01""
    }
}";
        File.WriteAllText(testBookmarkFile, legacyJson);

        // Act - create a new service that loads the legacy file
        var service = new BookmarkService(testBookmarkFile);
        var all = service.GetAll();

        // Assert - should have loaded the bookmark with correct Id
        Assert.AreEqual(1, all.Count, "Should load the legacy bookmark");
        Assert.AreEqual("region3_00100", all[0].Id, "Id should be extracted correctly from {TIMESTAMP} path");
        Assert.AreEqual("TestWorld", all[0].WorldName);
        Assert.IsTrue(all[0].WorldTimestamps.Contains("00100-01-01"));
    }

    [TestMethod]
    public void AddBookmark_AfterResetAll_ShouldResetOtherBookmarksState()
    {
        // Arrange
        var service = CreateService();
        var bookmark1 = CreateTestBookmark("/path/world1-00001-01-01-legends.xml", "World1", "00001-01-01");
        var bookmark2 = CreateTestBookmark("/path/world2-00002-02-02-legends.xml", "World2", "2-2-2");
        
        service.AddBookmark(bookmark1);
        bookmark1.State = BookmarkState.Loaded;
        service.AddBookmark(bookmark1); // This resets all, then sets bookmark1 back to Loaded
        
        // Act - add another bookmark, which triggers ResetAllBookmarks first
        service.AddBookmark(bookmark2);

        // Assert
        var all = service.GetAll();
        var world1 = all.FirstOrDefault(b => b.WorldName == "World1");
        var world2 = all.FirstOrDefault(b => b.WorldName == "World2");
        
        Assert.IsNotNull(world1);
        Assert.IsNotNull(world2);
        // World1 is reset to Default when adding world2 (due to ResetAllBookmarks)
        Assert.AreEqual(BookmarkState.Default, world1.State, "World1 state should be reset when adding new bookmark");
        // World2 was just added, so it's set to Loaded
        Assert.AreEqual(BookmarkState.Loaded, world2.State, "World2 should be in Loaded state");
    }

    private static Bookmark CreateTestBookmark(
        string filePath,
        string worldName,
        string timestamp,
        string[]? additionalTimestamps = null)
    {
        var timestamps = new List<string> { timestamp };
        if (additionalTimestamps != null)
        {
            timestamps.AddRange(additionalTimestamps);
        }

        return new Bookmark
        {
            // Use the actual file path directly. The real BookmarkService extracts
            // Id from FilePath, so we pass the real path.
            FilePath = filePath,
            WorldName = worldName,
            WorldAlternativeName = $"Alt {worldName}",
            WorldRegionName = "TestRegion",
            WorldTimestamps = timestamps,
            WorldWidth = 256,
            WorldHeight = 256,
            State = BookmarkState.Default,
            LoadedTimestamp = timestamp,
            LatestTimestamp = timestamp
        };
    }
}
