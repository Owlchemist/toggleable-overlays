using HarmonyLib;
using Verse;
using static ToggleableOverlays.ModSettings_ToggleableOverlays;
using static ToggleableOverlays.ToggleableOverlaysUtility;
using System;
using System.Collections.Generic;
using UnityEngine;
 
namespace ToggleableOverlays
{
    //Optimized lister
    [HarmonyPatch (typeof(ThingOverlays), nameof(ThingOverlays.ThingOverlaysOnGUI))]
    static class Replace_ThingOverlaysOnGUI
    {
        static bool Prepare()
        {
            return optimizedLister;
        }
        static bool Prefix(Thing __instance)
        {
            if (Event.current.type != EventType.Repaint) return false;

            Map map = Find.CurrentMap;
            List<Thing> listsByGroup = map.listerThings.listsByGroup[!zoomFilter || currentCameraZoom == CameraZoomRange.Closest ? 31 : 12]; //ThingRequestGroup.HasGUIOverlay is 31, Pawn is 12
            
            int length = listsByGroup.Count;
            for (int i = 0; i < length; ++i)
            {
                Thing thing = listsByGroup[i];
                if (CameraDriver.lastViewRect.Contains(thing.positionInt) && !map.fogGrid.fogGrid[thing.positionInt.z * map.info.sizeInt.x + thing.positionInt.x])
                {
                    try
                    {
                        thing.DrawGUIOverlay();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(string.Concat(new object[]
                        {
                            "Exception drawing ThingOverlay for ",
                            thing,
                            ": ",
                            ex
                        }));
                    }
                }
            }
            return false;
        }
    }
}