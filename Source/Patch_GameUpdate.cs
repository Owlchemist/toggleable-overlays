using HarmonyLib;
using Verse;
using UnityEngine;
using static ToggleableOverlays.ToggleableOverlaysUtility;
 
namespace ToggleableOverlays
{
    [HarmonyPatch (typeof(GameComponentUtility), nameof(GameComponentUtility.GameComponentUpdate))]
    static class Patch_GameComponentUpdate
    {
        static void Postfix()
        {
            if (Current.programStateInt == ProgramState.Playing)
            {
                Vector3 tmp;
				Input.get_mousePosition_Injected(out tmp);
                Ray ray;
                Vector2 tmp2 = (Vector2)tmp;
			    Current.cameraDriverInt.cachedCamera.ScreenPointToRay_Injected(ref tmp2, Camera.MonoOrStereoscopicEye.Mono, out ray);

                mousePosition.x = (int)ray.m_Origin.x;
                mousePosition.z = (int)ray.m_Origin.z;
                currentCameraZoom = Current.cameraDriverInt.CurrentZoom;
                quickView = Hotkeys.QuickShowKey.IsDown && ModSettings_ToggleableOverlays.quickShowEnabled;
            }
        }
    }
}