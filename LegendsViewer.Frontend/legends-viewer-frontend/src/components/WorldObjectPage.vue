<template>
    <v-fab class="me-2" icon="mdi-chevron-right" location="top end" absolute
        :to="'/' + objectType + '/' + (store.object?.nextId ?? routeId + 1)">
    </v-fab>
    <v-fab class="me-16" icon="mdi-chevron-left" location="top end" absolute
        :to="'/' + objectType + '/' + (store.object?.previousId ?? routeId - 1)">
    </v-fab>
    <v-row>
        <v-col cols="12">
            <v-card variant="text">
                <v-row align="center" no-gutters>
                    <v-col class="large-icon" cols="auto">
                        <div v-html="store.object?.icon"></div>
                    </v-col>
                    <v-col>
                        <v-card-title class="d-flex align-center">
                            {{ store.object?.name ?? '' }}
                            <v-btn
                                :icon="favoriteStore.isFavorite(store.object?.id ?? -1, objectType ?? '') ? 'mdi-star' : 'mdi-star-outline'"
                                :color="favoriteStore.isFavorite(store.object?.id ?? -1, objectType ?? '') ? 'yellow-darken-2' : undefined"
                                variant="text"
                                density="compact"
                                class="ml-2"
                                :disabled="!store.object"
                                @click="favoriteStore.toggleFavorite({
                                    id: store.object?.id ?? -1,
                                    type: objectType ?? '',
                                    name: store.object?.name ?? 'Unknown',
                                    icon: store.object?.icon
                                })"
                            ></v-btn>
                        </v-card-title>
                        <v-card-subtitle class="multiline-subtitle">
                            {{ store.object?.type ?? '' }}
                        </v-card-subtitle>
                    </v-col>
                </v-row>
            </v-card>
        </v-col>
    </v-row>
    <v-row v-if="store.object">
        <v-col cols="12">
             <v-card variant="text" class="mb-4">
                <v-card-title class="d-flex align-center">
                    <v-icon icon="mdi-magic-staff" class="mr-2"></v-icon>
                    Story
                    <v-spacer></v-spacer>
                    <v-btn 
                        v-if="!currentSummary"
                        prepend-icon="mdi-creation"
                        variant="text"
                        :loading="aiStore.isGenerating"
                        @click="generateAiSummary"
                    >
                        Generate Story
                    </v-btn>
                     <v-btn 
                        v-else
                        icon="mdi-refresh"
                        variant="text"
                        size="small"
                        :loading="aiStore.isGenerating"
                        @click="generateAiSummary"
                    >
                    </v-btn>
                </v-card-title>
                <v-card-text v-if="currentSummary" class="text-body-1 font-italic">
                    <div style="white-space: pre-wrap;">
                        <template v-for="(part, index) in parseSummary(currentSummary)" :key="index">
                            <span v-if="part.type === 'icon'" v-html="part.content" class="mr-1"></span>
                            <router-link v-else-if="part.type === 'link'" :to="'/' + part.link" class="text-decoration-none font-weight-bold text-primary">
                                {{ part.text }}
                            </router-link>
                            <span v-else>{{ part.content }}</span>
                        </template>
                    </div>
                </v-card-text>
                 <v-alert v-if="aiStore.error" type="error" variant="tonal" closable class="mt-2">
                    {{ aiStore.error }}
                </v-alert>
            </v-card>
        </v-col>
    </v-row>
    <v-row>
        <v-col v-if="mapStore?.currentWorldObjectMap" cols="12" xl="4" lg="6" md="12">
            <!-- Location on World Map -->
            <v-card title="Location" :subtitle="'The location of ' + store.object?.name + ' on the world map'"
                height="400" variant="text" to="/map">
                <template v-slot:prepend>
                    <v-icon class="mr-2" icon="mdi-map-search-outline" size="32px"></v-icon>
                </template>
                <v-card-text>
                    <v-img width="320" height="320" class="position-relative ml-12 pixelated-image"
                        :src="mapStore.currentWorldObjectMap" :cover="false" />
                </v-card-text>
            </v-card>
        </v-col>
        <slot name="type-specific-before-table"></slot>
    </v-row>
    <v-row>
        <v-col v-if="store.object?.eventCount != null && store.object?.eventCount > 0">
            <ExpandableCard title="Events" :subtitle="'An overview of events for ' + store.object?.name"
                icon="mdi-calendar-clock" :height="'auto'">
                <template #compact-content>
                    <div class="ml-12">
                        <LineChart v-if="store.objectEventChartData != null"
                            :chart-data="store.objectEventChartData" />
                        <v-data-table-server :key="store.object.id"
                            v-model:items-per-page="store.objectEventsPerPage" :headers="eventTableHeaders"
                            :items="store.objectEvents" :items-length="store.objectEventsTotalItems"
                            :loading="store.isLoading" item-value="id"
                            :items-per-page-options="store.itemsPerPageOptions" @update:options="loadEvents">
                            <template v-slot:item.html="{ value }">
                                <span v-html="value"></span>
                            </template>
                        </v-data-table-server>
                    </div>
                </template>
                <template #expanded-content>
                    <BarChart v-if="store.objectEventTypeChartData != null"
                        :chart-data="store.objectEventTypeChartData" />
                </template>
            </ExpandableCard>
        </v-col>
    </v-row>
    <v-row>
        <v-col v-if="store.object?.eventCollectionCount != null && store.object?.eventCollectionCount > 0">
            <v-card title="Chronicles" :subtitle="'A list of chronicles for ' + store.object?.name" variant="text">
                <template v-slot:prepend>
                    <v-icon class="mr-2" icon="mdi-calendar-clock" size="32px"></v-icon>
                </template>
                <v-card-text class="ml-12">
                    <v-data-table-server :key="store.object.id"
                        v-model:items-per-page="store.objectEventCollectionsPerPage"
                        :headers="eventCollectionTableHeaders" :items="store.objectEventCollections"
                        :items-length="store.objectEventCollectionsTotalItems" :loading="store.isLoading"
                        item-value="id" :items-per-page-options="store.itemsPerPageOptions"
                        @update:options="loadEventCollections">
                        <template v-slot:item.subtype="{ value }">
                            <span v-html="value"></span>
                        </template>
                        <template v-slot:item.html="{ value }">
                            <span v-html="value"></span>
                        </template>
                    </v-data-table-server>
                </v-card-text>
            </v-card>
        </v-col>
    </v-row>
    <v-row>
        <slot name="type-specific-after-table"></slot>
    </v-row>
