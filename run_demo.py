#!/usr/bin/env python3
"""
The Last Ride of Isaiah Free — Terminal Demo Runner
Minority Narrative Engine Toolkit

Reads IsaiahFree.story.json and runs the full branching story
in your terminal with AAVE code-switching, oral tradition tags,
community trust tracking, and collectivity scoring.
"""

import json
import os
import sys
import time
import re
import textwrap

IS_TTY = sys.stdin.isatty()
AUTO_INPUTS = []  # filled in auto mode

# ── Paths ────────────────────────────────────────────────────────────────
STORY_PATH = os.path.join(
    os.path.dirname(__file__),
    "com.minoritynarrative.engine",
    "Samples~",
    "IsaiahFree",
    "IsaiahFree.story.json",
)

# ── ANSI Colors ──────────────────────────────────────────────────────────
class C:
    RESET   = "\033[0m"
    BOLD    = "\033[1m"
    DIM     = "\033[2m"
    # speakers
    ISAIAH  = "\033[38;5;214m"   # warm amber
    JUNIPER = "\033[38;5;107m"   # sage green
    MAELLEN = "\033[38;5;183m"   # soft violet
    VANCE   = "\033[38;5;160m"   # red
    NARRATOR= "\033[38;5;245m"   # grey
    OTHER   = "\033[38;5;195m"   # light blue
    # UI
    CHOICE  = "\033[38;5;220m"   # gold
    COMM    = "\033[38;5;77m"    # green (community)
    INDV    = "\033[38;5;209m"   # peach (individual)
    TAG     = "\033[38;5;140m"   # purple (oral tradition tag)
    BAR     = "\033[38;5;240m"   # dark grey
    TITLE   = "\033[38;5;229m"   # cream
    WARN    = "\033[38;5;203m"   # orange-red
    GOOD    = "\033[38;5;120m"   # bright green

SPEAKER_COLORS = {
    "isaiah_free":      C.ISAIAH,
    "juniper_roots":    C.JUNIPER,
    "ma_ellen":         C.MAELLEN,
    "colonel_vance":    C.VANCE,
    "vance_lieutenant": C.VANCE,
}

SPEAKER_NAMES = {
    "isaiah_free":      "ISAIAH FREE",
    "juniper_roots":    "JUNIPER ROOTS",
    "ma_ellen":         "MA ELLEN",
    "colonel_vance":    "COLONEL VANCE",
    "vance_lieutenant": "VANCE'S MAN",
}

# ── Black American code-switching rules ──────────────────────────────────
CODE_SWITCH_RULES = [
    (r'\byou all\b',          "y'all"),
    (r'\bgoing to\b',         "finna"),
    (r'\bI am telling you\b', "I'm telling you"),
    (r'\bfor real\b',         "for real for real"),
    (r'\bHe is always\b',     "He be always"),
    (r'\bShe is always\b',    "She be always"),
]

ORAL_TRADITION_LABELS = {
    "call_and_response": "CALL & RESPONSE",
    "elder_wisdom":      "ELDER WISDOM",
    "signifying":        "SIGNIFYING",
    "testimony":         "TESTIMONY",
    "land_teaching":     "LAND TEACHING",
    "ancestor_voice":    "ANCESTOR VOICE",
    "story_circle":      "STORY CIRCLE",
    "dicho":             "DICHO",
}

CALL_AND_RESPONSE_REPLIES = [
    "That's right.",
    "Mm-hmm.",
    "Say that.",
    "I hear you.",
    "Tell it.",
]

# ── Engine ───────────────────────────────────────────────────────────────

