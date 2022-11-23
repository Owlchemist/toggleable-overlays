using HarmonyLib;
using Verse;
using RimWorld;
using static ToggleableOverlays.ModSettings_ToggleableOverlays;
using static ToggleableOverlays.ToggleableOverlaysUtility;
 
namespace ToggleableOverlays
{
    //Pawn names
    [HarmonyPatch (typeof(Pawn), nameof(Pawn.DrawGUIOverlay))]
    static class Patch_Pawn_DrawGUIOverlay
    {
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
        static bool Prefix(Thing __instance)
        {
            return CheckZoomFirst(CameraZoomRange.Closest) ? CheckMouseOver(__instance, hideBedAssignment) : false;
        }
    }

    //Item qualities and quanities, bed and throne assignments, storage buildings
    [HarmonyPatch (typeof(ThingWithComps), nameof(ThingWithComps.DrawGUIOverlay))]
    static class Patch_ThingWithComps_DrawGUIOverlay
    {
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

    //Power overlay: building lacks power
    [HarmonyPatch (typeof(OverlayDrawer), nameof(OverlayDrawer.RenderNeedsPowerOverlay))]
    static class Patch_OverlayDrawer_RenderNeedsPowerOverlay
    {
        static bool Prefix(Thing t)
        {
            return CheckZoomFirst(CameraZoomRange.Middle) ? CheckMouseOver(t, hidePowerWarnings) : false;
        }
    }

    //Power overlay: building disconnected
    [HarmonyPatch (typeof(OverlayDrawer), nameof(OverlayDrawer.RenderPowerOffOverlay))]
    static class Patch_OverlayDrawer_RenderPowerOffOverlay
    {
        static bool Prefix(Thing t)
        {
            return CheckZoomFirst(CameraZoomRange.Middle) ? CheckMouseOver(t, hidePowerWarnings) : false;
        }
    }

    //Fuel overlay: Building lacks fuel
    [HarmonyPatch (typeof(OverlayDrawer), nameof(OverlayDrawer.RenderOutOfFuelOverlay))]
    static class Patch_OverlayDrawer_RenderOutOfFuelOverlay
    {
        static bool Prefix(Thing t)
        {
            return CheckZoomFirst(CameraZoomRange.Middle) ? CheckMouseOver(t, hideFuelWarnings) : false;
        }
    }

    //Fuel overlay: do not refuel
    [HarmonyPatch (typeof(OverlayDrawer), nameof(OverlayDrawer.RenderForbiddenRefuelOverlay))]
    static class Patch_OverlayDrawer_RenderForbiddenRefuelOverlay
    {
        static bool Prefix(Thing t)
        {
            return CheckZoomFirst(CameraZoomRange.Middle) ? CheckMouseOver(t, hideFuelWarnings) : false;
        }
    }

    //Forbidden overlay (the red X)
    [HarmonyPatch (typeof(OverlayDrawer), nameof(OverlayDrawer.RenderForbiddenBigOverlay))]
    static class Patch_OverlayDrawer_RenderForbiddenBigOverlay
    {
        static bool Prefix(Thing t)
        {
            return CheckZoomFirst(CameraZoomRange.Middle) ? CheckMouseOver(t, hideForbiddenBuildings) : false;
        }
    }

    //Broken down overlay
    [HarmonyPatch (typeof(OverlayDrawer), nameof(OverlayDrawer.RenderBrokenDownOverlay))]
    static class Patch_OverlayDrawer_RenderBrokenDownOverlay
    {
        static bool Prefix(Thing t)
        {
            return CheckZoomFirst(CameraZoomRange.Far) ? CheckMouseOver(t, hideBrokenDown) : false;
        }
    }

     //Forbidden overlay (the red X)
    [HarmonyPatch (typeof(OverlayDrawer), nameof(OverlayDrawer.RenderForbiddenOverlay))]
    static class Patch_OverlayDrawer_RenderForbiddenOverlay
    {
        static bool Prefix(Thing t)
        {
            if (!hideForbidden && !hideForbiddenBuildings) return true;
            if (!CheckZoomFirst(CameraZoomRange.Middle)) return false;
            
            if (t.def.category == ThingCategory.Item) return CheckMouseOver(t, hideForbidden, true);
            if (t.def.category == ThingCategory.Ethereal || t.def.category == ThingCategory.Building) return CheckMouseOver(t, hideForbiddenBuildings);
            return true;
        }
    }
}