</template>

<script setup lang="ts">
import { computed, watch } from 'vue';
import { useRoute } from 'vue-router';
import { LoadItemsOptions, LoadItemsSortOption, TableHeader } from '../types/legends';
import LineChart from '../components/LineChart.vue';
import ExpandableCard from '../components/ExpandableCard.vue';
import BarChart from './BarChart.vue';
import { useFavoriteStore } from '../stores/favoriteStore';
import { useAiStore, AiSummary } from '../stores/aiStore';
import { generatePrompt, extractReferences } from '../utils/aiPrompts';

const favoriteStore = useFavoriteStore();
const aiStore = useAiStore();

const route = useRoute()
const routeId = computed(() => {
    if (typeof route.params.id === 'string') {
        return parseInt(route.params.id, 10)
    }
    return 0;
});

const loadEvents = async ({ page, itemsPerPage, sortBy }: LoadItemsOptions) => {
    await props.store.loadEvents(routeId.value, page, itemsPerPage, sortBy)
}

const loadEventCollections = async ({ page, itemsPerPage, sortBy }: LoadItemsOptions) => {
    await props.store.loadEventCollections(routeId.value, page, itemsPerPage, sortBy)
}

const eventSortBy: LoadItemsSortOption[] = [{ key: 'date', order: 'asc' }]

const eventTableHeaders: TableHeader[] = [
    { title: 'Date', key: 'date' },
    { title: 'Type', key: 'type' },
    { title: 'Event', key: 'html', sortable: false },
]

const eventCollectionTableHeaders: TableHeader[] = [
    { title: 'Start', key: 'startDate', align: 'center' },
    { title: 'End', key: 'endDate', align: 'center' },
    { title: 'Name', key: 'html', align: 'start', sortable: false },
    { title: 'Type', key: 'type', align: 'start' },
    { title: 'Subtype', key: 'subtype', align: 'start' },
    { title: 'Chronicles', key: 'eventCollectionCount', align: 'end' },
    { title: 'Events', key: 'eventCount', align: 'end' },
]

const props = defineProps({
    store: {
        type: Object,
        required: true,
    },
    mapStore: {
        type: Object,
        required: false,
    },
    objectType: {
        type: String,
        required: true,
    },
});

const currentSummary = computed(() => aiStore.getSummary(props.store.object?.id ?? -1, props.objectType));

const parseSummary = (summaryObj: AiSummary) => {
    const text = summaryObj.summary;
    const references = summaryObj.references || {};
    const parts = [];
    const regex = /\[(.*?)\]\((.*?)\)/g;
    let lastIndex = 0;
    let match;

    while ((match = regex.exec(text)) !== null) {
        if (match.index > lastIndex) {
            parts.push({ type: 'text', content: text.substring(lastIndex, match.index) });
        }
        
        const linkText = match[1];
        const linkUrl = match[2];
        
        // Check if we have an icon for this link
        if (references[linkUrl]) {
             parts.push({ type: 'icon', content: references[linkUrl] });
        }
        
        parts.push({ type: 'link', text: linkText, link: linkUrl });
        
        lastIndex = regex.lastIndex;
    }

    if (lastIndex < text.length) {
        parts.push({ type: 'text', content: text.substring(lastIndex) });
    }
    return parts;
};

const generateAiSummary = async () => {
    if (!props.store.object) return;
    
    const prompt = generatePrompt(
        props.store.object.name,
        props.objectType,
        props.store.objectEvents,
        props.store.objectEventCollections,
        props.store.object
    );

    const references = extractReferences(
        props.store.objectEvents,
        props.store.objectEventCollections,
        props.store.object
    );
    
    await aiStore.generateSummary(
        props.store.object.id,
        props.objectType,
        props.store.object.name,
        prompt,
        references
    );
}

const load = async (idString: string | string[]) => {
    if (typeof idString === 'string') {
        const id = parseInt(idString, 10)
        await props.store.load(id)
        await props.mapStore?.loadWorldObjectMap(id, 'Default')
        await props.store.loadEventChartData(id)
        await props.store.loadEventTypeChartData(id)
        await loadEvents({ page: 1, itemsPerPage: props.store.objectEventsPerPage, sortBy: eventSortBy })
    }
}

load(route.params.id)


watch(
    () => route.params.id,
    load
)

</script>

<style scoped></style>
