using HarmonyLib;
using Verse;
using UnityEngine;
using static ToggleableOverlays.ToggleableOverlaysUtility;
using static ToggleableOverlays.ModSettings_ToggleableOverlays;
 
namespace ToggleableOverlays
{
    //This patch is responsible for caching the mouse position, camera zoom, and hotkey handler
    [HarmonyPatch (typeof(GameComponentUtility), nameof(GameComponentUtility.GameComponentUpdate))]
    static class Patch_GameComponentUpdate
    {
        static void Postfix()
        {
            if (Current.programStateInt == ProgramState.Playing)
            {
				Input.get_mousePosition_Injected(out Vector3 tmp);
                Vector2 tmp2 = (Vector2)tmp;
			    Current.cameraDriverInt.cachedCamera.ScreenPointToRay_Injected(ref tmp2, Camera.MonoOrStereoscopicEye.Mono, out Ray ray);

                mousePosition.x = mousePositionX = (int)ray.m_Origin.x;
                mousePosition.z = mousePositionZ = (int)ray.m_Origin.z;
                currentCameraZoom = Current.cameraDriverInt.CurrentZoom;
                if (quickShowAltMode)
                {
                    if (Hotkeys.QuickShowKey.JustPressed) quickView ^= true;
                }
                else quickView = Hotkeys.QuickShowKey.IsDown && ModSettings_ToggleableOverlays.quickShowEnabled;
            }
        }
    }
}