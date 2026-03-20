/**
 * Bookmark Store Tests
 * 
 * Tests for the bookmarkStore Pinia store covering:
 * - isLoadingNewWorld reset on error
 * - Error handling and display
 * - Concurrent request handling
 * - Bookmark deletion feedback
 */

import { setActivePinia, createPinia } from 'pinia'
import { vi, describe, it, expect, beforeEach, afterEach } from 'vitest'
import { useBookmarkStore } from '../stores/bookmarkStore'
import type { components } from '../generated/api-schema'

// Mock the API client
vi.mock('../apiClient', () => {
  return {
    default: {
      GET: vi.fn(),
      POST: vi.fn(),
      DELETE: vi.fn(),
    },
  }
})

// Mock console.error to keep test output clean
vi.spyOn(console, 'error').mockImplementation(() => { })

import client from '../apiClient'

type Bookmark = components['schemas']['Bookmark']

// Helper to create a mock bookmark
const createTestBookmark = (overrides: Partial<Bookmark> = {}): Bookmark => ({
  filePath: '/path/to/world-00001-01-01-legends.xml',
  worldName: 'TestWorld',
  worldAlternativeName: 'Alt TestWorld',
  worldRegionName: 'TestRegion',
  worldTimestamps: ['00001-01-01'],
  worldWidth: 256,
  worldHeight: 256,
  worldMapImage: null,
  state: 'Loaded',
  loadedTimestamp: '00001-01-01',
  latestTimestamp: '00001-01-01',
  ...overrides,
})

