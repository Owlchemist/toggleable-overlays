using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;
using static ToggleableOverlays.ModSettings_ToggleableOverlays;
using static ToggleableOverlays.ToggleableOverlaysUtility;
 
namespace ToggleableOverlays
{
    //Optimized lister
    [HarmonyPatch (typeof(ThingOverlays), nameof(ThingOverlays.ThingOverlaysOnGUI))]
    static class Patch_ThingOverlays_ThingOverlaysOnGUI
    {
        static bool Prepare()
        {
            return optimizedLister;
        }
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var workingList = new List<CodeInstruction>();
            bool found1 = false, found2 = false;
            int counter = 0;
            var method1 = AccessTools.PropertyGetter(typeof(Find), nameof(Find.CurrentMap));
            var method2 = AccessTools.PropertyGetter(typeof(List<Thing>), nameof(List<Thing>.Count));
            
            LocalBuilder map = generator.DeclareLocal(typeof(Map));
            LocalBuilder length = generator.DeclareLocal(typeof(int));

            var code = new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Find), nameof(Find.CurrentMap)));
            var label = generator.DefineLabel();
            code.labels.Add(label);
            
            foreach (var instruction in instructions)
            {
                //Add a local variable for CurrentMap
                if (!found2 && instruction.opcode == OpCodes.Beq_S)
                {
                    workingList.Add(new CodeInstruction(OpCodes.Beq_S, label));
                    found2 = true;
                }
                else if (!found1 && instruction.opcode == OpCodes.Ret)
                {
                    workingList.Add(instruction);
                    workingList.Add(code);
                    workingList.Add(new CodeInstruction(OpCodes.Stloc_S, map.LocalIndex));
                    found1 = true;
                }
                //Skip the 31 request group for the lister as we don't use it
                else if (instruction.opcode == OpCodes.Ldc_I4_S) continue;
                //Replace CurrentMap with our local variable
                else if (instruction.Calls(method1))
                {
                    workingList.Add(new CodeInstruction(OpCodes.Ldloc_S, map.LocalIndex));
                }
                //Cache the count
                else if (instruction.opcode == OpCodes.Stloc_1)
                {
                    workingList.Add(instruction);
                    workingList.Add(new CodeInstruction(OpCodes.Ldloc_1));
                    workingList.Add(new CodeInstruction(OpCodes.Callvirt, method2));
                    workingList.Add(new CodeInstruction(OpCodes.Stloc_S, length.LocalIndex));
                }
                //Skip the Count getter pt I
                else if (instruction.opcode == OpCodes.Ldloc_1)
                {
                    if (counter == 0) workingList.Add(instruction);
                    counter++;
                }
                else if (instruction.Calls(method2))
                {
                    workingList.Add(new CodeInstruction(OpCodes.Ldloc_S, length.LocalIndex));
                }
                else workingList.Add(instruction);
            }

            return workingList
                .MethodReplacer(AccessTools.PropertyGetter(typeof(CameraDriver), nameof(CameraDriver.CurrentViewRect)),
				AccessTools.Method(typeof(Patch_ThingOverlays_ThingOverlaysOnGUI), nameof(CurrentViewRect)))
                
                .MethodReplacer(AccessTools.Method(typeof(ListerThings), nameof(ListerThings.ThingsInGroup)),
				AccessTools.Method(typeof(Patch_ThingOverlays_ThingOverlaysOnGUI), nameof(AlteredListByGroup)))
                
                .MethodReplacer(AccessTools.Method(typeof(FogGrid), nameof(FogGrid.IsFogged), new Type[] { typeof(IntVec3) }),
				AccessTools.Method(typeof(Patch_ThingOverlays_ThingOverlaysOnGUI), nameof(IsFoggedFast)));
        }

        public static CellRect CurrentViewRect(CameraDriver cameraDriver)
        {
            return CameraDriver.lastViewRect;
        }

        public static bool IsFoggedFast(FogGrid fogGrid, IntVec3 position)
        {
            return fogGrid.fogGrid[position.z * fogGrid.map.info.sizeInt.x + position.x];
        }

        public static List<Thing> AlteredListByGroup(ListerThings listerThings)
        {
            return listerThings.listsByGroup[!zoomFilter || currentCameraZoom == CameraZoomRange.Closest ? 31 : 12] ?? new List<Thing>(); //ThingRequestGroup.HasGUIOverlay is 31, Pawn is 12
        }
    }
}