class Session:
    def __init__(self):
        self.flags = set()
        self.visited = set()
        self.community_scores = {"freedens_town": 50.0}
        self.relationships = {}
        self.collectivity = 0.0
        self.choice_history = []

    def set_flag(self, k):       self.flags.add(k)
    def clear_flag(self, k):     self.flags.discard(k)
    def has_flag(self, k):       return k in self.flags
    def mark_visited(self, nid): self.visited.add(nid)
    def has_visited(self, nid):  return nid in self.visited

    def get_community(self, k):  return self.community_scores.get(k, 50.0)
    def adjust_community(self, k, d):
        self.community_scores[k] = max(0, min(100, self.community_scores.get(k, 50) + d))

    def get_rel(self, a, b):
        key = "::".join(sorted([a, b]))
        return self.relationships.get(key, 0.0)
    def adjust_rel(self, a, b, d):
        key = "::".join(sorted([a, b]))
        self.relationships[key] = max(-100, min(100, self.relationships.get(key, 0) + d))

    def apply_collectivity(self, d): self.collectivity += d

    def evaluate_condition(self, cond):
        t, k, op = cond["type"], cond["key"], cond["op"]
        val = cond.get("value", 0)
        if t == "flag":
            return self.has_flag(k) if op == "set" else not self.has_flag(k)
        if t == "community":
            return _cmp(self.get_community(k), op, val)
        if t == "relationship":
            return _cmp(self.get_rel(k, cond.get("relatedCharacterId", "")), op, val)
        if t == "collectivity":
            return _cmp(self.collectivity, op, val)
        if t == "visited":
            return self.has_visited(k) if op == "set" else not self.has_visited(k)
        return True

    def apply_trigger(self, trig):
        t = trig["type"]
        k = trig["key"]
        v = trig.get("value", 0)
        if t == "set_flag":            self.set_flag(k)
        elif t == "clear_flag":        self.clear_flag(k)
        elif t == "adjust_community":  self.adjust_community(k, v)
        elif t == "adjust_relationship":
            self.adjust_rel(k, trig.get("relatedCharacterId", ""), v)
        elif t == "set_variable":      pass  # tracked via flags in demo
        elif t == "fire_event":        pass  # handled at engine level

def _cmp(subject, op, value):
    return {
        "eq":  abs(subject - value) < 0.001,
        "neq": abs(subject - value) >= 0.001,
        "gt":  subject > value,
        "gte": subject >= value,
        "lt":  subject < value,
        "lte": subject <= value,
    }.get(op, True)


# ── Display helpers ──────────────────────────────────────────────────────

WIDTH = 72

def clear():
    if IS_TTY:
        print("\033[H\033[J", end="")
    else:
        print("\n" + "─" * WIDTH)

def divider(ch="─"):
    print(C.BAR + ch * WIDTH + C.RESET)

def slow_print(text, delay=0.012):
    if not IS_TTY:
        print(text)
        return
    for ch in text:
        sys.stdout.write(ch)
        sys.stdout.flush()
        if ch not in " \n": time.sleep(delay)
    print()

def wrap(text, indent=0):
    return textwrap.fill(text, width=WIDTH - indent, initial_indent=" " * indent,
                         subsequent_indent=" " * indent)

def apply_code_switching(text):
    for pattern, replacement in CODE_SWITCH_RULES:
        text = re.sub(pattern, replacement, text, flags=re.IGNORECASE)
    return text

def speaker_color(sid):
    return SPEAKER_COLORS.get(sid, C.OTHER)

def print_oral_tag(tags):
    if not tags: return
    for tag in tags:
        label = ORAL_TRADITION_LABELS.get(tag)
        if label:
            print(f"  {C.TAG}◆ {label}{C.RESET}")

def print_status_bar(session):
    trust = session.get_community(k="freedens_town")
    coll  = session.collectivity
    bar_len = 20

    # Community trust bar
    filled = int((trust / 100) * bar_len)
    bar = "█" * filled + "░" * (bar_len - filled)
    trust_color = C.GOOD if trust > 60 else (C.CHOICE if trust > 30 else C.WARN)

    # Collectivity bar (centered at 0)
    normalised = max(0, min(1, (coll + 5) / 10))
    c_filled = int(normalised * bar_len)
    c_bar = "█" * c_filled + "░" * (bar_len - c_filled)
    c_color = C.COMM if coll > 0 else C.INDV

    print(f"{C.BAR}  Freedmen's Town  {trust_color}{bar}{C.RESET}{C.BAR}  {trust:.0f}/100  "
          f"  {c_color}{'Community ◀' if coll > 0 else '▶ Personal ':>12} {c_bar}{C.RESET}")

def print_title_card():
    clear()
    divider("═")
    title = "THE LAST RIDE OF ISAIAH FREE"
    print(C.TITLE + C.BOLD + title.center(WIDTH) + C.RESET)
    sub = "1873 · Texas Frontier · A Story of Freedom, Community & Reckoning"
    print(C.DIM + sub.center(WIDTH) + C.RESET)
    divider("═")
    print()
    slow_print(
        "  Built with the Minority Narrative Engine Toolkit\n"
        "  Press ENTER to advance · Type a number to choose",
        delay=0.005
    )
    print()
    if IS_TTY:
        input(f"  {C.CHOICE}[ Press ENTER to begin ]{C.RESET}")
    else:
        print(f"  {C.CHOICE}[ AUTO-PLAY MODE — Community path ]{C.RESET}\n")

