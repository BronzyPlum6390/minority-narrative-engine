# Contributing to Minority Narrative Engine

Thank you for your interest in contributing. This project exists to help indie devs from underrepresented communities tell their stories with culturally authentic tools.

## Before You Contribute

- Read the [full documentation](MINORITY_NARRATIVE_ENGINE.md) to understand the architecture
- Check open [Issues](https://github.com/BronzyPlum6390/minority-narrative-engine/issues) and [Discussions](https://github.com/BronzyPlum6390/minority-narrative-engine/discussions) before starting work
- For significant changes, open an issue first to discuss the approach

## Cultural Sensitivity

This project centers the lived experiences of underrepresented communities. When contributing to cultural contexts (Black American, Indigenous, Latinx, Southeast Asian, West African, Caribbean, South Asian, MENA, Afro-Latinx):

- **Do not add** cultural elements you haven't researched deeply
- **Do add** a note in your PR describing your connection to or research into the cultural context you're working on
- Accuracy matters — stereotypes are harmful and will be rejected
- When in doubt, open a discussion rather than submitting a PR

## Development Setup

### Requirements
- Unity 2022.3 LTS or newer
- Git
- Python 3.9+ (for story validation scripts)

### Getting Started

```bash
git clone https://github.com/BronzyPlum6390/minority-narrative-engine.git
cd minority-narrative-engine
git checkout v2-dev
```

Open `TestProject/` in Unity to run the test suite.

### Running Tests

**In Unity:**
1. Open `TestProject/` in Unity Editor
2. Window → General → Test Runner
3. Run All (EditMode)

**In CI:** Tests run automatically on every push and PR via GitHub Actions.

### Validating Story Files

```bash
python tools/validate_stories.py
```

### Running the Demo

```bash
python run_demo.py
```

## Branching

| Branch | Purpose |
|--------|---------|
| `main` | Stable releases |
| `v2-dev` | Active V2 development |

- Branch off `v2-dev` for new features: `feature/your-feature-name`
- Branch off `main` for hotfixes: `fix/your-fix-name`
- Submit PRs to `v2-dev` (not `main`) unless it's a critical bug fix

## Commit Style

Use clear, descriptive commits:
```
feat: add Yoruba honorific patterns to WestAfricanContext
fix: resolve null reference in OralTraditionProcessorRegistry.RunAll
test: add coverage for LocalizationManager.SetLanguage edge cases
docs: document V2 analytics API in MINORITY_NARRATIVE_ENGINE.md
```

## Pull Request Checklist

- [ ] Tests pass locally
- [ ] New features include tests
- [ ] Story files validated with `python tools/validate_stories.py`
- [ ] No new Unity compiler warnings introduced
- [ ] PR description explains the change and cultural context (if applicable)

## Adding a New Cultural Context

1. Create `Runtime/CulturalContext/Contexts/YourCultureContext.cs` extending `CulturalContextBase`
2. Define honorifics, code-switching rules, and oral tradition patterns
3. Add it to `CulturalContextRegistry`
4. Write at least one sample story node using the context in `Samples~/`
5. Add tests in `Tests/Runtime/CulturalContext/`
6. Document it in `MINORITY_NARRATIVE_ENGINE.md`

## Code of Conduct

Be kind, be respectful, and center the communities this project serves. Contributions that diminish, mock, or misrepresent any cultural group will be rejected.

Questions? Open a Discussion or email the maintainer.
