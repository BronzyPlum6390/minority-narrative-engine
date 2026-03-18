# Minority Narrative Engine Toolkit

**Version 0.1.0 — Unity Layer**

> Built for storytellers from underrepresented communities who are tired of forcing their narratives into tools that were never designed for them.

---

## Table of Contents

1. [What Is This?](#what-is-this)
2. [Who It's For](#who-its-for)
3. [Core Concepts](#core-concepts)
4. [Getting Started](#getting-started)
5. [Writing Your First Story](#writing-your-first-story)
6. [The Story JSON Format](#the-story-json-format)
7. [Cultural Contexts](#cultural-contexts)
8. [Honorifics System](#honorifics-system)
9. [Code-Switching](#code-switching)
10. [Oral Tradition Patterns](#oral-tradition-patterns)
11. [The Collectivity System](#the-collectivity-system)
12. [Community Nodes](#community-nodes)
13. [Conditions & Triggers](#conditions--triggers)
14. [Characters & Relationships](#characters--relationships)
15. [The Story Builder (Visual Editor)](#the-story-builder-visual-editor)
16. [The Cultural Template Wizard](#the-cultural-template-wizard)
17. [Wiring Up Your Unity Scene](#wiring-up-your-unity-scene)
18. [The Isaiah Free Demo](#the-isaiah-free-demo)
19. [Running the Demo Without Unity](#running-the-demo-without-unity)
20. [Extending the Engine (V2 Preview)](#extending-the-engine-v2-preview)
21. [Cultural Sensitivity Notes](#cultural-sensitivity-notes)

---

## What Is This?

The **Minority Narrative Engine Toolkit** is a Unity package that gives indie developers from underrepresented communities a purpose-built foundation for cultural storytelling. It layers on top of Unity as a narrative system — handling dialogue, branching choices, character relationships, and community dynamics — with features that mainstream tools like Twine, Ink, and Ren'Py don't provide out of the box.

Most narrative tooling was built with a narrow set of storytelling assumptions baked in: individual protagonists, linear moral framing, Western family structures, and language patterns that treat "standard English" as the neutral default. When a Black, Indigenous, Latinx, or Southeast Asian storyteller tries to use those tools, they spend half their time fighting the tool instead of writing the story.

This toolkit flips that. The engine's defaults assume:

- Communities matter as much as individuals
- Language is cultural, not neutral — AAVE, Spanglish, and honorifics like *Auntie* or *Abuela* are first-class features, not workarounds
- Oral storytelling traditions — call-and-response, testimony, dichos, story circles — are valid narrative structures
- A story's stakes can be about what happens to a *people*, not just a person
- Respect and kinship are structural to dialogue, not cosmetic

**V1** is designed for non-technical storytellers — writers who may never open a code editor. The visual Story Builder and Cultural Template Wizard let you build a full branching narrative without writing a line of C# or JSON by hand. **V2** (roadmap) will expose the full developer API for teams that want to build custom UI, integrate with existing Unity systems, or extend the engine's core behavior.

---

## Who It's For

**Writers and creators** who want to tell stories rooted in Black American, Indigenous, Southeast Asian, or Latinx cultural contexts without having to explain why their honorifics don't fit a standard dropdown.

**Indie game studios** building narrative games where community stakes, cultural identity, and oral tradition are central to the gameplay, not flavor text.

**Educators and community organizations** building interactive experiences for cultural education or historical documentation.

**Game jams and student projects** where a team needs a story system up and running in hours, not days, with cultural authenticity built in from the start.

---

## Core Concepts

Before diving into setup, these are the five ideas that run through everything in the engine:

### Story Graph
The story is a graph of **nodes** connected by **choices** or **linear flow**. Each node is a single beat — a line of dialogue, a narration, a scene. The graph can branch at any point and converge back together. Stories are saved as human-readable JSON files so any writer can edit them in any text editor.

### Cultural Context
Every story declares a **cultural context** — a configuration object that tells the engine how language, honorifics, and community framing should work in this story. The engine ships with four pre-built contexts (Black American, Indigenous, Southeast Asian, Latinx), and you can create your own using the wizard. The cultural context is the engine's soul — it shapes everything from how characters address each other to how choice consequences are framed.

### Collectivity
Every choice in the engine has an optional **collectivity delta** — a number between -1 and +1 that tracks whether a decision leans toward the individual or the community. Over the course of a story, this accumulates into a **collectivity score** that reflects the player's cumulative stance. This score can unlock branches, change endings, and be displayed as a UI indicator. It's a first-class narrative mechanic because in many cultural storytelling traditions, the central tension isn't "good vs. evil" — it's "self vs. community."

### Community Nodes
Beyond characters, stories can track named **communities** — a settlement, a family, a neighborhood, a nation. Each community has a health/trust score that choices affect. When Isaiah Free protects Freedmen's Town, that settlement's score rises. When he rides off alone, it drops. These scores power conditional branches (the community's fate can change what's possible later in the story) and can be displayed as a UI bar.

### Oral Tradition Patterns
The engine understands that dialogue in many cultural traditions isn't just one person talking to another. **Call-and-response**, **testimony**, **dichos**, **elder wisdom**, and **story circles** are structural patterns with their own rhythm and rules. Story nodes can be tagged with these patterns, and the engine handles them — injecting affirmation prompts, prefix/suffix text, and response options automatically.

---

## Getting Started

### Installation

1. Copy the `com.minoritynarrative.engine/` folder into your Unity project's `Packages/` directory.
2. Open Unity. The package will import automatically.
3. Verify installation: the menu `Window > Minority Narrative` should appear.

### Minimum Unity Version

Unity 2022.3 LTS or later.

### Package Structure

```
com.minoritynarrative.engine/
├── Runtime/                    C# scripts that run in your game
│   ├── Core/                   NarrativeEngine, StoryLoader, StorySession
│   ├── StoryGraph/             StoryNode, Choice, conditions, triggers
│   ├── Dialogue/               DialogueSystem, code-switching, honorifics
│   ├── CulturalContext/        Base class + 4 built-in contexts
│   ├── CharacterProfile/       Character data + relationship tracking
│   └── Community/              Community nodes + registry
├── Editor/                     Unity editor tools (don't ship with your game)
│   ├── StoryBuilder/           Visual node graph editor
│   └── CulturalTemplateWizard/ 5-step context configuration wizard
├── Samples~/
│   └── IsaiahFree/             Complete demo story + controller
└── package.json
```

---

## Writing Your First Story

### The Fastest Path (No Code)

1. Open `Window > Minority Narrative > Story Builder`
2. Click `+ Dialogue` to add your first node
3. Select the node — the sidebar shows its fields. Fill in a speaker ID and text.
4. Add choices with the `+ Add Choice` button
5. Add more nodes and connect them by filling in `→ Target Node` on each choice
6. Click `Export JSON` when you're ready

That's a working story. Drop the JSON into your Unity project and assign it to the NarrativeEngine — it's playable.

### The Story File Format

Stories live in `.story.json` files. Here's the smallest possible story:

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
      "choices": [
        {
          "text": "Yes ma'am.",
          "targetNodeId": "scene_2",
          "collectivityDelta": 0.3
        },
        {
          "text": "I got things to do, Grandma.",
          "targetNodeId": "scene_2_b",
          "collectivityDelta": -0.3
        }
      ]
    },
    {
      "id": "scene_2",
      "speakerId": "grandma",
      "text": "Good. Then sit down.",
      "nextNodeId": "end_node"
    },
    {
      "id": "scene_2_b",
      "speakerId": "grandma",
      "text": "...",
      "nextNodeId": "end_node"
    },
    {
      "id": "end_node",
      "type": "end",
      "text": "She pours you a cup anyway."
    }
  ]
}
```

When the `black_american` context is active, `"you all"` in Grandma's first line is automatically substituted with `"y'all"` at runtime.

---

## The Story JSON Format

A story file has two sections: **metadata** at the top and a **nodes array**.

### Metadata Fields

| Field | Required | Description |
|---|---|---|
| `title` | Yes | Story title shown in the editor and UI |
| `startNodeId` | Yes | ID of the first node to display |
| `nodes` | Yes | Array of all story nodes |
| `culturalContext` | Recommended | Key matching a cultural context asset (`black_american`, `indigenous`, `southeast_asian`, `latinx`) |
| `description` | No | Short synopsis for the story browser |
| `author` | No | Author credit |
| `version` | No | Version string for save-game compatibility (default: `"1.0"`) |

### Node Fields

| Field | Type | Description |
|---|---|---|
| `id` | string | **Required.** Unique identifier for this node |
| `type` | string | `dialogue`, `narration`, `choice_prompt`, `community_event`, `end` (default: `dialogue`) |
| `speakerId` | string | Character ID of the speaker. Leave empty for narration |
| `text` | string | The dialogue or narration text. Supports token substitution |
| `choices` | array | Player choices. If empty, node advances linearly via `nextNodeId` |
| `nextNodeId` | string | Next node for linear flow (ignored if choices is non-empty) |
| `culturalTags` | array | Oral tradition pattern roles: `call_and_response`, `elder_wisdom`, `testimony`, etc. |
| `collectiveFrame` | string | UI framing hint: `individual`, `collective`, `both`, `none` |
| `displayDuration` | number | Seconds before auto-advancing. `-1` means wait for player input |
| `triggers` | array | Side-effects fired when this node is entered |
| `entryConditions` | array | Conditions that must pass for this node to be reachable |

### Choice Fields

| Field | Type | Description |
|---|---|---|
| `text` | string | **Required.** Button text shown to the player |
| `targetNodeId` | string | **Required.** Node to go to when this choice is selected |
| `communityImpact` | string | One-line description of the community consequence |
| `individualImpact` | string | One-line description of the personal consequence |
| `collectivityDelta` | number | `-1.0` to `+1.0`. Positive = collective-leaning |
| `conditions` | array | Conditions that must pass for this choice to appear |
| `triggers` | array | Side-effects fired when this choice is selected |

---

## Cultural Contexts

A **cultural context** is a ScriptableObject that configures how the engine handles language, honorifics, and community framing for a specific cultural setting. Every story references one.

### Built-In Contexts

#### Black American (`black_american`)
Configured with AAVE code-switching rules, kinship honorifics (`OG`, `Auntie`, `Big Homie`, `Blood`), and oral tradition patterns rooted in call-and-response, elder wisdom, signifying, and testimony. Collectivity weight: **0.75**.

#### Indigenous (`indigenous`)
Configured with clan and ancestor honorifics (`Elder`, `Grandmother`, `the Ancestors`), land-relational language substitutions (`"the land"` → `"our Mother"`), and oral tradition patterns including story circle, ancestor voice, and land teaching. Collectivity weight: **0.90** — the highest of any built-in context, reflecting the centrality of collective wellbeing in many Indigenous storytelling traditions. *Note: Indigenous cultures are highly diverse. This template is a starting point — customize it for your specific nation or community.*

#### Southeast Asian (`southeast_asian`)
Configured with hierarchical honorifics across Filipino, Thai, and Vietnamese defaults (`Kuya`, `Ate`, `Lola`, `Phi`, `Nong`), face-saving indirect speech patterns, and oral tradition patterns including family council, elder teaching, and face moments. Collectivity weight: **0.80**. *Customize for your specific culture.*

#### Latinx (`latinx`)
Configured with familismo honorifics (`Abuela`, `Don`, `Doña`, `Compadre`), Spanglish code-switching (`"family"` → `"familia"`, `"my love"` → `"mi amor"`), and oral tradition patterns including cuento, dicho, familismo moment, and consejos. Collectivity weight: **0.82**. *Customize for your specific region.*

### Creating a Custom Context

Use `Window > Minority Narrative > Cultural Template Wizard`. The wizard walks you through five steps and generates a `.asset` file in your project. No code required.

To create a context in code (V2 approach):

```csharp
// Create a new context asset manually
[CreateAssetMenu(menuName = "Minority Narrative/Contexts/My Context")]
public class MyContext : CulturalContextBase
{
    private void Reset()
    {
        contextKey = "my_context";
        displayName = "My Cultural Context";
        collectivityWeight = 0.75f;

        honorifics.Add(new HonorificsEntry {
            token = "elder",
            form = "Nana",
            tier = RelationshipTier.Elder
        });

        codeSwitchingRules.Add(new CodeSwitchEntry {
            standard = "going to",
            culturalForm = "fixin' to",
            wholeWordOnly = false
        });
    }
}
```

---

## Honorifics System

Honorifics are how characters address each other with cultural respect. In your story text, you write a **token** and the engine resolves it at runtime based on the active cultural context and the relationship between the speaker and the person being addressed.

### Token Format

```
{honorific.TOKEN}
```

### Example in Story JSON

```json
{
  "speakerId": "deja",
  "text": "I hear you, {honorific.elder}. I just need a minute to think."
}
```

With the `black_american` context active, this resolves to:
> *"I hear you, OG. I just need a minute to think."*

With the `indigenous` context:
> *"I hear you, Elder. I just need a minute to think."*

### Relationship-Aware Resolution

The resolved form can vary based on the relationship score between the speaker and the person they're addressing. A character who has built a close bond (`RelationshipTier.Family`) might get a warmer, more intimate honorific than a stranger. Configure the tiers in your cultural context asset.

### Built-In Tokens by Context

**Black American:** `elder`, `elder_female`, `elder_male`, `respected_peer`, `community_leader`, `stranger`, `family`

**Indigenous:** `elder`, `elder_female`, `elder_male`, `ancestor`, `community_leader`, `family`, `land`

**Southeast Asian:** `elder_sibling_male`, `elder_sibling_female`, `grandmother`, `grandfather`, `elder_general`, `younger_general`, `uncle_paternal`, `aunt_maternal`, `respected_elder`

**Latinx:** `grandmother`, `grandfather`, `respected_man`, `respected_woman`, `godfather`, `godmother`, `family_friend`, `family_friend_female`, `peer`

---

## Code-Switching

Code-switching is the practice of moving between languages or language varieties depending on context. The engine supports it as a first-class runtime feature — writers can author their story in whatever base language they choose, and the engine applies cultural substitutions automatically.

### How It Works

The `CodeSwitchingProcessor` runs every line of dialogue through the active context's substitution rules before it reaches the UI. Rules are simple find-and-replace pairs with an optional whole-word boundary flag.

### Example Rules (Black American Context)

| Standard Form | Cultural Form |
|---|---|
| `you all` | `y'all` |
| `going to` | `finna` |
| `for real` | `for real for real` |
| `He is always` | `He be always` |

### Accessibility Toggle

Players can turn code-switching off and on at runtime:

```csharp
narrativeEngine.SetCodeSwitching(false); // player prefers standard form
narrativeEngine.SetCodeSwitching(true);  // restore cultural substitutions
```

This is important: code-switching should be a feature players can control, not something forced on them. Some players will want it on because it's authentic to their voice. Others may have accessibility or comprehension reasons to turn it off. Neither choice is wrong.

### Adding Your Own Rules

In the Cultural Template Wizard (Step 3: Language), or directly in the Inspector on your context asset:

```
"God willing"  →  "si Dios quiere"   (Latinx context)
"I own"        →  "I am in relationship with"   (Indigenous context)
```

---

## Oral Tradition Patterns

Oral storytelling traditions carry structure that written Western narrative doesn't have built-in vocabulary for. The engine supports these as **tagged patterns** on story nodes.

When a node carries a cultural tag matching an oral tradition pattern, the engine:
1. Applies prefix/suffix text if defined
2. Shows a labeled badge in the story builder
3. Optionally presents call-and-response prompts to the player
4. Colors the node differently on the canvas

### Tagging a Node

```json
{
  "id": "mamas_warning",
  "speakerId": "mama",
  "text": "You came from something, baby. Don't let them make you forget it.",
  "culturalTags": ["elder_wisdom"]
}
```

### Built-In Patterns

#### Call and Response (`call_and_response`)
A beat where the speaker says something and the community or player affirms it. The engine presents a set of response options — *"That's right."*, *"Mm-hmm."*, *"Say that."* — and the player picks one before the story advances. Used for communal scenes, church moments, and shared grief or celebration.

#### Elder Wisdom (`elder_wisdom`)
An elder delivers a proverb or hard-won truth. These beats slow the pace and carry moral weight. A suffix ` — remember that.` is automatically appended in the default Black American context. In the Indigenous context, ancestor voice patterns work similarly.

#### Signifying (`signifying`)
A Black American oral tradition pattern of indirect speech with layered meaning — the character says one thing, means another. Irony, wit, coded critique. No response prompt; the meaning lands in the silence.

#### Testimony (`testimony`)
A character witnesses or recounts something true and painful. The community affirms. Response prompts: *"Tell it."*, *"We know."* These nodes carry the weight of historical memory.

#### Dicho (`dicho`)
A Latinx proverb or traditional saying. The text is automatically wrapped in quotation marks in the display to signal its status as a dicho.

#### Familismo Moment (`familismo_moment`)
A Latinx pattern where the needs of family or community directly confront individual desire. Response prompts reinforce the cultural value: *"La familia primero."*

#### Story Circle (`story_circle`) — Indigenous
Stories told in a circle with no defined beginning or end. Used for multi-generational or cyclical narrative beats.

#### Land Teaching (`land_teaching`) — Indigenous
A teaching that comes from observing land, animals, or seasons. Grounds the narrative in place and relational knowledge.

---

## The Collectivity System

This is the engine's most distinctive mechanic. Most choice systems only track individual outcomes — *did you save the person or not?* The collectivity system tracks something more nuanced: across all the choices a player makes, are they tilting toward the self or toward the community?

### How It Works

Each `Choice` has a `collectivityDelta` value from `-1.0` to `+1.0`:

- `+1.0` — strongly collective (choosing community over self)
- `+0.5` — moderately collective
- `0.0` — neutral
- `-0.5` — moderately individual
- `-1.0` — strongly individual (choosing self over community)

When the player selects a choice, that delta is added to the session's `collectivityScore`. The score accumulates across the whole story.

### Using It in Conditions

The collectivity score can gate specific story branches:

```json
{
  "conditions": [
    {
      "type": "collectivity",
      "key": "collectivity",
      "op": "gte",
      "value": 2.0
    }
  ]
}
```

This choice or node only appears if the player has made enough collective-leaning choices. Use this to unlock community-specific endings, trust from community characters, or elder wisdom moments.

### Displaying It in UI

Connect a `Slider` component to `IsaiahFreeController.collectivityBar` (or your equivalent). The engine maps the collectivity score to a 0–1 range, with the center representing balance.

### Design Guidance

Avoid moralizing. Don't make collective choices "good" and individual choices "bad." Many of the most powerful narrative moments in culturally specific stories come from the genuine tragedy of an individual having to choose between themselves and their people — and neither answer being wrong. Use the collectivity system to *track* this tension, not to judge it.

---

## Community Nodes

A **CommunityNode** is a named collective body — a settlement, a family unit, a neighborhood, a nation — that exists in the story world and has a health/trust score that choices can affect.

### Creating a Community Node

In Unity: `Assets > Create > Minority Narrative > Community Node`

| Field | Description |
|---|---|
| `communityId` | Unique ID used in JSON triggers and conditions (e.g., `freedens_town`) |
| `displayName` | Name shown in UI indicators |
| `startingScore` | Health score at the start of a new game (0–100) |
| `crisisThreshold` | Score below this unlocks crisis-specific branches |
| `thrivingThreshold` | Score above this unlocks positive branches |
| `crisisStateText` | Text displayed when the community is in crisis |
| `thrivingStateText` | Text displayed when the community is thriving |

### Affecting Community Scores

Use triggers on choices or nodes:

```json
{
  "triggers": [
    {
      "type": "adjust_community",
      "key": "freedens_town",
      "value": 25
    }
  ]
}
```

### Checking Community Scores in Conditions

```json
{
  "conditions": [
    {
      "type": "community",
      "key": "freedens_town",
      "op": "gte",
      "value": 70
    }
  ]
}
```

This condition passes only if Freedmen's Town's trust score is 70 or above — unlocking a branch that reflects a community in good standing.

---

## Conditions & Triggers

Conditions and triggers are how the story responds to everything the player has done.

### Trigger Types

| Type | Key | Value | Effect |
|---|---|---|---|
| `set_flag` | flag name | — | Sets a named boolean flag |
| `clear_flag` | flag name | — | Clears a named boolean flag |
| `adjust_community` | community ID | delta | Adds delta to community score |
| `adjust_relationship` | character A ID | delta | Adjusts relationship score between two characters |
| `fire_event` | event name | — | Fires a named Unity event (wire up custom behavior) |
| `set_variable` | variable name | value | Sets a named float variable |

### Condition Types

| Type | Key | Ops | Description |
|---|---|---|---|
| `flag` | flag name | `set`, `unset` | Checks if a flag is set |
| `community` | community ID | `gt`, `gte`, `lt`, `lte`, `eq` | Checks a community score |
| `relationship` | character A ID | `gt`, `gte`, `lt`, `lte` | Checks relationship between two characters |
| `collectivity` | — | `gt`, `gte`, `lt`, `lte` | Checks the collectivity score |
| `visited` | node ID | `set`, `unset` | Checks if a node has been visited |

### Example: A Choice That Only Appears After Building Trust

```json
{
  "text": "I'll stand with you.",
  "targetNodeId": "alliance_scene",
  "conditions": [
    {
      "type": "relationship",
      "key": "isaiah_free",
      "relatedCharacterId": "juniper_roots",
      "op": "gte",
      "value": 30
    }
  ]
}
```

This choice only appears if Isaiah and Juniper's relationship score is 30 or above — meaning the player has invested in that relationship earlier in the story.

---

## Characters & Relationships

### CharacterProfile

Each named character in your story has a `CharacterProfile` ScriptableObject.

Create one: `Assets > Create > Minority Narrative > Character Profile`

| Field | Description |
|---|---|
| `characterId` | Unique ID matching the `speakerId` field in story nodes |
| `displayName` | Name shown in dialogue UI |
| `culturalContext` | Reference to the character's cultural context |
| `portraitId` | Asset key for the character's portrait image |
| `communityTier` | Default social standing (`Elder`, `Trusted`, `Neutral`, etc.) |
| `startingRelationships` | Initial relationship scores with other characters |

### Relationship Scores

Relationships are tracked per-session as float values from `-100` to `+100`. They affect:

- Which honorific form is used when one character addresses another
- Whether relationship-gated choices are available
- Custom story branches that respond to character trust

Set starting values on the CharacterProfile. Adjust them at runtime with `adjust_relationship` triggers:

```json
{
  "type": "adjust_relationship",
  "key": "isaiah_free",
  "relatedCharacterId": "ma_ellen",
  "value": 20
}
```

### CharacterRegistry

The `CharacterRegistry` is a ScriptableObject that holds all the characters in your story. Assign it to the NarrativeEngine and it will resolve character IDs to profiles at runtime and seed starting relationships into the session automatically.

Create one: `Assets > Create > Minority Narrative > Character Registry`

---

## The Story Builder (Visual Editor)

Open via `Window > Minority Narrative > Story Builder`

The Story Builder is a canvas-based visual editor for non-technical authors. You can build a complete branching story without touching the JSON file directly.

### Canvas Controls

| Action | Input |
|---|---|
| Pan the canvas | Middle mouse button drag |
| Zoom | Scroll wheel |
| Select a node | Left-click |
| Drag a node | Left-click drag on the node |
| Deselect | Left-click on empty canvas |

### Toolbar

- **+ Dialogue** — adds a new dialogue node (one character speaking)
- **+ Narration** — adds a narration node (third-person scene description)
- **+ Choice** — adds a choice prompt node (where the player decides)
- **+ End** — adds a story ending node
- **Import JSON** — opens an existing `.story.json` file into the canvas
- **Export JSON** — saves the canvas as a `.story.json` file

### Sidebar — Node Inspector

When you select a node, the left sidebar shows all its editable fields:

- **ID** — unique identifier (auto-generated, but you can rename it to something readable)
- **Speaker ID** — the character ID speaking this line
- **Text** — the dialogue or narration body
- **Collectivity Frame** — how the UI should frame this beat
- **Next Node ID** — for linear flow
- **Choices** — add, edit, and remove choices with target node IDs, impact descriptions, and collectivity deltas

### Node Colors

Nodes are color-coded by type on the canvas:
- **Blue-grey** — dialogue
- **Green-grey** — narration
- **Orange-grey** — choice prompt
- **Purple-grey** — community event
- **Red-grey** — end node

Cultural tags are shown as colored badges on each node (green for call-and-response, gold for elder wisdom, purple for signifying, etc.)

---

## The Cultural Template Wizard

Open via `Window > Minority Narrative > Cultural Template Wizard`

The wizard is a five-step guided setup for creating a cultural context asset without writing code.

### Step 1 — Choose Your Context
Select one of the four cultural foundations with plain-language descriptions. The pre-built contexts come with sensible defaults for their cultural settings.

### Step 2 — Configure Honorifics
A table of honorific tokens and their resolved forms. Edit, add, or remove entries. The `{honorific.TOKEN}` syntax is explained inline.

### Step 3 — Language & Code-Switching
A list of substitution pairs. Toggle code-switching on/off for the whole context. Add new word and phrase mappings.

### Step 4 — Community & Individual Balance
A plain-language slider from "Individual-focused" to "Community-focused" with descriptions at each range. Toggle whether community impact text appears alongside player choices.

### Step 5 — Review & Generate
A full summary of your configuration. Set the save folder and click **Generate Asset** to create the `.asset` file in your project. The asset will be selected automatically.

---

## Wiring Up Your Unity Scene

### Minimum Setup

1. Create an empty GameObject and name it `NarrativeEngine`
2. Add the `NarrativeEngine` component (`Add Component > Minority Narrative > Narrative Engine`)
3. In the Inspector, assign:
   - **Story Json** — your `.story.json` TextAsset
   - **Character Registry** — your CharacterRegistry ScriptableObject
   - **Community Registry** — your CommunityRegistry ScriptableObject
   - **Cultural Context Registry** — your CulturalContextRegistry ScriptableObject
4. Wire up `OnDialogueEvent` (UnityEvent) to your UI handler
5. Wire up `OnStoryComplete` to your end-of-story handler
6. Call `narrativeEngine.BeginStory()` from your game flow

### UI Integration

The `NarrativeEngine` emits a `DialogueEvent` each time a new beat is ready. Subscribe to `OnDialogueEvent` in your UI script:

```csharp
narrativeEngine.OnDialogueEvent.AddListener(OnDialogueEvent);

void OnDialogueEvent(DialogueEvent evt)
{
    speakerLabel.text = evt.speakerDisplayName;
    bodyText.text = evt.resolvedText; // already code-switched + honorifics resolved

    choicePanel.SetActive(evt.choices.Count > 0);
    foreach (var choice in evt.choices)
    {
        // spawn a button
        // choice.text — the choice display text
        // choice.communityImpact — optional community consequence line
        // choice.individualImpact — optional personal consequence line
        // choice.collectivityDelta — for collectivity indicator
    }
}
```

### Player Input

```csharp
// Player selects a choice by index
narrativeEngine.SelectChoice(choiceIndex);

// Player advances a linear (no-choice) node
narrativeEngine.Advance();

// Toggle code-switching (accessibility)
narrativeEngine.SetCodeSwitching(enabled);
```

### Named Events

Story nodes can fire named events via triggers: `{ "type": "fire_event", "key": "door_opens" }`. Register listeners at runtime:

```csharp
narrativeEngine.RegisterNamedEventListener("door_opens", () => {
    door.SetActive(true);
});
```

---

## The Isaiah Free Demo

**"The Last Ride of Isaiah Free"** is the full demo story shipped with the toolkit under `Samples~/IsaiahFree/`.

### Story Synopsis

1873, Texas. Isaiah Free — a Black freedman and bounty hunter — rides into Redrock with one name on his list: Colonel Harlan Vance, the man who sold his family into bondage. But Vance has just burned buildings in Freedmen's Town, the Black settlement Isaiah once called home, and his men are circling. Isaiah has to choose: close the distance on Vance, or turn back for his people.

### Characters

| Character | Role |
|---|---|
| **Isaiah Free** | Protagonist. Black freedman, bounty hunter. Carries personal grief and community responsibility in equal measure. |
| **Juniper Roots** | Comanche Nation. Introduces cross-cultural collective-choice dynamics via Indigenous oral tradition patterns. |
| **Ma Ellen** | Elder matriarch of Freedmen's Town. The moral weight of the community arc. |
| **Colonel Harlan Vance** | Antagonist. Drives the revenge branch. |

### Endings

The demo has six distinct endings depending on the player's path:

1. **Stay in Freedmen's Town** — Isaiah puts down the bounty poster. The community gets its best chance.
2. **Arrest Vance** — Isaiah uses a federal warrant. Legal justice, visible to all.
3. **Bear Witness** — Isaiah names his parents. Makes Vance look. Walks out.
4. **Shoot Vance** — Revenge. The question of whether it was enough is left open.
5. **Stand Ground (Community Path)** — Isaiah rallies the community. Vance's men leave. The fight isn't over.
6. **Retreat** — Isaiah protects lives but the town is lost. Something that can't be rebuilt.

### Systems Demonstrated

- AAVE code-switching (`"you all"` → `"y'all"` in the first paragraph)
- `◆ ELDER WISDOM` and `◆ LAND TEACHING` oral tradition tags on Juniper's dialogue
- `◆ TESTIMONY` and `◆ CALL & RESPONSE` in Ma Ellen's scenes
- `◆ SIGNIFYING` in Isaiah's standoff speech
- `⬡ Community:` and `◈ Personal:` impact labels on every choice
- Collectivity delta tracking from -1.0 to +1.0
- Freedmen's Town community score ranging from 0 to 100
- Relationship triggers between Isaiah and the other characters
- Flag-based conditional branches

---

## Running the Demo Without Unity

The demo can be run as a terminal interactive story without Unity installed, using the included Python runner.

### Requirements

Python 3.8 or later. No additional packages needed.

### Run

```bash
cd /path/to/minority_game_engine_tool_kit
python3 run_demo.py
```

The terminal runner implements the same engine logic as the Unity version: code-switching, oral tradition tags, community score tracking, collectivity scoring, conditional choices, and all six ending paths.

### Controls

- Press **ENTER** to advance a narration or dialogue line
- Type a **number** and press ENTER to select a choice
- At call-and-response prompts, type a number to pick your affirmation
- Press **Ctrl+C** at any time to exit

---

## Extending the Engine (V2 Preview)

V2 will expose the full developer API more completely. Here's a preview of what's coming:

### Custom Cultural Contexts via Code

Subclass `CulturalContextBase` and register it in the `CulturalContextRegistry`. The wizard will pick it up automatically.

### Custom Oral Tradition Processors

The `OralTraditionPattern` system will support custom processor delegates — so a `story_circle` pattern can shuffle nodes, or a `proverb` pattern can pull from an external database.

### Save / Load

`StorySession.TakeSnapshot()` already returns a serializable `SessionSnapshot`. V2 will add a `SaveManager` that handles JSON serialization to disk with versioning and migration support.

### Localization

The story format is designed to be localization-friendly. V2 will add a localization pass that maps node IDs to external translation tables, so the same story graph can run in multiple languages.

### Analytics

The `choiceHistory` and collectivity score in `StorySession` provide a foundation for anonymous playthrough analytics — understanding which paths players take, which choices get the most consideration time, and where stories lose players.

---

## Cultural Sensitivity Notes

The four built-in cultural contexts ship with reasonable defaults, but they are starting points — not authoritative representations of any culture.

**On Indigenous contexts:** Indigenous cultures span hundreds of distinct nations with their own languages, protocols, and storytelling traditions. The built-in `IndigenousContext` uses broadly shared patterns but cannot represent this diversity. If you are building a story set in or for a specific Indigenous community, consult with members of that community directly. Override the defaults in the Inspector. Credit your cultural consultants.

**On AAVE:** African American Vernacular English is a fully-formed linguistic system with grammatical rules and rich oral tradition. The code-switching rules in `BlackAmericanContext` are a starting set, not an exhaustive grammar. If your story features AAVE-speaking characters who are not Black, consider whether that's appropriate. If your story centers Black characters, consider having Black writers and cultural consultants review the language.

**On the collectivity system:** This system reflects a real tension present in many cultural storytelling traditions — self vs. community. It is not designed to suggest that collective choices are morally superior or that individual choices are selfish. Many stories in these traditions are *about* the tragedy of that impossible choice. Use the system to track and reflect the tension, not to grade the player.

**On representation:** This toolkit gives you tools. The responsibility for how those tools are used — who you consult, which stories you tell, and how you credit the communities whose culture you draw from — belongs to the storyteller.

---

*Minority Narrative Engine Toolkit — V0.1.0*
*Built for storytellers who carry something worth telling.*
