using System.Collections.Generic;
using UnityEngine;

namespace ConquerChronicles.Gameplay.Animation
{
    /// <summary>
    /// Loads and caches sprites from a TexturePacker atlas by name prefix.
    /// Sprites are loaded from Resources or directly from the atlas texture.
    /// </summary>
    public static class SpriteAtlasLoader
    {
        private static readonly Dictionary<string, Sprite[]> _cache = new();
        private static Sprite[] _allSprites;
        private static bool _loaded;

        private const string AtlasPath = "Atlases/GameAtlas";

        /// <summary>
        /// Loads all sprites from the atlas and caches them.
        /// Call once at startup or first use.
        /// </summary>
        public static int SpriteCount => _allSprites != null ? _allSprites.Length : 0;

        public static void EnsureLoaded()
        {
            if (_loaded) return;

            _allSprites = Resources.LoadAll<Sprite>(AtlasPath);
            if (_allSprites == null || _allSprites.Length == 0)
            {
                Debug.LogWarning($"[SpriteAtlasLoader] No sprites found at Resources/{AtlasPath}. " +
                    "Make sure the atlas is in a Resources folder or use LoadFromAtlas() directly.");
                _allSprites = System.Array.Empty<Sprite>();
            }
            else
            {
                // Log first few sprite names for diagnostics
                string names = "";
                for (int i = 0; i < Mathf.Min(5, _allSprites.Length); i++)
                    names += _allSprites[i].name + ", ";
                Debug.Log($"[SpriteAtlasLoader] Loaded {_allSprites.Length} sprites. First: {names}");
            }

            _loaded = true;
        }

        /// <summary>
        /// Manually provide all sprites from the atlas (e.g. loaded via AssetDatabase in editor).
        /// </summary>
        public static void SetSprites(Sprite[] sprites)
        {
            _allSprites = sprites ?? System.Array.Empty<Sprite>();
            _loaded = true;
            _cache.Clear();
        }

        /// <summary>
        /// Gets animation frames matching a name prefix, sorted by name.
        /// Example: GetFrames("Male_Base_EAttack_") returns frames 01, 02, 03, 04.
        /// </summary>
        public static Sprite[] GetFrames(string prefix)
        {
            if (_cache.TryGetValue(prefix, out var cached))
                return cached;

            EnsureLoaded();

            var frames = new List<Sprite>();
            for (int i = 0; i < _allSprites.Length; i++)
            {
                if (_allSprites[i].name.StartsWith(prefix))
                    frames.Add(_allSprites[i]);
            }

            // Sort by name to ensure correct frame order
            frames.Sort((a, b) => string.CompareOrdinal(a.name, b.name));

            var result = frames.ToArray();
            _cache[prefix] = result;
            return result;
        }

        /// <summary>
        /// Gets a single sprite by exact name.
        /// </summary>
        public static Sprite GetSprite(string name)
        {
            EnsureLoaded();

            for (int i = 0; i < _allSprites.Length; i++)
            {
                if (_allSprites[i].name == name)
                    return _allSprites[i];
            }

            return null;
        }

        /// <summary>
        /// Gets a specific idle direction frame.
        /// directionIndex: 0=S, 1=SE, 2=E, 3=NE, 4=N, 5=NW, 6=W, 7=SW
        /// </summary>
        public static Sprite GetIdleDirection(string prefix, int directionIndex)
        {
            var frames = GetFrames(prefix);
            if (frames.Length == 0) return null;
            int idx = Mathf.Clamp(directionIndex, 0, frames.Length - 1);
            return frames[idx];
        }

        /// <summary>
        /// Clears all cached data. Call if re-importing atlas.
        /// </summary>
        public static void ClearCache()
        {
            _cache.Clear();
            _allSprites = null;
            _loaded = false;
        }
    }
}
