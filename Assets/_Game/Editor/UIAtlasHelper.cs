using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

namespace ConquerChronicles.Editor
{
    public static class UIAtlasHelper
    {
        private const string AtlasPath = "Assets/Resources/Atlases/GameAtlas.png";
        private static Dictionary<string, Sprite> _spriteCache;

        public static void EnsureLoaded()
        {
            if (_spriteCache != null) return;
            _spriteCache = new Dictionary<string, Sprite>();
            var assets = AssetDatabase.LoadAllAssetsAtPath(AtlasPath);
            foreach (var asset in assets)
            {
                if (asset is Sprite s)
                    _spriteCache[s.name] = s;
            }
        }

        public static void ClearAndReload()
        {
            _spriteCache = null;
            EnsureLoaded();
        }

        public static Sprite GetSprite(string name)
        {
            EnsureLoaded();
            _spriteCache.TryGetValue(name, out var sprite);
            return sprite;
        }

        public static void ClearCache() => _spriteCache = null;

        /// <summary>Sets an Image to 9-sliced Panels sprite with optional tint.</summary>
        public static void SetSlicedPanel(Image img, Color tint = default)
        {
            var sprite = GetSprite("Panels");
            if (sprite != null)
            {
                img.sprite = sprite;
                img.type = Image.Type.Sliced;
                img.pixelsPerUnitMultiplier = 1f;
            }
            img.color = tint == default ? Color.white : tint;
        }

        /// <summary>Configures a Button with SpriteSwap using unpressed/pressed sprite pair.</summary>
        public static void SetSpriteSwapButton(
            Button btn, Image targetImage,
            string unpressedName, string pressedName,
            Color tint = default)
        {
            var unpressed = GetSprite(unpressedName);
            var pressed = GetSprite(pressedName);

            if (unpressed != null)
            {
                targetImage.sprite = unpressed;
                targetImage.type = Image.Type.Sliced;
                targetImage.pixelsPerUnitMultiplier = 1f;
            }
            targetImage.color = tint == default ? Color.white : tint;

            btn.transition = Selectable.Transition.SpriteSwap;
            btn.targetGraphic = targetImage;

            if (pressed != null)
            {
                var spriteState = new SpriteState();
                spriteState.pressedSprite = pressed;
                btn.spriteState = spriteState;
            }
        }

        /// <summary>Configures a close/X Button with SpriteSwap (Simple mode, not sliced).</summary>
        public static void SetXButton(Button btn, Image targetImage)
        {
            var unpressed = GetSprite("XButton_Unpressed");
            var pressed = GetSprite("XButton_Pressed");

            if (unpressed != null)
            {
                targetImage.sprite = unpressed;
                targetImage.type = Image.Type.Simple;
                targetImage.preserveAspect = true;
            }
            targetImage.color = Color.white;

            btn.transition = Selectable.Transition.SpriteSwap;
            btn.targetGraphic = targetImage;

            if (pressed != null)
            {
                var spriteState = new SpriteState();
                spriteState.pressedSprite = pressed;
                btn.spriteState = spriteState;
            }
        }

        /// <summary>Sets an Image to a specific sprite in Simple mode.</summary>
        public static void SetSimpleSprite(Image img, string spriteName, Color tint = default)
        {
            var sprite = GetSprite(spriteName);
            if (sprite != null)
            {
                img.sprite = sprite;
                img.type = Image.Type.Simple;
                img.preserveAspect = true;
            }
            img.color = tint == default ? Color.white : tint;
        }

        // Panel 9-slice border values (from Aseprite slices via TexturePacker)
        public const float PanelPadL = 65f;
        public const float PanelPadR = 63f;
        public const float PanelPadT = 61f;
        public const float PanelPadB = 64f;

        // Button 9-slice border values (using Unpressed values)
        public const float ButtonPadL = 50f;
        public const float ButtonPadR = 49f;
        public const float ButtonPadT = 53f;
        public const float ButtonPadB = 52f;

        /// <summary>
        /// Creates an inset content area child inside a panel or button.
        /// All content should be parented to this instead of the panel/button directly.
        /// </summary>
        public static RectTransform CreateContentArea(Transform parent, float left, float right, float top, float bottom)
        {
            var go = new GameObject("Content", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(left, bottom);
            rt.offsetMax = new Vector2(-right, -top);
            return rt;
        }

        /// <summary>
        /// Adds a tiled UI_tile background inside a panel, behind content.
        /// Call this BEFORE CreatePanelContent so it renders behind.
        /// </summary>
        public static void AddTiledBackground(Transform parent, Color tint = default)
        {
            var sprite = GetSprite("UI_tile");
            if (sprite == null) return;

            var go = new GameObject("TiledBg", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(PanelPadL, PanelPadB);
            rt.offsetMax = new Vector2(-PanelPadR, -PanelPadT);
            var img = go.AddComponent<Image>();
            img.sprite = sprite;
            img.type = Image.Type.Tiled;
            img.color = tint == default ? new Color(1f, 1f, 1f, 0.08f) : tint;
            img.raycastTarget = false;
        }

        /// <summary>Creates a content area with full panel padding.</summary>
        public static RectTransform CreatePanelContent(Transform parent)
        {
            return CreateContentArea(parent, PanelPadL, PanelPadR, PanelPadT, PanelPadB);
        }

        /// <summary>Creates a content area with button padding. Vertical padding is capped for short buttons.</summary>
        public static RectTransform CreateButtonContent(Transform parent, float height = 0f)
        {
            float vPad = ButtonPadT;
            if (height > 0f && height < ButtonPadT + ButtonPadB)
                vPad = Mathf.Max(4f, height * 0.15f);
            return CreateContentArea(parent, ButtonPadL, ButtonPadR, vPad, vPad);
        }
    }

}
