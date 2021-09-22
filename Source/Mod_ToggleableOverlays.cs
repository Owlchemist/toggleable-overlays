using Verse;
using HarmonyLib;
using UnityEngine;
using static ToggleableOverlays.ModSettings_ToggleableOverlays;
 
namespace ToggleableOverlays
{
    public class Mod_HiddenOverlays : Mod
	{
		public Mod_HiddenOverlays(ModContentPack content) : base(content)
		{
			new Harmony(this.Content.PackageIdPlayerFacing).PatchAll();
			base.GetSettings<ModSettings_ToggleableOverlays>();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			Listing_Standard options = new Listing_Standard();
			options.Begin(inRect);
			options.Gap();
			options.Label("Hide text overlays:");
			options.GapLine();
			options.CheckboxLabeled("Items", ref hideItems, "Owl_HideItem".Translate());
			options.CheckboxLabeled("Storage buildings", ref hideStorageBuilding, "Owl_HideStorage".Translate());
			options.CheckboxLabeled("Bed assignment", ref hideBedAssignment, "Owl_HideBed".Translate());
			options.CheckboxLabeled("Throne assignment (Royalty)", ref hideThroneAssignment, "Owl_HideThrone".Translate());
			options.Gap();
			options.Label("Hide icon overlays:");
			options.GapLine();
			options.CheckboxLabeled("Power", ref hidePowerWarnings, "Owl_HidePower".Translate());
			options.CheckboxLabeled("Fuel", ref hideFuelWarnings, "Owl_HideFuel".Translate());
			options.CheckboxLabeled("Forbidden items", ref hideForbidden, "Owl_HideForbidden".Translate());
			options.CheckboxLabeled("Forbidden buildings and blueprints", ref hideForbiddenBuildings, "Owl_HideForbiddenBuildings".Translate());
			options.Gap();
			options.Label("Hide pawn names:");
			options.GapLine();
			options.CheckboxLabeled("Player pawns", ref hidePlayerPawns, "Owl_HidePlayerPawn".Translate());
			if (hidePlayerPawns) options.CheckboxLabeled("Drafted pawns", ref hideDraftedPawns, "Owl_HideDraftedPawn".Translate());
			options.CheckboxLabeled("Prisoners and slaves", ref hidePrisonerPawns, "Owl_HidePrisonerPawn".Translate());
			options.CheckboxLabeled("Hostiles", ref hideHostilePawns, "Owl_HideHostilePawn".Translate());
			options.CheckboxLabeled("Friendlies and neutrals", ref hideFriendlyPawns, "Owl_HideOtherPawn".Translate());
			options.Gap();
			options.GapLine();
			options.CheckboxLabeled("Enable quick show (ctrl key)", ref quickShowEnabled, "Owl_QuickShowEnabled".Translate());
			options.End();
			base.DoSettingsWindowContents(inRect);
		}

		public override string SettingsCategory()
		{
			return "Toggleable Overlays";
		}

		public override void WriteSettings()
		{
			if (!hidePlayerPawns) hideDraftedPawns = false;
			GameComponent_ToggleableOverlays.drawAllPawns = !hidePlayerPawns && !hidePrisonerPawns && !hideFriendlyPawns && !hideHostilePawns;
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
			Scribe_Values.Look<bool>(ref hideForbidden, "hideForbidden", false, false);
			Scribe_Values.Look<bool>(ref hideForbiddenBuildings, "hideForbiddenBuildings", false, false);

			Scribe_Values.Look<bool>(ref quickShowEnabled, "quickShowEnabled", true, false);
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
		public static bool quickShowEnabled = true;
	}
}
