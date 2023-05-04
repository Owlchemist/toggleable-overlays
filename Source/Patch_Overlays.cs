using HarmonyLib;
using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using static ToggleableOverlays.ModSettings_ToggleableOverlays;
using static ToggleableOverlays.ToggleableOverlaysUtility;
 
namespace ToggleableOverlays
{
    //Power overlay: building lacks power
    [HarmonyPatch (typeof(OverlayDrawer), nameof(OverlayDrawer.RenderNeedsPowerOverlay))]
    static class Patch_OverlayDrawer_RenderNeedsPowerOverlay
    {
        static bool Prepare()
        {
            return hidePowerWarnings;
        }
        static bool Prefix(Thing t)
        {
            return quickView || (CheckZoomFirst(CameraZoomRange.Middle) && CheckMouseOver(t));
        }
    }

    //Power overlay: building disconnected
    [HarmonyPatch (typeof(OverlayDrawer), nameof(OverlayDrawer.RenderPowerOffOverlay))]
    static class Patch_OverlayDrawer_RenderPowerOffOverlay
    {
        static bool Prepare()
        {
            return hidePowerWarnings;
        }
        static bool Prefix(Thing t)
        {
            return quickView || (CheckZoomFirst(CameraZoomRange.Middle) && CheckMouseOver(t));
        }
    }

    //Fuel overlay: Building lacks fuel
    [HarmonyPatch (typeof(OverlayDrawer), nameof(OverlayDrawer.RenderOutOfFuelOverlay))]
    static class Patch_OverlayDrawer_RenderOutOfFuelOverlay
    {
        static bool Prepare()
        {
            return hideFuelWarnings;
        }
        static bool Prefix(Thing t)
        {
            return quickView || (CheckZoomFirst(CameraZoomRange.Middle) && CheckMouseOver(t));
        }
    }

    //Fuel overlay: do not refuel
    [HarmonyPatch (typeof(OverlayDrawer), nameof(OverlayDrawer.RenderForbiddenRefuelOverlay))]
    static class Patch_OverlayDrawer_RenderForbiddenRefuelOverlay
    {
        static bool Prepare()
        {
            return hideFuelWarnings;
        }
        static bool Prefix(Thing t)
        {
            return quickView || (CheckZoomFirst(CameraZoomRange.Middle) && CheckMouseOver(t));
        }
    }

    //Forbidden overlay (the red X)
    [HarmonyPatch (typeof(OverlayDrawer), nameof(OverlayDrawer.RenderForbiddenBigOverlay))]
    static class Patch_OverlayDrawer_RenderForbiddenBigOverlay
    {
        static bool Prepare()
        {
            return hideForbiddenBuildings;
        }
        static bool Prefix(Thing t)
        {
            return quickView || (CheckZoomFirst(CameraZoomRange.Middle) && CheckMouseOver(t));
        }
    }

    //Broken down overlay
    [HarmonyPatch (typeof(OverlayDrawer), nameof(OverlayDrawer.RenderBrokenDownOverlay))]
    static class Patch_OverlayDrawer_RenderBrokenDownOverlay
    {
        static bool Prepare()
        {
            return hideBrokenDown;
        }
        static bool Prefix(Thing t)
        {
            return quickView || (CheckZoomFirst(CameraZoomRange.Far) && CheckMouseOver(t));
        }
    }

    //Forbidden overlay (the red X)
    [HarmonyPatch (typeof(OverlayDrawer), nameof(OverlayDrawer.RenderForbiddenOverlay))]
    static class Patch_OverlayDrawer_RenderForbiddenOverlay
    {
        static bool Prepare()
        {
            return hideForbidden || hideForbiddenBuildings;
        }
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var label = generator.DefineLabel();
            foreach (var instruction in instructions) if (instruction.opcode == OpCodes.Ret) { instruction.labels.Add(label); break; }

            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_OverlayDrawer_RenderForbiddenOverlay), nameof(CheckRenderForbiddenOverlay)));
            yield return new CodeInstruction(OpCodes.Brfalse, label);

            foreach (var instruction in instructions) yield return instruction;
        }
        static bool CheckRenderForbiddenOverlay(Thing t)
        {
            if (!CheckZoomFirst(CameraZoomRange.Middle)) return false;
            
            if (t.def.category == ThingCategory.Item) return CheckMouseOver(t, hideForbidden, true);
            if (!quickView && (hideForbiddenBuildings && (t.def.category == ThingCategory.Ethereal || t.def.category == ThingCategory.Building))) return CheckMouseOver(t);
            return true;
        }
    }
}