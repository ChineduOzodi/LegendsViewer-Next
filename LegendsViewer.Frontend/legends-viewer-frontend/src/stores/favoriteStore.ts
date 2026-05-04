import { defineStore } from 'pinia';
import { ref, watch, computed } from 'vue';
import { useBookmarkStore } from './bookmarkStore';

export interface Favorite {
    id: number;
    type: string;
    name: string;
    icon?: string;
    worldName: string;
}

export const useFavoriteStore = defineStore('favorite', () => {
    const bookmarkStore = useBookmarkStore();
    const allFavorites = ref<Favorite[]>([]);

    const stored = localStorage.getItem('legends-favorites');
    if (stored) {
        try {
            allFavorites.value = JSON.parse(stored);
        } catch (e) {
            console.error("Failed to parse favorites", e);
        }
    }

    watch(allFavorites, (newVal) => {
        localStorage.setItem('legends-favorites', JSON.stringify(newVal));
    }, { deep: true });

    const favorites = computed(() => {
        if (!bookmarkStore.currentWorld?.worldName) return [];
        return allFavorites.value.filter(f => f.worldName === bookmarkStore.currentWorld?.worldName);
    });

    function isFavorite(id: number, type: string): boolean {
        if (!bookmarkStore.currentWorld?.worldName) return false;
        return allFavorites.value.some(f => f.id === id && f.type === type && f.worldName === bookmarkStore.currentWorld?.worldName);
    }

    function toggleFavorite(item: Omit<Favorite, 'worldName'>) {
        if (!bookmarkStore.currentWorld?.worldName) {
            console.warn("Cannot toggle favorite: World name not loaded");
            return;
        }
        const worldName = bookmarkStore.currentWorld.worldName;

        if (isFavorite(item.id, item.type)) {
            allFavorites.value = allFavorites.value.filter(f => !(f.id === item.id && f.type === item.type && f.worldName === worldName));
        } else {
            allFavorites.value.push({ ...item, worldName });
        }
    }

    return {
        favorites,
        isFavorite,
        toggleFavorite
    };
});