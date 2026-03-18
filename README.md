# Minority Narrative Engine Toolkit

**A Unity narrative engine built for cultural storytelling.**

Most game narrative tools were built with a narrow set of assumptions: individual protagonists, Western family structures, and "standard English" as the neutral default. When a Black, Indigenous, Latinx, or Southeast Asian storyteller uses those tools, they spend half their time fighting the tool instead of writing the story.

This toolkit flips that.

---

## What It Does

The Minority Narrative Engine is a Unity package that layers on top of Unity as a complete narrative system — dialogue, branching choices, character relationships, and community dynamics — with features designed specifically for cultural storytelling:

- **Code-switching** — AAVE, Spanglish, and other language varieties are first-class runtime features, not workarounds. Toggle on/off for accessibility.
- **Honorifics** — Characters address each other correctly. `{honorific.elder}` resolves to *OG*, *Auntie*, *Elder*, *Abuela*, or *Phi* depending on the cultural context and relationship.
- **Oral tradition patterns** — Call-and-response, testimony, elder wisdom, signifying, dichos, story circles. Tagged on story nodes and handled by the engine automatically.
- **Collectivity system** — Every choice tracks whether it leans toward the individual or the community. The central tension in many cultural stories isn't good vs. evil — it's *self vs. people*.
- **Community health tracking** — Settlements, families, and communities are first-class story objects with health scores that choices affect and conditions can check.
- **Non-technical first** — A visual Story Builder and Cultural Template Wizard let writers build full branching stories without touching code or JSON.

---

## Cultural Contexts

The engine ships with four pre-configured cultural contexts. Each one is a ScriptableObject you can inspect and customize in Unity:

| Context | Honorifics | Code-Switching | Oral Tradition | Collectivity |
|---|---|---|---|---|
| **Black American** | OG, Auntie, Big Homie, Blood | AAVE rules | Call-and-response, testimony, signifying, elder wisdom | 0.75 |
| **Indigenous** | Elder, Grandmother, the Ancestors | Land-relational language | Story circle, ancestor voice, land teaching, protocol | 0.90 |
| **Southeast Asian** | Kuya, Ate, Lola, Phi, Nong | Face-saving indirect speech | Family council, face moment, elder teaching | 0.80 |
| **Latinx** | Abuela, Don/Doña, Compadre | Spanglish substitutions | Cuento, dicho, familismo moment, consejos | 0.82 |

Create your own context using the Cultural Template Wizard — no code required.

---

## Getting Started

### Install

Copy `com.minoritynarrative.engine/` into your Unity project's `Packages/` folder. Unity 2022.3 LTS or later.

### Write a Story

Open `Window > Minority Narrative > Story Builder` and start adding nodes. Export to JSON when ready.

Or write the JSON directly — it's designed to be human-readable:

```json
{
  "title": "First Steps",
  "culturalContext": "black_american",
  "startNodeId": "intro",
  "nodes": [
    {
      "id": "intro",
      "speakerId": "grandma",
      "text": "Baby, I need you to listen to me. You all ready?",
      "culturalTags": ["elder_wisdom"],
      "choices": [
        {
          "text": "Yes ma'am.",
          "targetNodeId": "scene_2",
          "communityImpact": "You show respect. Grandma sees it.",
          "collectivityDelta": 0.3
        },
        {
          "text": "I got things to do, Grandma.",
          "targetNodeId": "scene_2_b",
          "collectivityDelta": -0.3
        }
      ]
    }
  ]
}
```

`"you all"` becomes `"y'all"` automatically at runtime. The `elder_wisdom` tag labels the node in the Story Builder and applies the oral tradition styling.

### Wire Up Your Scene

```csharp
// Drop NarrativeEngine on a GameObject, assign your JSON + registries, then:
narrativeEngine.OnDialogueEvent.AddListener(evt => {
    speakerLabel.text = evt.speakerDisplayName;
    bodyText.text = evt.resolvedText; // code-switched, honorifics resolved
    // build choice buttons from evt.choices
});

narrativeEngine.BeginStory();
```

---

## Package Structure

```
com.minoritynarrative.engine/
├── Runtime/
│   ├── Core/               NarrativeEngine, StoryLoader, StorySession
│   ├── StoryGraph/         StoryNode, Choice, conditions, triggers
│   ├── Dialogue/           DialogueSystem, code-switching, honorifics
│   ├── CulturalContext/    Base class + 4 built-in contexts
│   ├── CharacterProfile/   Character data + relationship tracking
│   └── Community/          Community nodes + registry
├── Editor/
│   ├── StoryBuilder/       Visual node graph editor
│   └── CulturalTemplateWizard/  5-step context configuration wizard
├── Samples~/
│   └── IsaiahFree/         Complete demo story + Unity controller
└── package.json
```

---

## Demo — The Last Ride of Isaiah Free

The toolkit ships with a complete demo story: **44 nodes, fully branching, 6 distinct endings.**

> *1873, Texas. Isaiah Free — a Black freedman and bounty hunter — rides into Redrock with one name on his list: Colonel Harlan Vance, the man who sold his family into bondage. But Vance has just burned buildings in Freedmen's Town, the Black settlement Isaiah once called home, and his men are circling.*
>
> *Isaiah has to choose.*

The demo demonstrates every engine system: AAVE code-switching, call-and-response with Ma Ellen, land teaching from Juniper Roots (Comanche Nation ally), signifying in Isaiah's standoff speech, the Freedmen's Town community score, collectivity tracking across six possible endings.

### Run It Now (No Unity Required)

```bash
python3 run_demo.py
```

Python 3.8+, no dependencies. Full terminal interactive story with color-coded dialogue, oral tradition tags, community bar, and collectivity indicator.

---

## Editor Tools

### Story Builder
`Window > Minority Narrative > Story Builder`

Canvas-based visual node graph. Add nodes, connect choices, set cultural tags and collectivity framing — all without editing JSON. Import and export story files. Designed for writers who've never opened a code editor.

### Cultural Template Wizard
`Window > Minority Narrative > Cultural Template Wizard`

Five-step guided setup for creating a cultural context asset:
1. Choose a cultural foundation
2. Configure honorific tokens
3. Set code-switching rules
4. Set individual vs. community balance
5. Review and generate the `.asset` file

---

## Full Documentation

See [`MINORITY_NARRATIVE_ENGINE.md`](MINORITY_NARRATIVE_ENGINE.md) for the complete guide covering every system, all JSON fields, condition and trigger reference, UI integration code, design guidance, and cultural sensitivity notes.

---

## Roadmap

**V1 (current)** — Non-technical first. Visual Story Builder, Cultural Template Wizard, four cultural contexts, complete demo story.

**V2 (planned)** — Developer API layer. Save/load system, localization support, custom oral tradition processors, playthrough analytics, additional cultural contexts.

---

## Cultural Sensitivity

The four built-in contexts are starting points, not authoritative representations. Indigenous cultures span hundreds of distinct nations — the built-in context uses broadly shared patterns but cannot represent that diversity. If you are building a story for a specific community, consult members of that community directly and customize the defaults.

Code-switching is a feature players can control. Some will want it on because it's authentic to their voice. Others may have accessibility reasons to turn it off. Neither choice is wrong — the toggle is built in.

The collectivity system tracks the individual vs. community tension in choices. It is not designed to moralize. Many of the most powerful cultural stories are *about* the tragedy of that impossible choice. Use it to reflect the tension, not to grade the player.

---

*Built for storytellers who carry something worth telling.*