def render_node(node, session):
    clear()
    divider()
    print_status_bar(session)
    divider()
    print()

    sid   = node.get("speakerId", "")
    ntype = node.get("type", "dialogue")
    tags  = node.get("culturalTags", [])
    text  = apply_code_switching(node.get("text", ""))
    frame = node.get("collectiveFrame", "none")

    # Oral tradition tag
    print_oral_tag(tags)

    # Collective frame hint
    if frame in ("collective", "both"):
        print(f"  {C.COMM}⬡ COMMUNITY STAKES{C.RESET}")
    if frame in ("individual", "both"):
        print(f"  {C.INDV}◈ PERSONAL STAKES{C.RESET}")
    if tags or frame not in ("none", ""):
        print()

    # Speaker
    if sid and ntype not in ("narration",):
        color = speaker_color(sid)
        name  = SPEAKER_NAMES.get(sid, sid.upper().replace("_", " "))
        print(f"  {color}{C.BOLD}{name}{C.RESET}")
        print()

    # Body text
    if ntype == "narration":
        for line in text.split("\n"):
            if line.strip():
                print(C.NARRATOR + wrap(line.strip(), indent=2) + C.RESET)
            else:
                print()
    else:
        color = speaker_color(sid) if sid else C.NARRATOR
        for line in text.split("\n"):
            if line.strip():
                print(color + wrap(f'"{line.strip()}"', indent=4) + C.RESET)
            else:
                print()

    print()

def render_choices(choices, session):
    divider()
    print(f"  {C.BOLD}{C.CHOICE}WHAT DO YOU DO?{C.RESET}")
    print()

    for i, choice in enumerate(choices):
        idx_str  = f"{C.CHOICE}{C.BOLD}  [{i+1}]{C.RESET}"
        text     = apply_code_switching(choice.get("text", ""))
        comm_imp = choice.get("communityImpact", "")
        indv_imp = choice.get("individualImpact", "")
        coll_d   = choice.get("collectivityDelta", 0)

        print(f"{idx_str}  {C.BOLD}{text}{C.RESET}")

        if comm_imp:
            print(f"       {C.COMM}⬡ Community:{C.RESET}  {wrap(comm_imp, indent=18).lstrip()}")
        if indv_imp:
            print(f"       {C.INDV}◈ Personal:{C.RESET}   {wrap(indv_imp, indent=18).lstrip()}")

        # Collectivity indicator
        if coll_d != 0:
            arrow = "▲ collective" if coll_d > 0 else "▼ individual"
            color = C.COMM if coll_d > 0 else C.INDV
            print(f"       {color}{arrow}  ({'+' if coll_d > 0 else ''}{coll_d:.1f}){C.RESET}")
        print()

def render_call_and_response(session):
    """Show call-and-response affirmation prompts."""
    print()
    divider("·")
    print(f"  {C.TAG}◆ CALL & RESPONSE — How do you answer?{C.RESET}")
    print()
    for i, r in enumerate(CALL_AND_RESPONSE_REPLIES):
        print(f"  {C.CHOICE}[{i+1}]{C.RESET}  {r}")
    print()
    if AUTO_INPUTS:
        choice = AUTO_INPUTS.pop(0) - 1
        print(f"\n  {C.CHOICE}> {C.RESET}{choice+1}  {C.DIM}(auto){C.RESET}")
    else:
        choice = get_input(len(CALL_AND_RESPONSE_REPLIES))
    print(f"\n  {C.DIM}You say: \"{CALL_AND_RESPONSE_REPLIES[choice]}\"{C.RESET}")
    if IS_TTY: time.sleep(1.2)

def get_input(max_choices):
    if AUTO_INPUTS:
        val = AUTO_INPUTS.pop(0)
        print(f"\n  {C.CHOICE}> {C.RESET}{val}  {C.DIM}(auto){C.RESET}")
        time.sleep(0.3)
        return val - 1
    while True:
        try:
            raw = input(f"\n  {C.CHOICE}> {C.RESET}").strip()
            if raw == "": return -1
            n = int(raw)
            if 1 <= n <= max_choices:
                return n - 1
        except (ValueError, EOFError):
            pass
        print(f"  {C.WARN}Enter a number between 1 and {max_choices}.{C.RESET}")

def get_advance():
    if AUTO_INPUTS is not None and not IS_TTY:
        time.sleep(0.15)
        return
    input(f"  {C.DIM}[ Press ENTER to continue ]{C.RESET}")


# ── Main game loop ───────────────────────────────────────────────────────

