using RimWorld;
using Verse;

namespace ToggleableOverlays
{
    public static class ResourceBank
    {
        [DefOf]
        public static class ThingCategoryDefOf
        {
            public static ThingCategoryDef Chunks;
        }
        [DefOf]
        public static class KeyBindingDefOf
        {
            static KeyBindingDefOf()
            {
                DefOfHelper.EnsureInitializedInCtor(typeof(KeyBindingDefOf));
            }
            public static KeyBindingDef QuickShowKey;
        }
    }
}