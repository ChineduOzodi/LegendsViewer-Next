import { defineStore } from 'pinia';
import { ref, watch } from 'vue';

export interface Favorite {
    id: number;
    type: string;
    name: string;
    icon?: string;
}

export const useFavoriteStore = defineStore('favorite', () => {
    const favorites = ref<Favorite[]>([]);

    const stored = localStorage.getItem('legends-favorites');
    if (stored) {
        try {
            favorites.value = JSON.parse(stored);
        } catch (e) {
            console.error("Failed to parse favorites", e);
        }
    }

    watch(favorites, (newVal) => {
        localStorage.setItem('legends-favorites', JSON.stringify(newVal));
    }, { deep: true });

    function isFavorite(id: number, type: string): boolean {
        return favorites.value.some(f => f.id === id && f.type === type);
    }

    function toggleFavorite(item: Favorite) {
        if (isFavorite(item.id, item.type)) {
            favorites.value = favorites.value.filter(f => !(f.id === item.id && f.type === item.type));
        } else {
            favorites.value.push(item);
        }
    }

    return {
        favorites,
        isFavorite,
        toggleFavorite
    };
});