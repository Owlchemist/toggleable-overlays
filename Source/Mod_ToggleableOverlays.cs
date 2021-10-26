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
		public Mod_ToggleableOverlays(ModContentPack content) : base(content)
		{
			new Harmony(this.Content.PackageIdPlayerFacing).PatchAll();
			base.GetSettings<ModSettings_ToggleableOverlays>();
			LongEventHandler.QueueLongEvent(() => LoadedModManager.GetMod<Mod_ToggleableOverlays>().WriteSettings(), "Mod_HiddenOverlays.WriteSettings", false, null);
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			inRect.yMin += 20f;
			inRect.yMax -= 20f;
			Listing_Standard options = new Listing_Standard();
			Rect outRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
			Rect rect = new Rect(0f, 0f, inRect.width - 30f, inRect.height * 1.2f);
			Widgets.BeginScrollView(outRect, ref scrollPos, rect, true);

			//Listing_Standard options = new Listing_Standard();
			options.Begin(rect);
			options.Gap();
			options.Label("ToggleableOverlays.Header.TextOverlays".Translate());
			options.GapLine(); //======================================
			options.CheckboxLabeled("ToggleableOverlays.Settings.HideItem".Translate(), ref hideItems, "ToggleableOverlays.Settings.HideItem.Desc".Translate());
			options.CheckboxLabeled("ToggleableOverlays.Settings.HideStorage".Translate(), ref hideStorageBuilding, "ToggleableOverlays.Settings.HideStorage.Desc".Translate());
			options.CheckboxLabeled("ToggleableOverlays.Settings.HideBed".Translate(), ref hideBedAssignment, "ToggleableOverlays.Settings.HideBed.Desc".Translate());
			options.CheckboxLabeled("ToggleableOverlays.Settings.HideThrone".Translate(), ref hideThroneAssignment, "ToggleableOverlays.Settings.HideThrone.Desc".Translate());
			
			options.Gap();
			options.Label("ToggleableOverlays.Header.IconOverlays".Translate());
			options.GapLine(); //======================================
			options.CheckboxLabeled("ToggleableOverlays.Settings.HidePower".Translate(), ref hidePowerWarnings, "ToggleableOverlays.Settings.HidePower.Desc".Translate());
			options.CheckboxLabeled("ToggleableOverlays.Settings.HideFuel".Translate(), ref hideFuelWarnings, "ToggleableOverlays.Settings.HideFuel.Desc".Translate());
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
			
			options.Gap();
			options.Label("ToggleableOverlays.Header.MiscOverlays".Translate());
			options.GapLine(); //======================================
			options.Label("ToggleableOverlays.Settings.BlueprintTransparency".Translate("1", "0", "1") + Math.Round(blueprintTransparency, 2) + "ToggleableOverlays.Settings.BlueprintTransparencyReload".Translate(), -1f, "ToggleableOverlays.Settings.BlueprintTransparency.Desc".Translate());
			blueprintTransparency = options.Slider(blueprintTransparency, 0f, 1f);
			
			options.Gap();
			options.CheckboxLabeled("ToggleableOverlays.Settings.QuickShowEnabled".Translate(), ref quickShowEnabled, "ToggleableOverlays.Settings.QuickShowEnabled.Desc".Translate());
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
			//Pawn rule sanity
			if (!hidePlayerPawns) hideDraftedPawns = false;

			//Set skip-all bool if relevant
			ToggleableOverlaysUtility.drawAllPawns = !hidePlayerPawns && !hidePrisonerPawns && !hideFriendlyPawns && !hideHostilePawns;

			//Apply blueprint transparency
			DefDatabase<ThingDef>.AllDefs.Where(x => x.IsBlueprint && !x.IsFrame).ToList().ForEach(y => y.graphic.color.a = blueprintTransparency);

			base.WriteSettings();
		}
	}

	public class ModSettings_ToggleableOverlays : ModSettings
	{
		public override void ExposeData()
		{
			Scribe_Values.Look<bool>(ref hideItems, "hideItems", true, false);
			Scribe_Values.Look<bool>(ref hideStorageBuilding, "hideStorageBuilding", true, false);
			Scribe_Values.Look<bool>(ref hideBedAssignment, "hideBedAssignment", true, false);
			Scribe_Values.Look<bool>(ref hideThroneAssignment, "hideThroneAssignment", true, false);

			Scribe_Values.Look<bool>(ref hidePlayerPawns, "hidePlayerPawns", false, false);
			Scribe_Values.Look<bool>(ref hideDraftedPawns, "hideDraftedPawns", false, false);
			Scribe_Values.Look<bool>(ref hidePrisonerPawns, "hidePrisonerPawns", false, false);
			Scribe_Values.Look<bool>(ref hideFriendlyPawns, "hideFriendlyPawns", false, false);
			Scribe_Values.Look<bool>(ref hideHostilePawns, "hideHostilePawns", false, false);

			Scribe_Values.Look<bool>(ref hidePowerWarnings, "hidePowerWarnings", false, false);
			Scribe_Values.Look<bool>(ref hideFuelWarnings, "hideFuelWarnings", true, false);
			Scribe_Values.Look<bool>(ref hideBrokenDown, "hideBrokenDown", false, false);
			Scribe_Values.Look<bool>(ref hideForbidden, "hideForbidden", false, false);
			Scribe_Values.Look<bool>(ref hideForbiddenBuildings, "hideForbiddenBuildings", false, false);

			Scribe_Values.Look<bool>(ref quickShowEnabled, "quickShowEnabled", true, false);
			Scribe_Values.Look<float>(ref blueprintTransparency, "blueprintTransparency", 1f, false);

			base.ExposeData();
		}

		public static bool hideItems = true;
		public static bool hideBedAssignment = true;
		public static bool hideThroneAssignment = true;
		public static bool hideStorageBuilding = true;
		public static bool hidePlayerPawns = false;
		public static bool hideDraftedPawns = false;
		public static bool hidePrisonerPawns = false;
		public static bool hideFriendlyPawns = false;
		public static bool hideHostilePawns = false;
		public static bool hidePowerWarnings = false;
		public static bool hideFuelWarnings = true;
		public static bool hideForbidden = false;
		public static bool hideForbiddenBuildings = false;
		public static bool hideBrokenDown = false;
		public static bool quickShowEnabled = true;
		public static float blueprintTransparency = 1f;
		public static Vector2 scrollPos = Vector2.zero;
	}
}
