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
                if (faction == null && !__instance.RaceProps.Humanlike) return true;
                
                //Factionless pawns. May happen if debug spawned or some events.
                if (faction == null)
                {
                    return !hideFriendlyPawns || __instance.Position.x == mousePosition.x && __instance.Position.z == mousePosition.z;
                }
                if (faction.IsPlayer)
                {
                    return (!hidePlayerPawns || (!hideDraftedPawns && __instance.Drafted) || (__instance.Position.x == mousePosition.x && __instance.Position.z == mousePosition.z));
                }
                if (__instance.IsPrisoner || __instance.IsSlave)
                {
                    return !hidePrisonerPawns || __instance.Position.x == mousePosition.x && __instance.Position.z == mousePosition.z;
                }
                if (faction.HostileTo(Faction.OfPlayer))
                {
                    return !hideHostilePawns || __instance.Position.x == mousePosition.x && __instance.Position.z == mousePosition.z;
                }
                //Assume neutral/friendly
                if (faction != null)
                {
                    if (!hideFriendlyPawns) return true;
                    //Make exception for trader pawns
                    if (__instance.CanTradeNow) __instance.Map.overlayDrawer.DrawOverlay(__instance, OverlayTypes.QuestionMark);
                    return __instance.Position.x == mousePosition.x && __instance.Position.z == mousePosition.z;
                }
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

    //Item stackable chunks, if relevant
    [HarmonyPatch (typeof(Thing), nameof(Thing.DrawGUIOverlay))]
    static class Patch_Thing_DrawGUIOverlay
    {
        static bool Prefix(Thing __instance)
        {
            if (stackableChunks && __instance.def.category == ThingCategory.Item) return CheckZoomFirst(CameraZoomRange.Closest) ? CheckMouseOver(__instance, hideItems) : false;
            return true;
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
                System.Type type = __instance.GetType();
                
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