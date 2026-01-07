import { defineStore } from 'pinia';
import { ref, watch } from 'vue';
import { GoogleGenAI } from "@google/genai";
import { useBookmarkStore } from './bookmarkStore';

export interface AiSummary {
    id: number;
    type: string;
    worldName: string;
    summary: string;
    references?: Record<string, string>;
    timestamp: number;
}

export const useAiStore = defineStore('ai', () => {
    const bookmarkStore = useBookmarkStore();
    const allSummaries = ref<AiSummary[]>([]);
    const isGenerating = ref(false);
    const error = ref<string | null>(null);

    // Load from local storage
    const stored = localStorage.getItem('legends-ai-summaries');
    if (stored) {
        try {
            allSummaries.value = JSON.parse(stored);
        } catch (e) {
            console.error("Failed to parse AI summaries", e);
        }
    }

    // Save to local storage on change
    watch(allSummaries, (newVal) => {
        localStorage.setItem('legends-ai-summaries', JSON.stringify(newVal));
    }, { deep: true });

    const getSummary = (id: number, type: string): AiSummary | undefined => {
        if (!bookmarkStore.currentWorld?.worldName) return undefined;
        return allSummaries.value.find(s => s.id === id && s.type === type && s.worldName === bookmarkStore.currentWorld?.worldName);
    };

    const generateSummary = async (id: number, type: string, _name: string, prompt: string, availableReferences: Record<string, string>) => {
        if (!bookmarkStore.currentWorld?.worldName) {
            error.value = "World not loaded";
            return;
        }
        
        const apiKey = import.meta.env.VITE_GEMINI_API_KEY;
        if (!apiKey) {
            error.value = "Gemini API Key not configured (VITE_GEMINI_API_KEY)";
            return;
        }

        isGenerating.value = true;
        error.value = null;

        try {
            const client = new GoogleGenAI({ apiKey });
            
            // Using gemini-1.5-flash as a safe default for now.
            const response = await client.models.generateContent({
                model: "gemini-2.5-flash", 
                contents: prompt,
            });
            
            const summaryText = response.text; 

            if (summaryText) {
                // Filter references to only those used in the summary
                const usedReferences: Record<string, string> = {};
                const linkRegex = /\]\((.*?)\)/g;
                let match;
                while ((match = linkRegex.exec(summaryText)) !== null) {
                    const typeId = match[1];
                    if (availableReferences[typeId]) {
                        usedReferences[typeId] = availableReferences[typeId];
                    }
                }

                const newSummary: AiSummary = {
                    id,
                    type,
                    worldName: bookmarkStore.currentWorld.worldName,
                    summary: summaryText,
                    references: usedReferences,
                    timestamp: Date.now()
                };
                
                // Remove existing if any (to update)
                const index = allSummaries.value.findIndex(s => s.id === id && s.type === type && s.worldName === bookmarkStore.currentWorld!.worldName);
                if (index !== -1) {
                    allSummaries.value[index] = newSummary;
                } else {
                    allSummaries.value.push(newSummary);
                }
            } else {
                error.value = "No text returned from AI";
            }

        } catch (e: any) {
            console.error("AI Generation failed", e);
            error.value = e.message || "Failed to generate summary";
        } finally {
            isGenerating.value = false;
        }
    };

    return {
        getSummary,
        generateSummary,
        isGenerating,
        error
    };
});
