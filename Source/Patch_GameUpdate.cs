using HarmonyLib;
using Verse;
using static ToggleableOverlays.ToggleableOverlaysUtility;
 
namespace ToggleableOverlays
{
    [HarmonyPatch (typeof(GameComponentUtility), nameof(GameComponentUtility.GameComponentUpdate))]
    static class Patch_GameComponentUpdate
    {
        static void Postfix()
        {
            mousePosition = UI.MouseCell();
			currentCameraZoom = Find.CameraDriver?.CurrentZoom ?? CameraZoomRange.Close;
			quickView = Hotkeys.QuickShowKey.IsDown && ModSettings_ToggleableOverlays.quickShowEnabled;
        }
    }
}