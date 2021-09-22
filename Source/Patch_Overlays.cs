using HarmonyLib;
using Verse;
using RimWorld;
using static ToggleableOverlays.ModSettings_ToggleableOverlays;
using static ToggleableOverlays.GameComponent_ToggleableOverlays;
 
namespace ToggleableOverlays
{
    //Pawn names
    [HarmonyPatch (typeof(Pawn), "DrawGUIOverlay")]
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
                    if (!hideFriendlyPawns || __instance.Position.x == mousePosition.x && __instance.Position.z == mousePosition.z) return true;
                    return false;
                }
                if (faction.IsPlayer)
                {
                    if (!hidePlayerPawns) return true;
                    if (!hideDraftedPawns && __instance.Drafted) return true;
                    if (__instance.Position.x == mousePosition.x && __instance.Position.z == mousePosition.z) return true;
                    return false;
                }
                if (__instance.IsPrisoner || __instance.IsSlave)
                {
                    if (!hidePrisonerPawns || __instance.Position.x == mousePosition.x && __instance.Position.z == mousePosition.z) return true;
                    return false;
                }
                if (faction.HostileTo(Faction.OfPlayer))
                {
                    if (!hideHostilePawns || __instance.Position.x == mousePosition.x && __instance.Position.z == mousePosition.z) return true;
                    return false;
                }
                //Assume neutral/friendly
                if (faction != null)
                {
                    if (!hideFriendlyPawns) return true;
                    //Make exception for trader pawns
                    if (__instance.CanTradeNow) __instance.Map.overlayDrawer.DrawOverlay(__instance, OverlayTypes.QuestionMark);
                    if (__instance.Position.x == mousePosition.x && __instance.Position.z == mousePosition.z) return true;

                    return false;
                }
            }
            return true;  
        }
    }

    //Bed assignments
    [HarmonyPatch (typeof(Building_Bed), "DrawGUIOverlay")]
    static class Patch_Building_Bed_DrawGUIOverlay
    {
        static bool Prefix(Thing __instance)
        {
            if (currentCameraZoom != CameraZoomRange.Closest) return false;
            return CheckMouseOver(__instance, hideBedAssignment);
        }
    }

    //Item qualities and quanities, bed and throne assignments, storage buildings
    [HarmonyPatch (typeof(ThingWithComps), "DrawGUIOverlay")]
    static class Patch_ThingWithComps
    {
        static bool Prefix(Thing __instance)
        {
            if (currentCameraZoom != CameraZoomRange.Closest) return false;
            
            if (__instance.def.category == ThingCategory.Item) return CheckMouseOver(__instance, hideItems, true);
            else if (__instance.def.category == ThingCategory.Building)
            {
                var type = __instance.GetType();
                
                if (type == typeof(Building_Storage) || type == typeof(Building)) return CheckMouseOver(__instance, hideStorageBuilding);
                if (type == typeof(Building_Throne)) return CheckMouseOver(__instance, hideThroneAssignment);
            }
            return true;
        }
    }

    //Power overlay: building lacks power
    [HarmonyPatch (typeof(OverlayDrawer), "RenderNeedsPowerOverlay")]
    static class Patch_OverlayDrawer_RenderNeedsPowerOverlay
    {
        static bool Prefix(Thing t)
        {
            return (CheckZoomFirst(CameraZoomRange.Close)) ? CheckMouseOver(t, hidePowerWarnings) : false;
        }
    }

    //Power overlay: building disconnected
    [HarmonyPatch (typeof(OverlayDrawer), "RenderPowerOffOverlay")]
    static class Patch_OverlayDrawer_RenderPowerOffOverlay
    {
        static bool Prefix(Thing t)
        {
            return (CheckZoomFirst(CameraZoomRange.Close)) ? CheckMouseOver(t, hidePowerWarnings) : false;
        }
    }

    //Fuel overlay: Building lacks fuel
    [HarmonyPatch (typeof(OverlayDrawer), "RenderOutOfFuelOverlay")]
    static class Patch_OverlayDrawer_RenderOutOfFuelOverlay
    {
        static bool Prefix(Thing t)
        {
            return (CheckZoomFirst(CameraZoomRange.Close)) ? CheckMouseOver(t, hideFuelWarnings) : false;
        }
    }

    //Fuel overlay: do not refuel
    [HarmonyPatch (typeof(OverlayDrawer), "RenderForbiddenRefuelOverlay")]
    static class Patch_OverlayDrawer_RenderForbiddenRefuelOverlay
    {
        static bool Prefix(Thing t)
        {
            return (CheckZoomFirst(CameraZoomRange.Close)) ? CheckMouseOver(t, hideFuelWarnings) : false;
        }
    }

    //Forbidden overlay (the red X)
    [HarmonyPatch (typeof(OverlayDrawer), "RenderForbiddenBigOverlay")]
    static class Patch_OverlayDrawer_RenderForbiddenBigOverlay
    {
        static bool Prefix(Thing t)
        {
            return (CheckZoomFirst(CameraZoomRange.Close)) ? CheckMouseOver(t, hideForbiddenBuildings) : false;
        }
    }

     //Forbidden overlay (the red X)
    [HarmonyPatch (typeof(OverlayDrawer), "RenderForbiddenOverlay")]
    static class Patch_OverlayDrawer_RenderForbiddenOverlay
    {
        static bool Prefix(Thing t)
        {
            if (!hideForbidden) return true;
            if (!CheckZoomFirst(CameraZoomRange.Close)) return false;
            
            if (t.def.category == ThingCategory.Item) return CheckMouseOver(t, hideForbidden, true);
            if (t.def.category == ThingCategory.Ethereal || t.def.category == ThingCategory.Building) return CheckMouseOver(t, hideForbiddenBuildings);
            return true;
        }
    }
}