def run():
    # Load story
    with open(STORY_PATH, encoding="utf-8") as f:
        story = json.load(f)

    nodes = {n["id"]: n for n in story["nodes"]}
    session = Session()

    print_title_card()

    current_id = story["startNodeId"]

    while current_id:
        node = nodes.get(current_id)
        if not node:
            print(f"{C.WARN}[ERROR] Node '{current_id}' not found.{C.RESET}")
            break

        # Apply entry triggers
        for trig in node.get("triggers", []):
            session.apply_trigger(trig)

        session.mark_visited(current_id)

        render_node(node, session)

        # Call-and-response pattern
        tags = node.get("culturalTags", [])
        if "call_and_response" in tags or "testimony" in tags:
            render_call_and_response(session)
            clear()
            render_node(node, session)

        # End node
        if node.get("type") == "end":
            print_ending(session)
            break

        # Evaluate choices
        choices = node.get("choices", [])
        available = []
        for c in choices:
            if all(session.evaluate_condition(cond) for cond in c.get("conditions", [])):
                available.append(c)

        if available:
            render_choices(available, session)
            idx = get_input(len(available))
            chosen = available[idx]

            # Apply choice triggers
            for trig in chosen.get("triggers", []):
                session.apply_trigger(trig)
            session.apply_collectivity(chosen.get("collectivityDelta", 0))
            session.choice_history.append(chosen.get("targetNodeId", ""))

            current_id = chosen.get("targetNodeId")
        else:
            next_id = node.get("nextNodeId", "")
            if next_id:
                get_advance()
                current_id = next_id
            else:
                print_ending(session)
                break


def print_ending(session):
    trust   = session.get_community("freedens_town")
    coll    = session.collectivity
    choices = len(session.choice_history)

    print()
    divider("═")
    print(C.TITLE + C.BOLD + "END OF STORY".center(WIDTH) + C.RESET)
    divider("═")
    print()

    # Community outcome
    if trust >= 70:
        outcome = f"{C.GOOD}Freedmen's Town is standing. {trust:.0f}/100 — the community endures.{C.RESET}"
    elif trust >= 40:
        outcome = f"{C.CHOICE}Freedmen's Town is shaken but alive. {trust:.0f}/100 — uncertain days ahead.{C.RESET}"
    else:
        outcome = f"{C.WARN}Freedmen's Town is in crisis. {trust:.0f}/100 — the community is scattered.{C.RESET}"

    print(f"  {outcome}")
    print()

    # Collectivity summary
    if coll >= 2:
        stance = f"{C.COMM}Isaiah chose his people over himself. The community feels it.{C.RESET}"
    elif coll >= 0:
        stance = f"{C.CHOICE}Isaiah walked the line between self and community.{C.RESET}"
    else:
        stance = f"{C.INDV}Isaiah rode alone. The score is settled — or it isn't.{C.RESET}"

    print(f"  {stance}")
    print()

    # Flags summary
    milestones = []
    if session.has_flag("chose_community_first"):   milestones.append("Turned back for the community")
    if session.has_flag("community_survived"):       milestones.append("Freedmen's Town survived the standoff")
    if session.has_flag("arrested_vance"):           milestones.append("Put Vance in irons — by the law")
    if session.has_flag("shot_vance"):               milestones.append("Settled it with a gun")
    if session.has_flag("bore_witness"):             milestones.append("Named his parents. Made Vance look.")
    if session.has_flag("community_blessing"):       milestones.append("Rode with Ma Ellen's blessing")
    if session.has_flag("chose_vance_first"):        milestones.append("Chased Vance before coming home")

    if milestones:
        print(f"  {C.BOLD}Path taken:{C.RESET}")
        for m in milestones:
            print(f"    {C.DIM}·  {m}{C.RESET}")
        print()

    print(f"  {C.DIM}{choices} choices made.{C.RESET}")
    print()
    divider()
    print(C.DIM + "  This story was built with the Minority Narrative Engine Toolkit.".center(WIDTH))
    print("  Built for storytellers who carry something worth telling.".center(WIDTH))
    print(("  " + "— Thank the people who told you your first story.").center(WIDTH) + C.RESET)
    divider()
    print()


# ── Auto-play sequence ───────────────────────────────────────────────────
# Path: ask Juniper → ride to community → stand ground → stay in Freedmen's Town
# Encodes: choice selections for every branching point + call-and-response replies
DEMO_AUTO_PATH = [
    3,  # first_choice: ask Juniper for advice
    1,  # first_choice_after_advice: ride to community
    1,  # maellen_greeting C&R reply
    1,  # maellen_situation C&R reply
    1,  # community_choice: stand your ground
    1,  # stand_ground C&R reply
    1,  # maellen_aftermath C&R reply
    1,  # final_choice_community: stay in Freedmen's Town
]

if __name__ == "__main__":
    try:
        if not IS_TTY:
            AUTO_INPUTS.extend(DEMO_AUTO_PATH)
        run()
    except KeyboardInterrupt:
        print(f"\n\n  {C.DIM}Ride cut short. Isaiah keeps moving.{C.RESET}\n")
