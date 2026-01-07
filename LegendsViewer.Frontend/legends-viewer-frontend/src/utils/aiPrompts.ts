export const generatePrompt = (
    name: string,
    type: string,
    events: any[],
    collections: any[],
    objectData?: any
): string => {
    let prompt = `Write a short, engaging summary (max 2 paragraphs) for the ${type} named "${name}" in a fantasy world (Dwarf Fortress). The style should be like a Lord of the Rings story.\n\n`;
    
    prompt += `IMPORTANT: When mentioning other entities, sites, or historical figures:\n`;
    prompt += `- If you know their ID, use the format [Name](Type/ID).\n`;
    prompt += `- Do NOT include icons in the text. Just the link format.\n`;
    prompt += `- Example: [Urist](HistoricalFigure/123).\n\n`;
    
    prompt += `Here is some context about ${name}:\n`;

    if (objectData) {
        // Basic Info
        if (objectData.description) {
            prompt += `- Description: ${convertHtmlToMarkdown(objectData.description)}\n`;
        }
        if (objectData.race?.nameSingular) {
            prompt += `- Race: ${objectData.race.nameSingular}\n`;
        }
        if (objectData.caste) {
            prompt += `- Caste: ${objectData.caste}\n`;
        }
        if (objectData.age != null && objectData.age >= 0) {
            prompt += `- Age: ${objectData.age}\n`;
        }
        if (objectData.birthYear != null) {
            prompt += `- Born: ${objectData.birthYear}\n`;
        }
        if (objectData.deathYear != null && objectData.deathYear > -1) {
            prompt += `- Died: ${objectData.deathYear}\n`;
        }
        if (objectData.goal) {
            prompt += `- Goal: ${objectData.goal}\n`;
        }

        // Skills
        if (objectData.skillDescriptions && objectData.skillDescriptions.length > 0) {
            const skills = objectData.skillDescriptions
                .filter((s: any) => s.level > 0 || s.rank) // Assuming some way to filter relevant skills, or just take top
                .slice(0, 10)
                .map((s: any) => `${s.name} (${s.rank})`)
                .join(', ');
            if (skills) {
                prompt += `- Notable Skills: ${skills}\n`;
            }
        }

        // Spheres
        if (objectData.spheres && objectData.spheres.length > 0) {
            prompt += `- Spheres: ${objectData.spheres.join(', ')}\n`;
        }

        // Worshipped Deities
        if (objectData.worshippedDeities && objectData.worshippedDeities.length > 0) {
            const deities = objectData.worshippedDeities.map((d: any) => convertListItemToMarkdown(d)).join(', ');
            prompt += `- Worshipped Deities: ${deities}\n`;
        }
        // Entity Worship
        if (objectData.worshippedLinks && objectData.worshippedLinks.length > 0) {
             const deities = objectData.worshippedLinks.map((d: any) => convertListItemToMarkdown(d)).join(', ');
             prompt += `- Worshipped Deities: ${deities}\n`;
        }

        // Relationships (HF)
        if (objectData.relatedHistoricalFigureList && objectData.relatedHistoricalFigureList.length > 0) {
            const rels = objectData.relatedHistoricalFigureList.slice(0, 10).map((r: any) => convertListItemToMarkdown(r)).join(', ');
            prompt += `- Close Relationships: ${rels}\n`;
        }
        
        // Related Entities (HF)
        if (objectData.relatedEntityList && objectData.relatedEntityList.length > 0) {
             const rels = objectData.relatedEntityList.slice(0, 10).map((r: any) => convertListItemToMarkdown(r)).join(', ');
             prompt += `- Related Factions/Groups: ${rels}\n`;
        }

        // Current Leader (Entity)
        if (objectData.currentLeader) {
            prompt += `- Current Leader: ${convertListItemToMarkdown(objectData.currentLeader)}\n`;
        }
        
        prompt += `\n`;
    }
    
    if (events && events.length > 0) {
        prompt += `Major Events:\n`;
        events.slice(0, 50).forEach(e => {
             prompt += `- [${e.date}] ${convertHtmlToMarkdown(e.html)}\n`;
        });
    }
    
    if (collections && collections.length > 0) {
        prompt += `\nChronicles:\n`;
        collections.slice(0, 20).forEach(c => {
            prompt += `- ${convertHtmlToMarkdown(c.html)} (${c.type})\n`;
        });
    }
    
    return prompt;
}

function convertHtmlToMarkdown(html: string) {
   if (!html) return "";
   
   // Remove icons from context to save tokens and avoid confusion, 
   // since we will reconstruct them from the original data later.
   let text = html.replace(/<i\s+[^>]*><\/i>/g, '');
   text = text.replace(/<span\s+[^>]*class="[^"]*soc[^"]*"[^>]*>.*?<\/span>/g, '');

   // Replace <a href="/type/id" ...>text</a> with [text](type/id)
   text = text.replace(/<a\s+href="\/([^"]+)"[^>]*>(.*?)<\/a>/g, '[$2]($1)');
   // Remove other tags
   text = text.replace(/<[^>]*>?/gm, '');
   return text;
}

function convertListItemToMarkdown(item: any) {
    if (!item) return "";
    // ListItemDto usually has title with HTML link
    let text = item.title || "";
    if (item.subtitle) {
        text += ` (${item.subtitle})`;
    }
    return convertHtmlToMarkdown(text);
}

export const extractReferences = (events: any[], collections: any[], objectData?: any): Record<string, string> => {
    const references: Record<string, string> = {};
    const allItems = [...(events || []), ...(collections || [])];
    
    // Regex to capture icon (<i> or <span class="soc">) followed by <a href="/Type/ID">
    // We capture the icon HTML group 1, and the Type/ID group 2.
    const regex = /(<i\s+[^>]*class="[^"]*mdi-[^"]*"[^>]*><\/i>|<span\s+[^>]*class="[^"]*soc[^"]*"[^>]*>.*?<\/span>)\s*<a\s+href="\/([^"]+)"/g;

    const extractFromHtml = (html: string) => {
        if (!html) return;
        let match;
        const itemRegex = new RegExp(regex); 
        while ((match = itemRegex.exec(html)) !== null) {
            const iconHtml = match[1];
            const typeId = match[2];
            references[typeId] = iconHtml;
        }
    };

    allItems.forEach(item => {
        if (item.html) extractFromHtml(item.html);
    });

    if (objectData) {
        if (objectData.description) {
            extractFromHtml(objectData.description);
        }

        const listsToScan = [
            objectData.relatedHistoricalFigureList,
            objectData.relatedEntityList,
            objectData.relatedSiteList,
            objectData.worshippedDeities,
            objectData.worshippedLinks,
            objectData.vagueRelationshipList,
            objectData.miscList,
            objectData.notableKillList,
            objectData.currentSiteList,
            objectData.lostSiteList
        ];

        listsToScan.forEach(list => {
            if (Array.isArray(list)) {
                list.forEach(item => {
                    if (item.prepend) extractFromHtml(item.prepend + (item.title || ""));
                    else if (item.title) extractFromHtml(item.title);
                });
            }
        });

        if (objectData.currentLeader) {
             if (objectData.currentLeader.prepend) extractFromHtml(objectData.currentLeader.prepend + (objectData.currentLeader.title || ""));
             else if (objectData.currentLeader.title) extractFromHtml(objectData.currentLeader.title);
        }
    }

    return references;
}
