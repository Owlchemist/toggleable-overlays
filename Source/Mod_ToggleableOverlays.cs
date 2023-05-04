using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using static ToggleableOverlays.ModSettings_ToggleableOverlays;
 
namespace ToggleableOverlays
{
	[StaticConstructorOnStartup]
	public static class HarmonyPatches
	{
        static HarmonyPatches()
        {
			LoadedModManager.GetMod<Mod_ToggleableOverlays>().WriteSettings();
        }
    }

    public class Mod_ToggleableOverlays : Mod
	{
		static bool ranOnce = false;
		public Mod_ToggleableOverlays(ModContentPack content) : base(content)
		{
			base.GetSettings<ModSettings_ToggleableOverlays>();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			//========Setup tabs=========
			GUI.BeginGroup(inRect);
			var tabs = new List<TabRecord>();
			tabs.Add(new TabRecord("ToggleableOverlays.Header.TextOverlays".Translate(), delegate { selectedTab = Tab.labels; }, selectedTab == Tab.labels));
			tabs.Add(new TabRecord("ToggleableOverlays.Header.IconOverlays".Translate(), delegate { selectedTab = Tab.icons; }, selectedTab == Tab.icons));
			tabs.Add(new TabRecord("ToggleableOverlays.Header.Misc".Translate(), delegate { selectedTab = Tab.misc; }, selectedTab == Tab.misc));

			Rect rect = new Rect(0f, 32f, inRect.width, inRect.height - 32f);
			Widgets.DrawMenuSection(rect);
			TabDrawer.DrawTabs(new Rect(0f, 32f, inRect.width, Text.LineHeight), tabs);

			Listing_Standard options = new Listing_Standard();
			options.Begin(new Rect(inRect.x + 15, inRect.y + 15, inRect.width - 30, inRect.height - 30));

			if (selectedTab == Tab.labels) DrawLabels(options);
			else if (selectedTab == Tab.icons) DrawIcons(options);
			else DrawMisc(options);

			options.End();
			GUI.EndGroup();

			void DrawLabels(Listing_Standard options)
			{
				options.CheckboxLabeled("ToggleableOverlays.Settings.HideItem".Translate(), ref hideItems, "ToggleableOverlays.Settings.HideItem.Desc".Translate());
				options.CheckboxLabeled("ToggleableOverlays.Settings.HideStorage".Translate(), ref hideStorageBuilding, "ToggleableOverlays.Settings.HideStorage.Desc".Translate());
				options.CheckboxLabeled("ToggleableOverlays.Settings.HideBed".Translate(), ref hideBedAssignment, "ToggleableOverlays.Settings.HideBed.Desc".Translate());
				if (ModLister.RoyaltyInstalled) options.CheckboxLabeled("ToggleableOverlays.Settings.HideThrone".Translate(), ref hideThroneAssignment, "ToggleableOverlays.Settings.HideThrone.Desc".Translate());
				
				options.GapLine();
				
				options.CheckboxLabeled("ToggleableOverlays.Settings.HidePlayerPawn".Translate(), ref hidePlayerPawns, "ToggleableOverlays.Settings.HidePlayerPawn.Desc".Translate());
				if (hidePlayerPawns) options.CheckboxLabeled("ToggleableOverlays.Settings.HideDraftedPawn".Translate(), ref hideDraftedPawns, "ToggleableOverlays.Settings.HideDraftedPawn.Desc".Translate());
				options.CheckboxLabeled("ToggleableOverlays.Settings.HidePrisonerPawn".Translate(), ref hidePrisonerPawns, "ToggleableOverlays.Settings.HidePrisonerPawn.Desc".Translate());
				options.CheckboxLabeled("ToggleableOverlays.Settings.HideSlavePawn".Translate(), ref hideSlavePawns, "ToggleableOverlays.Settings.HideSlavePawn.Desc".Translate());
				options.CheckboxLabeled("ToggleableOverlays.Settings.HideHostilePawn".Translate(), ref hideHostilePawns, "ToggleableOverlays.Settings.HideHostilePawn.Desc".Translate());
				options.CheckboxLabeled("ToggleableOverlays.Settings.HideOtherPawn".Translate(), ref hideFriendlyPawns, "ToggleableOverlays.Settings.HideOtherPawn.Desc".Translate());
				options.CheckboxLabeled("ToggleableOverlays.Settings.HideMoteProgress".Translate(), ref hideMoteProgress, "ToggleableOverlays.Settings.HideMoteProgress.Desc".Translate());
			}
			void DrawIcons(Listing_Standard options)
			{
				options.CheckboxLabeled("ToggleableOverlays.Settings.HidePower".Translate(), ref hidePowerWarnings, "ToggleableOverlays.Settings.HidePower.Desc".Translate());
				options.CheckboxLabeled("ToggleableOverlays.Settings.HideFuel".Translate(), ref hideFuelWarnings, "ToggleableOverlays.Settings.HideFuel.Desc".Translate());
				options.CheckboxLabeled("ToggleableOverlays.Settings.HideFuelGauge".Translate(), ref hideFuelGauge, "ToggleableOverlays.Settings.HideFuelGauge.Desc".Translate());
				options.CheckboxLabeled("ToggleableOverlays.Settings.HideWindGauge".Translate(), ref hideWindGauge, "ToggleableOverlays.Settings.HideWindGauge.Desc".Translate());
				options.CheckboxLabeled("ToggleableOverlays.Settings.HideBrokenDown".Translate(), ref hideBrokenDown, "ToggleableOverlays.Settings.HideBrokenDown.Desc".Translate());
				options.CheckboxLabeled("ToggleableOverlays.Settings.HideForbidden".Translate(), ref hideForbidden, "ToggleableOverlays.Settings.HideForbidden.Desc".Translate());
				options.CheckboxLabeled("ToggleableOverlays.Settings.HideForbiddenBuildings".Translate(), ref hideForbiddenBuildings, "ToggleableOverlays.Settings.HideForbiddenBuildings.Desc".Translate());
			}
			void DrawMisc(Listing_Standard options)
			{
				options.Label("ToggleableOverlays.Settings.BlueprintTransparency".Translate("1", "0", "1", Math.Round(blueprintTransparency, 2)), -1f, "ToggleableOverlays.Settings.BlueprintTransparency.Desc".Translate());
				blueprintTransparency = options.Slider(blueprintTransparency, 0f, 1f);
				
				options.CheckboxLabeled("ToggleableOverlays.Settings.QuickShowEnabled".Translate(), ref quickShowEnabled, "ToggleableOverlays.Settings.QuickShowEnabled.Desc".Translate());
				if (quickShowEnabled) options.CheckboxLabeled("ToggleableOverlays.Settings.QuickShowAltMode".Translate(), ref quickShowAltMode, "ToggleableOverlays.Settings.QuickShowAltMode.Desc".Translate());
				options.CheckboxLabeled("ToggleableOverlays.Settings.UseOptimizedLister".Translate(), ref optimizedLister, "ToggleableOverlays.Settings.UseOptimizedLister.Desc".Translate());
				if (optimizedLister) options.CheckboxLabeled("ToggleableOverlays.Settings.UseZoomFilter".Translate(), ref zoomFilter, "ToggleableOverlays.Settings.UseZoomFilter.Desc".Translate());
				options.CheckboxLabeled("ToggleableOverlays.Settings.HideAllGauges".Translate(), ref hideAllGauges, "ToggleableOverlays.Settings.HideAllGauges.Desc".Translate());
			}
		}