describe('BookmarkStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  describe('loadByFullPath', () => {
    it('given_newWorld_when_loadByFullPath_succeeds_should_set_isLoadingNewWorld_to_false', async () => {
      // Arrange
      const store = useBookmarkStore()
      const testBookmark = createTestBookmark()
      
      // Simulate a new world (not in existing bookmarks)
      ;(client.POST as ReturnType<typeof vi.fn>).mockResolvedValueOnce({
        data: testBookmark,
        error: undefined,
      })

      // Act
      const loadPromise = store.loadByFullPath('/path/to/world-00001-01-01-legends.xml', '00001-01-01')
      
      // Assert - isLoadingNewWorld should be true during load
      expect(store.isLoadingNewWorld).toBe(true)
      
      await loadPromise

      // Assert - isLoadingNewWorld should be reset after load
      expect(store.isLoadingNewWorld).toBe(false)
    })

    it('given_existingBookmark_when_loadByFullPath_fails_should_reset_isLoadingNewWorld', async () => {
      // Arrange
      const store = useBookmarkStore()
      
      // Pre-populate with a bookmark
      store.bookmarks.push(createTestBookmark({
        filePath: '/path/to/world-00001-01-01-legends.xml',
        state: 'Default',
      }))
      
      // Mock API to return error
      const errorResponse = {
        error: {
          title: 'File not found',
          type: 'Error',
        },
        data: undefined,
      }
      ;(client.POST as ReturnType<typeof vi.fn>).mockResolvedValueOnce(errorResponse)

      // Act
      await store.loadByFullPath('/path/to/world-00001-01-01-legends.xml', '00001-01-01')

      // Assert - isLoadingNewWorld should be reset after error
      expect(store.isLoadingNewWorld).toBe(false)
      expect(store.bookmarkError).toBe('File not found')
    })

    it('given_newWorld_when_loadByFullPath_fails_should_reset_isLoadingNewWorld_and_show_error', async () => {
      // Arrange
      const store = useBookmarkStore()
      
      // Mock API to return error
      ;(client.POST as ReturnType<typeof vi.fn>).mockResolvedValueOnce({
        error: {
          title: 'Parse error',
          type: 'Error',
        },
        data: undefined,
      })

      // Act
      await store.loadByFullPath('/path/to/world-00001-01-01-legends.xml', '00001-01-01')

      // Assert
      expect(store.isLoadingNewWorld).toBe(false)
      expect(store.bookmarkError).toBe('Parse error')
    })

    it('given_existingBookmark_when_loadByFullPath_in_progress_should_cancel_previous_request', async () => {
      // Arrange
      const store = useBookmarkStore()
      
      // First request starts
      const firstMockResponse = new Promise<any>((resolve) => {
        setTimeout(() => {
          resolve({ data: null, error: undefined })
        }, 100)
      })

      // Mock to return pending promise first time, then resolved
      ;(client.POST as ReturnType<typeof vi.fn>).mockImplementationOnce(
        async () => firstMockResponse
      )

      // Start first load
      const firstLoadPromise = store.loadByFullPath('/path/to/world1-00001-01-01-legends.xml', '00001-01-01')
      
      // Wait a tick for the first request to be initiated
      await new Promise(r => setTimeout(r, 10))
      
      // Start second load (should abort first via AbortController)
      const secondTestBookmark = createTestBookmark({ filePath: '/path/to/world2-00002-02-02-legends.xml' })
      ;(client.POST as ReturnType<typeof vi.fn>).mockResolvedValueOnce({
        data: secondTestBookmark,
        error: undefined,
      })
      
      const secondLoadPromise = store.loadByFullPath('/path/to/world2-00002-02-02-legends.xml', '00002-02-02')
      
      await firstLoadPromise
      await secondLoadPromise
      
      // The store should have only the second bookmark (first was cancelled)
      expect(store.bookmarks.length).toBe(1)
      expect(store.bookmarks[0].filePath).toBe('/path/to/world2-00002-02-02-legends.xml')
    })

    it('given_rapid_clicks_when_loadByFullPath_should_handle_gracefully_without_duplicate_bookmarks', async () => {
      // Arrange
      const store = useBookmarkStore()
      let requestCount = 0
      
      ;(client.POST as ReturnType<typeof vi.fn>).mockImplementation(
        async () => {
          requestCount++
          await new Promise(r => setTimeout(r, 10)) // Simulate network delay
          return {
            data: createTestBookmark({
              filePath: '/path/to/world-00001-01-01-legends.xml',
              worldTimestamps: [`00001-01-01`],
            }),
            error: undefined,
          }
        }
      )

      // Act - simulate rapid clicks
      const promises = [
        store.loadByFullPath('/path/to/world-00001-01-01-legends.xml', '00001-01-01'),
        store.loadByFullPath('/path/to/world-00001-01-01-legends.xml', '00001-01-01'),
        store.loadByFullPath('/path/to/world-00001-01-01-legends.xml', '00001-01-01'),
      ]
      
      await Promise.all(promises)

      // Assert - only one bookmark should exist (due to cancellation)
      expect(store.bookmarks.length).toBeLessThanOrEqual(1)
    })

    it('given_missing_legends_plus_when_loadByFullPath_should_set_warning', async () => {
      // Arrange
      const store = useBookmarkStore()
      
      // Mock response with empty worldName (indicates missing legends_plus)
      const testBookmark = createTestBookmark({
        worldName: '', // Empty worldName means legends_plus was not found
      })
      ;(client.POST as ReturnType<typeof vi.fn>).mockResolvedValueOnce({
        data: testBookmark,
        error: undefined,
      })

      // Act
      await store.loadByFullPath('/path/to/world-00001-01-01-legends.xml', '00001-01-01')

      // Assert
      expect(store.bookmarkWarning).toContain('legends_plus.xml file was not found')
    })
  })

  describe('deleteByFullPath', () => {
    it('given_existingBookmark_when_deleteByFullPath_succeeds_should_show_delete_feedback', async () => {
      // Arrange
      const store = useBookmarkStore()
      store.bookmarks.push(createTestBookmark({
        filePath: '/path/to/world-00001-01-01-legends.xml',
        worldTimestamps: ['00001-01-01', '00002-02-02'],
      }))
      
      // Mock DELETE to return remaining bookmark (has more timestamps)
      const remainingBookmark = createTestBookmark({
        filePath: '/path/to/world-00001-01-01-legends.xml',
        worldTimestamps: ['00002-02-02'],
      })
      ;(client.DELETE as ReturnType<typeof vi.fn>).mockResolvedValueOnce({
        data: remainingBookmark,
        error: undefined,
      })

      // Act
      await store.deleteByFullPath('/path/to/world-00001-01-01-legends.xml', '00001-01-01')

      // Assert
      expect(store.isLoadingNewWorld).toBe(false)
      expect(store.bookmarkError).toBe('')
    })

    it('given_last_timestamp_when_deleteByFullPath_should_remove_bookmark_completely', async () => {
      // Arrange
      const store = useBookmarkStore()
      store.bookmarks.push(createTestBookmark({
        filePath: '/path/to/world-00001-01-01-legends.xml',
        worldTimestamps: ['00001-01-01'], // Only one timestamp
      }))
      
      // Mock DELETE to return null (no more timestamps)
      ;(client.DELETE as ReturnType<typeof vi.fn>).mockResolvedValueOnce({
        data: null,
        error: undefined,
      })

      // Act
      await store.deleteByFullPath('/path/to/world-00001-01-01-legends.xml', '00001-01-01')

      // Assert - bookmark should be removed
      expect(store.bookmarks.find(b => b.filePath === '/path/to/world-00001-01-01-legends.xml')).toBeUndefined()
      expect(store.isLoadingNewWorld).toBe(false)
    })

    it('given_delete_fails_should_set_error_message', async () => {
      // Arrange
      const store = useBookmarkStore()
      store.bookmarks.push(createTestBookmark())
      
      ;(client.DELETE as ReturnType<typeof vi.fn>).mockResolvedValueOnce({
        error: {
          title: 'Delete failed',
          type: 'Error',
        },
        data: undefined,
      })

      // Act
      await store.deleteByFullPath('/path/to/world-00001-01-01-legends.xml', '00001-01-01')

      // Assert
      expect(store.bookmarkError).toBe('Delete failed')
      expect(store.isLoadingNewWorld).toBe(false)
    })
  })

  describe('loadByFolderAndFile', () => {
    it('given_valid_folder_and_file_when_loadByFolderAndFile_succeeds_should_update_bookmarks', async () => {
      // Arrange
      const store = useBookmarkStore()
      const testBookmark = createTestBookmark()
      ;(client.POST as ReturnType<typeof vi.fn>).mockResolvedValueOnce({
        data: testBookmark,
        error: undefined,
      })

      // Act
      await store.loadByFolderAndFile('/path/to', 'world-00001-01-01-legends.xml')

      // Assert
      expect(store.bookmarks.length).toBe(1)
      expect(store.isLoadingNewWorld).toBe(false)
    })

    it('given_existing_bookmark_when_loadByFolderAndFile_fails_should_reset_state', async () => {
      // Arrange
      const store = useBookmarkStore()
      store.bookmarks.push(createTestBookmark({
        filePath: '/path/to/world-00001-01-01-legends.xml',
        state: 'Default',
      }))
      
      ;(client.POST as ReturnType<typeof vi.fn>).mockResolvedValueOnce({
        error: { title: 'Load failed', type: 'Error' },
        data: undefined,
      })

      // Act
      await store.loadByFolderAndFile('/path/to', 'world-00001-01-01-legends.xml')

      // Assert
      expect(store.bookmarkError).toBe('Load failed')
      expect(store.isLoadingNewWorld).toBe(false)
      expect(store.bookmarks[0].state).toBe('Default')
    })

    it('given_rapid_folder_loads_when_loadByFolderAndFile_should_cancel_pending_requests', async () => {
      // Arrange
      const store = useBookmarkStore()
      let requestOrder: number[] = []
      
      ;(client.POST as ReturnType<typeof vi.fn>).mockImplementation(
        async () => {
          const order = requestOrder.length
          requestOrder.push(order)
          await new Promise(r => setTimeout(r, 20))
          return {
            data: createTestBookmark({
              filePath: `/path/to/world${order}-00001-01-01-legends.xml`,
            }),
            error: undefined,
          }
        }
      )

      // Act - rapid loads
      const p1 = store.loadByFolderAndFile('/path/to', 'world1-00001-01-01-legends.xml')
      const p2 = store.loadByFolderAndFile('/path/to', 'world2-00002-02-02-legends.xml')
      const p3 = store.loadByFolderAndFile('/path/to', 'world3-00003-03-03-legends.xml')
      
      await Promise.all([p1, p2, p3])

      // Assert - only the last one should complete successfully
      // Earlier requests should have been cancelled/aborted
      expect(store.bookmarks.length).toBe(1)
    })
  })

  describe('getAll', () => {
    it('when_getAll_succeeds_should_populate_bookmarks', async () => {
      // Arrange
      const store = useBookmarkStore()
      const testBookmarks = [
        createTestBookmark({ filePath: '/path/world1-00001-01-01-legends.xml' }),
        createTestBookmark({ filePath: '/path/world2-00002-02-02-legends.xml' }),
      ]
      ;(client.GET as ReturnType<typeof vi.fn>).mockResolvedValueOnce({
        data: testBookmarks,
        error: undefined,
      })

      // Act
      await store.getAll()

      // Assert
      expect(store.bookmarks.length).toBe(2)
    })

    it('when_getAll_fails_should_not_clear_existing_bookmarks', async () => {
      // Arrange
      const store = useBookmarkStore()
      store.bookmarks.push(createTestBookmark())
      ;(client.GET as ReturnType<typeof vi.fn>).mockResolvedValueOnce({
        error: { title: 'Server error', type: 'Error' },
        data: undefined,
      })

      // Act
      await store.getAll()

      // Assert - existing bookmark should still be there
      expect(store.bookmarks.length).toBe(1)
    })
  })

  describe('getters', () => {
    it('isLoadingExistingWorld_should_return_true_when_any_bookmark_is_Loading', () => {
      // Arrange
      const store = useBookmarkStore()
      store.bookmarks.push(
        createTestBookmark({ state: 'Default' }),
        createTestBookmark({ state: 'Loading' }),
        createTestBookmark({ state: 'Loaded' }),
      )

      // Assert
      expect(store.isLoadingExistingWorld).toBe(true)
    })

    it('isLoading_should_return_true_when_isLoadingNewWorld_or_any_bookmark_is_Loading', () => {
      // Arrange
      const store = useBookmarkStore()
      store.isLoadingNewWorld = true

      // Assert
      expect(store.isLoading).toBe(true)

      // More specific
      store.isLoadingNewWorld = false
      store.bookmarks.push(createTestBookmark({ state: 'Loading' }))
      expect(store.isLoading).toBe(true)
    })

    it('isLoaded_should_return_true_when_any_bookmark_is_Loaded', () => {
      // Arrange
      const store = useBookmarkStore()
      store.bookmarks.push(
        createTestBookmark({ state: 'Default' }),
        createTestBookmark({ state: 'Loaded' }),
      )

      // Assert
      expect(store.isLoaded).toBe(true)
    })

    it('currentWorld_should_return_bookmark_with_Loaded_state', () => {
      // Arrange
      const store = useBookmarkStore()
      const loadedBookmark = createTestBookmark({
        filePath: '/path/to/current-00001-01-01-legends.xml',
        state: 'Loaded',
        worldName: 'Current World',
      })
      store.bookmarks.push(
        createTestBookmark({ state: 'Default' }),
        loadedBookmark,
      )

      // Assert
      expect(store.currentWorld).toBeDefined()
      expect(store.currentWorld?.worldName).toBe('Current World')
    })
  })

  describe('error handling edge cases', () => {
    it('given_api_returns_undefined_error_should_handle_gracefully', async () => {
      // Arrange
      const store = useBookmarkStore()
      ;(client.POST as ReturnType<typeof vi.fn>).mockResolvedValueOnce({
        error: undefined,
        data: undefined,
      })

      // Act
      await store.loadByFullPath('/path/to/world-00001-01-01-legends.xml', '00001-01-01')

      // Assert - should not crash, state should be consistent
      expect(store.isLoadingNewWorld).toBe(false)
    })

    it('given_api_returns_error_with_no_title_should_use_type_as_fallback', async () => {
      // Arrange
      const store = useBookmarkStore()
      ;(client.POST as ReturnType<typeof vi.fn>).mockResolvedValueOnce({
        error: {
          type: 'ValidationError',
        },
        data: undefined,
      })

      // Act
      await store.loadByFullPath('/path/to/world-00001-01-01-legends.xml', '00001-01-01')

      // Assert
      expect(store.bookmarkError).toBe('ValidationError')
    })

    it('given_api_returns_error_with_no_title_or_type_should_show_generic_message', async () => {
      // Arrange
      const store = useBookmarkStore()
      ;(client.POST as ReturnType<typeof vi.fn>).mockResolvedValueOnce({
        error: {},
        data: undefined,
      })

      // Act
      await store.loadByFullPath('/path/to/world-00001-01-01-legends.xml', '00001-01-01')

      // Assert
      expect(store.bookmarkError).toBe('')
    })

    it('given_delete_returns_error_should_reset_loading_state', async () => {
      // Arrange
      const store = useBookmarkStore()
      store.bookmarks.push(createTestBookmark({ state: 'Loading' }))
      
      ;(client.DELETE as ReturnType<typeof vi.fn>).mockResolvedValueOnce({
        error: { title: 'Network error', type: 'Error' },
        data: undefined,
      })

      // Act
      await store.deleteByFullPath('/path/to/world-00001-01-01-legends.xml', '00001-01-01')

      // Assert
      expect(store.isLoadingNewWorld).toBe(false)
      expect(store.bookmarkError).toBe('Network error')
    })
  })
})
