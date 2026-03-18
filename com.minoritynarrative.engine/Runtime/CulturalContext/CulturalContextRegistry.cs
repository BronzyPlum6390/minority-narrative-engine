using System.Collections.Generic;
using UnityEngine;

namespace MinorityNarrativeEngine
{
    /// <summary>
    /// Runtime registry mapping context keys to loaded CulturalContextBase ScriptableObjects.
    /// Populate this via the NarrativeEngine inspector or call Register() at startup.
    /// </summary>
    [CreateAssetMenu(menuName = "Minority Narrative/Cultural Context Registry")]
    public class CulturalContextRegistry : ScriptableObject
    {
        [Tooltip("All cultural context assets available at runtime. Add your configured contexts here.")]
        public List<CulturalContextBase> contexts = new List<CulturalContextBase>();

        private Dictionary<string, CulturalContextBase> _lookup;

        public void Initialize()
        {
            _lookup = new Dictionary<string, CulturalContextBase>();
            foreach (var ctx in contexts)
            {
                if (ctx == null) continue;
                if (_lookup.ContainsKey(ctx.contextKey))
                {
                    Debug.LogWarning($"[MNE] Duplicate context key '{ctx.contextKey}' in registry. Using first registered.");
                    continue;
                }
                _lookup[ctx.contextKey] = ctx;
            }
        }

        public CulturalContextBase Get(string contextKey)
        {
            if (_lookup == null) Initialize();
            _lookup.TryGetValue(contextKey, out var ctx);
            if (ctx == null)
                Debug.LogWarning($"[MNE] Cultural context '{contextKey}' not found in registry.");
            return ctx;
        }

        public bool TryGet(string contextKey, out CulturalContextBase context)
        {
            if (_lookup == null) Initialize();
            return _lookup.TryGetValue(contextKey, out context);
        }
    }
}
