using HarmonyLib;
using Verse;
using RimWorld;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using static ToggleableOverlays.ModSettings_ToggleableOverlays;
using static ToggleableOverlays.ToggleableOverlaysUtility;
 
namespace ToggleableOverlays
{
    //Pawn names
    [HarmonyPatch (typeof(Pawn), nameof(Pawn.DrawGUIOverlay))]
    static class Patch_Pawn_DrawGUIOverlay
    {
        static bool Prepare()
        {
            return !drawAllPawns;
        }
        static bool Prefix(Pawn __instance)
        {
            if (!quickView && !drawAllPawns && __instance.trader == null)
            {
                Faction faction = __instance.factionInt;
                int x = __instance.positionInt.x;
                int z = __instance.positionInt.z;
                if (faction == null)
                {
                    if (__instance.def.race.intelligence < Intelligence.Humanlike) 
                    {
                        return __instance.nameInt != null;
                    }
                    //Factionless pawns. May happen if debug spawned or some events.
                    else return !hideFriendlyPawns || x == mousePositionX && z == mousePositionZ;
                }
                
                if (__instance.guest != null)
                {
                    if (__instance.guest.guestStatusInt == GuestStatus.Prisoner)
                    {
                        return !hidePrisonerPawns || x == mousePositionX && z == mousePositionZ;
                    }
                    if ( __instance.guest.guestStatusInt == GuestStatus.Slave)
                    {
                        return !hideSlavePawns || x == mousePositionX && z == mousePositionZ;
                    }
                }
                if (faction.def.isPlayer)
                {
                    return (!hidePlayerPawns || (!hideDraftedPawns && __instance.Drafted) || (x == mousePositionX && z == mousePositionZ));
                }
                if (faction.HostileTo(Current.gameInt.worldInt.factionManager.ofPlayer))
                {
                    return !hideHostilePawns || x == mousePositionX && z == mousePositionZ;
                }
                //Assume neutral/friendly
                if (!hideFriendlyPawns) return true;

                return x == mousePositionX && z == mousePositionZ;
            }
            return true;  
        }
    }


    //Bed assignments
    [HarmonyPatch (typeof(Building_Bed), nameof(Building_Bed.DrawGUIOverlay))]
    static class Patch_Building_Bed_DrawGUIOverlay
    {
        static bool Prepare()
        {
            return hideBedAssignment;
        }
        static bool Prefix(Thing __instance)
        {
            return (quickView || (CheckZoomFirst(CameraZoomRange.Closest) && CheckMouseOver(__instance)));
        }
    }

    //Item qualities and quanities, bed and throne assignments, storage buildings
    [HarmonyPatch (typeof(ThingWithComps), nameof(ThingWithComps.DrawGUIOverlay))]
    static class Patch_ThingWithComps_DrawGUIOverlay
    {
        static bool Prepare()
        {
            return hideItems || hideStorageBuilding || hideThroneAssignment;
        }
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var label = generator.DefineLabel();
            foreach (var instruction in instructions) if (instruction.opcode == OpCodes.Ret) { instruction.labels.Add(label); break; }

            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_ThingWithComps_DrawGUIOverlay), nameof(CheckDrawGUIOverlay)));
            yield return new CodeInstruction(OpCodes.Brfalse, label);

            foreach (var instruction in instructions) yield return instruction;
        }
        static bool CheckDrawGUIOverlay(Thing __instance)
        {
            if (currentCameraZoom != CameraZoomRange.Closest) return false;
            
            if (__instance.def.category == ThingCategory.Item) return CheckMouseOver(__instance, hideItems, true);
            else if (__instance.def.category == ThingCategory.Building)
            {
                System.Type type = __instance.def.thingClass;
                
                if (type == typeof(Building_Storage) || type == typeof(Building)) return CheckMouseOver(__instance, hideStorageBuilding);
                if (type == typeof(Building_Throne)) return CheckMouseOver(__instance, hideThroneAssignment);
            }
            return true;
        }
    }

	//Item stackable chunks, if relevant
    [HarmonyPatch (typeof(Thing), nameof(Thing.DrawGUIOverlay))]
    [HarmonyPriority(Priority.Last)]
    static class Patch_Thing_DrawGUIOverlay
    {
        static bool Prepare()
        {
            var list = DefDatabase<ThingDef>.AllDefsListForReading;
            var length = list.Count;
            for (int i = 0; i < length; i++)
            {
                var def = list[i];
                if ((def.thingCategories?.Contains(ResourceBank.ThingCategoryDefOf.Chunks) ?? false) && def.drawGUIOverlayQuality)
                {
                    return true;
                }
            }
            return false;
        }
        static bool Prefix(Thing __instance)
        {
            if (__instance.def.category == ThingCategory.Item) return CheckZoomFirst(CameraZoomRange.Closest) ? CheckMouseOver(__instance, hideItems) : false;
            return true;
        }
    }

    //Hospitality's guest beds
    [HarmonyPatch]
    static class Patch_Building_GuestBed
    {
        static MethodBase target;

        static bool Prepare()
        {
            if (hideBedAssignment == false) return false;
            
            target = AccessTools.DeclaredMethod(AccessTools.TypeByName("Hospitality.Building_GuestBed"), "DrawGUIOverlay");
            return target != null;
        }

        static MethodBase TargetMethod()
        {
            return target;
        }

        static bool Prefix(Thing __instance)
        {
            return (quickView || (CheckZoomFirst(CameraZoomRange.Closest) && CheckMouseOver(__instance)));
        }
    }
}