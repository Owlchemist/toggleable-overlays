using HarmonyLib;
using Verse;
using System.Linq;
using static ToggleableOverlays.ModSettings_ToggleableOverlays;
using static ToggleableOverlays.ToggleableOverlaysUtility;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
 
namespace ToggleableOverlays
{
    [StaticConstructorOnStartup]
	[HarmonyPriority(Priority.Last)]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
			string ID = "owlchemist.toggleableoverlays.dynamic";
            var harmony = new Harmony(ID);

            //Check if a stackable chunks mod is loaded
			if (DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.thingCategories?.Contains(ResourceBank.ThingCategoryDefOf.Chunks) ?? false).Any(y => y.drawGUIOverlayQuality == true))
            {
                harmony.Patch(AccessTools.Method(typeof(Thing), nameof(Thing.DrawGUIOverlay)),
                    prefix: new(typeof(HarmonyPatches), nameof(Patch_Thing_DrawGUIOverlay)));
            }

            if (optimizedLister) harmony.Patch(AccessTools.Method(typeof(ThingOverlays), nameof(ThingOverlays.ThingOverlaysOnGUI)),
                prefix: new(typeof(HarmonyPatches), nameof(Replace_ThingOverlaysOnGUI)));

			//MethodBase methodInfo = typeof(CellRect).GetMethod(nameof(CellRect.Contains), BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance );
			//harmony.Patch(methodInfo, transpiler: new(typeof(HarmonyPatches), nameof(Replace_RectContains)));

			//if (useOptimizedLister) harmony.Unpatch(AccessTools.Method(typeof(ThingOverlays), nameof(ThingOverlays.ThingOverlaysOnGUI)), HarmonyPatchType.Prefix, ID);
        }
        //Item stackable chunks, if relevant
        static bool Patch_Thing_DrawGUIOverlay(Thing __instance)
        {
            if (__instance.def.category == ThingCategory.Item) return CheckZoomFirst(CameraZoomRange.Closest) ? CheckMouseOver(__instance, hideItems) : false;
            return true;
        }

		//This is a destructive prefix. No replacement in case someone is postfixing it
        static bool Replace_ThingOverlaysOnGUI()
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

		/*
		//Only difference is it checks the z axis first. Due to widescreen monitors, this axis is most likely to short circuit first
		static bool FasterContains<T>(this CellRect rect, IntVec3 c)
		{
			return c.z >= rect.minZ && c.z <= rect.maxZ && c.x >= rect.minX && c.x <= rect.maxX;
		}

		static IEnumerable<CodeInstruction> Replace_RectContains(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var code in instructions)
			{
				if (code.opcode == OpCodes.Ldfld && code.OperandIs(AccessTools.Field(typeof(CellRect), nameof(CellRect.minX)) ))
				{
					yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CellRect), nameof(CellRect.minZ)));
				}
				else if (code.opcode == OpCodes.Ldfld && code.OperandIs(AccessTools.Field(typeof(CellRect), nameof(CellRect.maxX)) ))
				{
					yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CellRect), nameof(CellRect.maxZ)));
				}
				else if (code.opcode == OpCodes.Ldfld && code.OperandIs(AccessTools.Field(typeof(CellRect), nameof(CellRect.minZ)) ))
				{
					yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CellRect), nameof(CellRect.minX)));
				}
				else if (code.opcode == OpCodes.Ldfld && code.OperandIs(AccessTools.Field(typeof(CellRect), nameof(CellRect.maxZ)) ))
				{
					yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CellRect), nameof(CellRect.maxX)));
				}
				else yield return code;
			}
        }
		*/
    }
}