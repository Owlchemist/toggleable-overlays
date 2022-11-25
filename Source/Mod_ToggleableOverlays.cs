using Verse;
using HarmonyLib;
using UnityEngine;
using static ToggleableOverlays.ModSettings_ToggleableOverlays;
using System;
using System.Linq;
 
namespace ToggleableOverlays
{
    public class Mod_ToggleableOverlays : Mod
	{
		static Harmony harmony;
		static bool ranOnce = false;
		public Mod_ToggleableOverlays(ModContentPack content) : base(content)
		{
			harmony = new Harmony(this.Content.PackageIdPlayerFacing);
			base.GetSettings<ModSettings_ToggleableOverlays>();
			LongEventHandler.QueueLongEvent(() => this.WriteSettings(), null, false, null);
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			inRect.yMin += 20f;
			inRect.yMax -= 20f;
			Listing_Standard options = new Listing_Standard();
			Rect outRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
			Rect rect = new Rect(0f, 0f, inRect.width - 30f, inRect.height * 1.4f);
			Widgets.BeginScrollView(outRect, ref scrollPos, rect, true);

			options.Begin(rect);
			options.Gap();
			options.Label("ToggleableOverlays.Header.TextOverlays".Translate());
			options.GapLine(); //======================================
			options.CheckboxLabeled("ToggleableOverlays.Settings.HideItem".Translate(), ref hideItems, "ToggleableOverlays.Settings.HideItem.Desc".Translate());
			options.CheckboxLabeled("ToggleableOverlays.Settings.HideStorage".Translate(), ref hideStorageBuilding, "ToggleableOverlays.Settings.HideStorage.Desc".Translate());
			options.CheckboxLabeled("ToggleableOverlays.Settings.HideBed".Translate(), ref hideBedAssignment, "ToggleableOverlays.Settings.HideBed.Desc".Translate());
			if (ModLister.RoyaltyInstalled) options.CheckboxLabeled("ToggleableOverlays.Settings.HideThrone".Translate(), ref hideThroneAssignment, "ToggleableOverlays.Settings.HideThrone.Desc".Translate());
			
			options.Gap();
			options.Label("ToggleableOverlays.Header.IconOverlays".Translate());
			options.GapLine(); //======================================
			options.CheckboxLabeled("ToggleableOverlays.Settings.HidePower".Translate(), ref hidePowerWarnings, "ToggleableOverlays.Settings.HidePower.Desc".Translate());
			options.CheckboxLabeled("ToggleableOverlays.Settings.HideFuel".Translate(), ref hideFuelWarnings, "ToggleableOverlays.Settings.HideFuel.Desc".Translate());
			options.CheckboxLabeled("ToggleableOverlays.Settings.HideFuelGauge".Translate(), ref hideFuelGauge, "ToggleableOverlays.Settings.HideFuelGauge.Desc".Translate());
			options.CheckboxLabeled("ToggleableOverlays.Settings.HideWindGauge".Translate(), ref hideWindGauge, "ToggleableOverlays.Settings.HideWindGauge.Desc".Translate());
			options.CheckboxLabeled("ToggleableOverlays.Settings.HideBrokenDown".Translate(), ref hideBrokenDown, "ToggleableOverlays.Settings.HideBrokenDown.Desc".Translate());
			options.CheckboxLabeled("ToggleableOverlays.Settings.HideForbidden".Translate(), ref hideForbidden, "ToggleableOverlays.Settings.HideForbidden.Desc".Translate());
			options.CheckboxLabeled("ToggleableOverlays.Settings.HideForbiddenBuildings".Translate(), ref hideForbiddenBuildings, "ToggleableOverlays.Settings.HideForbiddenBuildings.Desc".Translate());
			
			options.Gap();
			options.Label("ToggleableOverlays.Header.PawnOverlays".Translate());
			options.GapLine(); //======================================
			options.CheckboxLabeled("ToggleableOverlays.Settings.HidePlayerPawn".Translate(), ref hidePlayerPawns, "ToggleableOverlays.Settings.HidePlayerPawn.Desc".Translate());
			if (hidePlayerPawns) options.CheckboxLabeled("ToggleableOverlays.Settings.HideDraftedPawn".Translate(), ref hideDraftedPawns, "ToggleableOverlays.Settings.HideDraftedPawn.Desc".Translate());
			options.CheckboxLabeled("ToggleableOverlays.Settings.HidePrisonerPawn".Translate(), ref hidePrisonerPawns, "ToggleableOverlays.Settings.HidePrisonerPawn.Desc".Translate());
			options.CheckboxLabeled("ToggleableOverlays.Settings.HideHostilePawn".Translate(), ref hideHostilePawns, "ToggleableOverlays.Settings.HideHostilePawn.Desc".Translate());
			options.CheckboxLabeled("ToggleableOverlays.Settings.HideOtherPawn".Translate(), ref hideFriendlyPawns, "ToggleableOverlays.Settings.HideOtherPawn.Desc".Translate());
			options.CheckboxLabeled("ToggleableOverlays.Settings.HideMoteProgress".Translate(), ref hideMoteProgress, "ToggleableOverlays.Settings.HideMoteProgress.Desc".Translate());
			
			options.Gap();
			options.Label("ToggleableOverlays.Header.MiscOverlays".Translate());
			options.GapLine(); //======================================
			options.Label("ToggleableOverlays.Settings.BlueprintTransparency".Translate("1", "0", "1") + Math.Round(blueprintTransparency, 2) + "ToggleableOverlays.Settings.BlueprintTransparencyReload".Translate(), -1f, "ToggleableOverlays.Settings.BlueprintTransparency.Desc".Translate());
			blueprintTransparency = options.Slider(blueprintTransparency, 0f, 1f);
			
			options.Gap();
			options.CheckboxLabeled("ToggleableOverlays.Settings.QuickShowEnabled".Translate(), ref quickShowEnabled, "ToggleableOverlays.Settings.QuickShowEnabled.Desc".Translate());
			if (quickShowEnabled) options.CheckboxLabeled("ToggleableOverlays.Settings.QuickShowAltMode".Translate(), ref quickShowAltMode, "ToggleableOverlays.Settings.QuickShowAltMode.Desc".Translate());
			options.CheckboxLabeled("ToggleableOverlays.Settings.UseOptimizedLister".Translate(), ref optimizedLister, "ToggleableOverlays.Settings.UseOptimizedLister.Desc".Translate());
			if (optimizedLister) options.CheckboxLabeled("ToggleableOverlays.Settings.UseZoomFilter".Translate(), ref zoomFilter, "ToggleableOverlays.Settings.UseZoomFilter.Desc".Translate());
			options.CheckboxLabeled("ToggleableOverlays.Settings.HideAllGauges".Translate(), ref hideAllGauges, "ToggleableOverlays.Settings.HideAllGauges.Desc".Translate());
			options.End();
			Widgets.EndScrollView();
			base.DoSettingsWindowContents(inRect);
		}

