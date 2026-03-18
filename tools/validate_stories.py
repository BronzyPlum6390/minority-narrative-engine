#!/usr/bin/env python3
"""
Story JSON validator for Minority Narrative Engine.
Validates all .story.json files for:
  - Valid JSON syntax
  - Required fields (title, startNodeId, nodes)
  - All node references resolve (choice targets, nextNodeId)
  - No duplicate node IDs
  - Start node exists in node list
"""

import json
import os
import sys
import glob

REQUIRED_TOP_LEVEL = {"title", "startNodeId", "nodes"}
REQUIRED_NODE_FIELDS = {"id"}
VALID_NODE_TYPES = {"dialogue", "narration", "choice_hub", "choice_prompt", "end", None}

errors = []
warnings = []
files_checked = 0


def validate_story(filepath):
    global files_checked
    files_checked += 1
    rel = os.path.relpath(filepath)

    try:
        with open(filepath, "r", encoding="utf-8") as f:
            data = json.load(f)
    except json.JSONDecodeError as e:
        errors.append(f"{rel}: Invalid JSON — {e}")
        return

    # Required top-level fields
    for field in REQUIRED_TOP_LEVEL:
        if field not in data:
            errors.append(f"{rel}: Missing required field '{field}'")

    if not isinstance(data.get("nodes"), list):
        errors.append(f"{rel}: 'nodes' must be an array")
        return

    nodes = data["nodes"]
    start_id = data.get("startNodeId")

    # Build ID set and check for duplicates
    seen_ids = {}
    for i, node in enumerate(nodes):
        node_id = node.get("id")
        if not node_id:
            errors.append(f"{rel}: Node at index {i} missing 'id'")
            continue
        if node_id in seen_ids:
            errors.append(f"{rel}: Duplicate node id '{node_id}'")
        seen_ids[node_id] = True

    all_ids = set(seen_ids.keys())

    # Start node exists
    if start_id and start_id not in all_ids:
        errors.append(f"{rel}: startNodeId '{start_id}' not found in nodes")

    # Validate each node's references
    for node in nodes:
        node_id = node.get("id", f"<unknown@{nodes.index(node)}>")
        node_type = node.get("type")

        if node_type not in VALID_NODE_TYPES:
            warnings.append(f"{rel}: Node '{node_id}' has unknown type '{node_type}'")

        # nextNodeId reference
        next_id = node.get("nextNodeId")
        if next_id and next_id not in all_ids:
            errors.append(f"{rel}: Node '{node_id}' nextNodeId '{next_id}' not found")

        # Choice target references
        for j, choice in enumerate(node.get("choices", [])):
            target = choice.get("targetNodeId")
            if target and target not in all_ids:
                errors.append(
                    f"{rel}: Node '{node_id}' choice[{j}] targetNodeId '{target}' not found"
                )

        # Condition references (visited type references node IDs)
        for cond in node.get("entryConditions", []):
            if cond.get("type") == "visited":
                ref = cond.get("nodeId")
                if ref and ref not in all_ids:
                    warnings.append(
                        f"{rel}: Node '{node_id}' condition references unknown nodeId '{ref}'"
                    )


def main():
    # Find all story JSON files
    search_paths = [
        "**/*.story.json",
        "Samples~/**/*.json",
        "com.minoritynarrative.engine/**/*.story.json",
    ]

    story_files = []
    for pattern in search_paths:
        story_files.extend(glob.glob(pattern, recursive=True))

    # Deduplicate
    story_files = list(set(story_files))

    if not story_files:
        print("No story files found. Checking default locations...")
        default = os.path.join(
            "com.minoritynarrative.engine",
            "Samples~",
            "IsaiahFree",
            "IsaiahFree.story.json",
        )
        if os.path.exists(default):
            story_files = [default]
        else:
            print("No .story.json files found anywhere. Nothing to validate.")
            sys.exit(0)

    print(f"Validating {len(story_files)} story file(s)...\n")

    for filepath in sorted(story_files):
        validate_story(filepath)

    # Report
    if warnings:
        print(f"WARNINGS ({len(warnings)}):")
        for w in warnings:
            print(f"  ⚠  {w}")
        print()

    if errors:
        print(f"ERRORS ({len(errors)}):")
        for e in errors:
            print(f"  ✗  {e}")
        print(f"\n{files_checked} file(s) checked — {len(errors)} error(s), {len(warnings)} warning(s)")
        sys.exit(1)
    else:
        print(f"✓ All {files_checked} story file(s) valid — {len(warnings)} warning(s)")
        sys.exit(0)


if __name__ == "__main__":
    main()
