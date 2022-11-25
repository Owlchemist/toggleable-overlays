using HarmonyLib;
using Verse;
using RimWorld;
using System.Collections.Generic;
using static ToggleableOverlays.ModSettings_ToggleableOverlays;
using static ToggleableOverlays.ToggleableOverlaysUtility;
 
namespace ToggleableOverlays
{
	//Fuel gauge
    [HarmonyPatch (typeof(CompRefuelable), nameof(CompRefuelable.PostDraw))]
    static class Patch_CompRefuelable
    {
        static bool Prepare()
        {
            return hideFuelGauge;
        }
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(AccessTools.Method(typeof(GenDraw), nameof(GenDraw.DrawFillableBar)),
				AccessTools.Method(typeof(Patch_CompRefuelable), nameof(Patch_CompRefuelable.DrawFillableBar)));
        }

        static void DrawFillableBar(GenDraw.FillableBarRequest r)
        {
            if (CheckZoomFirst(CameraZoomRange.Middle) && CheckMouseOver(r, hideFuelGauge)) GenDraw.DrawFillableBar(r);
        }
    }

    //Wind gauge
    [HarmonyPatch (typeof(CompPowerPlantWind), nameof(CompPowerPlantWind.PostDraw))]
    static class Patch_CompPowerPlantWind
    {
        static bool Prepare()
        {
            return hideWindGauge;
        }
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(AccessTools.Method(typeof(GenDraw), nameof(GenDraw.DrawFillableBar)),
				AccessTools.Method(typeof(Patch_CompPowerPlantWind), nameof(Patch_CompPowerPlantWind.DrawFillableBar)));
        }

        static void DrawFillableBar(GenDraw.FillableBarRequest r)
        {
            if (CheckZoomFirst(CameraZoomRange.Middle) && CheckMouseOver(r, hideWindGauge)) GenDraw.DrawFillableBar(r);
        }
    }

    //Mote process bar
    [HarmonyPatch (typeof(MoteProgressBar), nameof(MoteProgressBar.Draw))]
    static class Patch_MoteProgressBar
    {
        static bool Prepare()
        {
            return hideMoteProgress;
        }
        static bool Prefix(MoteProgressBar __instance)
        {
            return CheckZoomFirst(CameraZoomRange.Closest) && CheckMouseOver(__instance, hideMoteProgress);
        }
    }

    //Universal hide gauge
    [HarmonyPatch (typeof(GenDraw), nameof(GenDraw.DrawFillableBar))]
    static class Patch_DrawFillableBar
    {
        static bool Prepare()
        {
            return hideAllGauges;
        }
        static bool Prefix(GenDraw.FillableBarRequest r)
        {
            return (CheckZoomFirst(CameraZoomRange.Middle) && CheckMouseOver(r, hideAllGauges));
        }
    }
}