		public override string SettingsCategory()
		{
			return "Toggleable Overlays";
		}

		public override void WriteSettings()
		{
			try
			{
				Harmony harmony = new Harmony(this.Content.PackageIdPlayerFacing);
				if (ranOnce) harmony.UnpatchAll(this.Content.PackageIdPlayerFacing);

				//Pawn rule sanity
				if (!hidePlayerPawns) hideDraftedPawns = false;

				//Set skip-all bool if relevant
				ToggleableOverlaysUtility.drawAllPawns = !hidePlayerPawns && !hidePrisonerPawns && !hideFriendlyPawns && !hideHostilePawns;

				//Apply blueprint transparency
				var list = DefDatabase<ThingDef>.AllDefsListForReading;
				for (int i = list.Count; i-- > 0;)
				{
					var def = list[i];
					if (def.IsBlueprint && !def.IsFrame && def.graphic != null) def.graphic.color.a = blueprintTransparency;
				}

				harmony.PatchAll();
				ranOnce = true;	
			}
			catch (System.Exception ex)
			{
				Log.Warning("[Toggleable Overlays] Failed to write settings: " + ex);
			}
			
			base.WriteSettings();
		}
	}

	public class ModSettings_ToggleableOverlays : ModSettings
	{
		public override void ExposeData()
		{
			Scribe_Values.Look(ref hideItems, "hideItems", true);
			Scribe_Values.Look(ref hideStorageBuilding, "hideStorageBuilding", true);
			Scribe_Values.Look(ref hideBedAssignment, "hideBedAssignment", true);
			Scribe_Values.Look(ref hideThroneAssignment, "hideThroneAssignment", true);

			Scribe_Values.Look(ref hidePlayerPawns, "hidePlayerPawns");
			Scribe_Values.Look(ref hideDraftedPawns, "hideDraftedPawns");
			Scribe_Values.Look(ref hidePrisonerPawns, "hidePrisonerPawns");
			Scribe_Values.Look(ref hideSlavePawns, "hideSlavePawns");
			Scribe_Values.Look(ref hideFriendlyPawns, "hideFriendlyPawns");
			Scribe_Values.Look(ref hideHostilePawns, "hideHostilePawns");

			Scribe_Values.Look(ref hidePowerWarnings, "hidePowerWarnings");
			Scribe_Values.Look(ref hideFuelWarnings, "hideFuelWarnings", true);
			Scribe_Values.Look(ref hideBrokenDown, "hideBrokenDown");
			Scribe_Values.Look(ref hideForbidden, "hideForbidden");
			Scribe_Values.Look(ref hideForbiddenBuildings, "hideForbiddenBuildings");

			Scribe_Values.Look(ref quickShowEnabled, "quickShowEnabled", true);
			Scribe_Values.Look(ref quickShowAltMode, "quickShowAltMode", true);
			Scribe_Values.Look(ref blueprintTransparency, "blueprintTransparency", 1f);
			Scribe_Values.Look(ref optimizedLister, "useOptimizedLister", true);
			Scribe_Values.Look(ref zoomFilter, "zoomFilter", true);
			Scribe_Values.Look(ref hideFuelGauge, "hideFuelGauge");
			Scribe_Values.Look(ref hideWindGauge, "hideWindGauge");
			Scribe_Values.Look(ref hideMoteProgress, "hideMoteProgress");
			Scribe_Values.Look(ref hideAllGauges, "hideAllGauges");

			base.ExposeData();
		}

		public static bool hidePlayerPawns,
			hideDraftedPawns,
			hidePrisonerPawns,
			hideSlavePawns,
			hideFriendlyPawns,
			hideHostilePawns,
			hidePowerWarnings, 
			hideForbidden,
			hideForbiddenBuildings, 
			hideBrokenDown, 
			quickShowAltMode, 
			hideFuelGauge, 
			hideWindGauge, 
			hideMoteProgress, 
			hideAllGauges, 
			optimizedLister = true, 
			zoomFilter = true, 
			hideItems = true, 
			hideBedAssignment = true, 
			hideThroneAssignment = true, 
			hideStorageBuilding = true, 
			hideFuelWarnings = true, 
			quickShowEnabled = true;
		public static float blueprintTransparency = 1f;
		public static Vector2 scrollPos;

		public static Tab selectedTab = Tab.labels;
		public enum Tab { labels, icons, misc };
	}
}