		public override string SettingsCategory()
		{
			return "Toggleable Overlays";
		}

		public override void WriteSettings()
		{
			if (ranOnce) harmony.UnpatchAll(this.Content.PackageIdPlayerFacing);

			//Pawn rule sanity
			if (!hidePlayerPawns) hideDraftedPawns = false;

			//Set skip-all bool if relevant
			ToggleableOverlaysUtility.drawAllPawns = !hidePlayerPawns && !hidePrisonerPawns && !hideFriendlyPawns && !hideHostilePawns;

			//Apply blueprint transparency
			DefDatabase<ThingDef>.AllDefsListForReading.ForEach(x => {if (x.IsBlueprint && !x.IsFrame) x.graphic.color.a = blueprintTransparency;});

			harmony.PatchAll();
			ranOnce = true;
			base.WriteSettings();
		}
	}

	public class ModSettings_ToggleableOverlays : ModSettings
	{
		public override void ExposeData()
		{
			Scribe_Values.Look<bool>(ref hideItems, "hideItems", true);
			Scribe_Values.Look<bool>(ref hideStorageBuilding, "hideStorageBuilding", true);
			Scribe_Values.Look<bool>(ref hideBedAssignment, "hideBedAssignment", true);
			Scribe_Values.Look<bool>(ref hideThroneAssignment, "hideThroneAssignment", true);

			Scribe_Values.Look<bool>(ref hidePlayerPawns, "hidePlayerPawns");
			Scribe_Values.Look<bool>(ref hideDraftedPawns, "hideDraftedPawns");
			Scribe_Values.Look<bool>(ref hidePrisonerPawns, "hidePrisonerPawns");
			Scribe_Values.Look<bool>(ref hideFriendlyPawns, "hideFriendlyPawns");
			Scribe_Values.Look<bool>(ref hideHostilePawns, "hideHostilePawns");

			Scribe_Values.Look<bool>(ref hidePowerWarnings, "hidePowerWarnings");
			Scribe_Values.Look<bool>(ref hideFuelWarnings, "hideFuelWarnings", true);
			Scribe_Values.Look<bool>(ref hideBrokenDown, "hideBrokenDown");
			Scribe_Values.Look<bool>(ref hideForbidden, "hideForbidden");
			Scribe_Values.Look<bool>(ref hideForbiddenBuildings, "hideForbiddenBuildings");

			Scribe_Values.Look<bool>(ref quickShowEnabled, "quickShowEnabled", true);
			Scribe_Values.Look<bool>(ref quickShowAltMode, "quickShowAltMode", true);
			Scribe_Values.Look<float>(ref blueprintTransparency, "blueprintTransparency", 1f);
			Scribe_Values.Look<bool>(ref optimizedLister, "useOptimizedLister");
			Scribe_Values.Look<bool>(ref zoomFilter, "zoomFilter");
			Scribe_Values.Look<bool>(ref hideFuelGauge, "hideFuelGauge");
			Scribe_Values.Look<bool>(ref hideWindGauge, "hideWindGauge");
			Scribe_Values.Look<bool>(ref hideMoteProgress, "hideMoteProgress");
			Scribe_Values.Look<bool>(ref hideAllGauges, "hideAllGauges");

			base.ExposeData();
		}

		public static bool hidePlayerPawns, hideDraftedPawns, hidePrisonerPawns, hideFriendlyPawns, hideHostilePawns, hidePowerWarnings, hideForbidden,
		hideForbiddenBuildings, hideBrokenDown, quickShowAltMode, hideFuelGauge, hideWindGauge, hideMoteProgress, hideAllGauges, optimizedLister, zoomFilter, 
		hideItems = true, hideBedAssignment = true, hideThroneAssignment = true, hideStorageBuilding = true, hideFuelWarnings = true, quickShowEnabled = true;
		public static float blueprintTransparency = 1f;
		public static Vector2 scrollPos = Vector2.zero;
	}
}
