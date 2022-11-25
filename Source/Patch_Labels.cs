using HarmonyLib;
using Verse;
using RimWorld;
using System.Linq;
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
            if (!quickView && !drawAllPawns)
            {
                Faction faction = __instance.Faction;
                if (faction == null)
                {
                    if (__instance.def.race.intelligence < Intelligence.Humanlike) return true;
                    //Factionless pawns. May happen if debug spawned or some events.
                    else return !hideFriendlyPawns || __instance.positionInt.x == mousePositionX && __instance.positionInt.z == mousePositionZ;
                }
                
                if (faction.def.isPlayer)
                {
                    return (!hidePlayerPawns || (!hideDraftedPawns && __instance.Drafted) || (__instance.positionInt.x == mousePositionX && __instance.positionInt.z == mousePositionZ));
                }
                if (__instance.guest != null && (__instance.guest.guestStatusInt == GuestStatus.Prisoner ||  __instance.guest.guestStatusInt == GuestStatus.Slave))
                {
                    return !hidePrisonerPawns || __instance.positionInt.x == mousePositionX && __instance.positionInt.z == mousePositionZ;
                }
                if (faction.HostileTo(Current.gameInt.worldInt.factionManager.ofPlayer))
                {
                    return !hideHostilePawns || __instance.positionInt.x == mousePositionX && __instance.positionInt.z == mousePositionZ;
                }
                //Assume neutral/friendly
                if (!hideFriendlyPawns) return true;

                //Make exception for trader pawns
                if (__instance.CanTradeNow) __instance.Map.overlayDrawer.DrawOverlay(__instance, OverlayTypes.QuestionMark);
                return __instance.positionInt.x == mousePositionX && __instance.positionInt.z == mousePositionZ;
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
            return !quickView && CheckZoomFirst(CameraZoomRange.Closest) && CheckMouseOver(__instance);
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
        static bool Prefix(Thing __instance)
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
    static class Patch_DrawGUIOverlay
    {
        static bool Prepare()
        {
            return DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.thingCategories?.Contains(ResourceBank.ThingCategoryDefOf.Chunks) ?? false).Any(y => y.drawGUIOverlayQuality == true);
        }
        static bool Prefix(Thing __instance)
        {
            if (__instance.def.category == ThingCategory.Item) return CheckZoomFirst(CameraZoomRange.Closest) ? CheckMouseOver(__instance, hideItems) : false;
            return true;
        }
    }
}