
using Verse;
using RimWorld;
 
namespace ToggleableOverlays
{
    internal static class ToggleableOverlaysUtility
	{
		public static IntVec3 mousePosition = new IntVec3(0,0,0); //Instead of every entity calling this, we centralize the call just once per frame
		public static CameraZoomRange currentCameraZoom = CameraZoomRange.Close; //Same thing here...
		public static bool quickView; //Ctrl key is pressed?
		public static bool drawAllPawns; //Performance bool, skip method is irrelevant

		public static bool CheckMouseOver(Thing thing, bool flag, bool sizeOne = false)
		{
			if (!flag || quickView) return true;
			
			if (sizeOne || thing.def.size.x == 1 && thing.def.size.z == 1) return thing.positionInt.x == mousePosition.x && thing.positionInt.z == mousePosition.z;
			
			return GenAdj.IsInside(mousePosition, thing.positionInt, thing.rotationInt, thing.def.size);
		}

		public static bool CheckZoomFirst(CameraZoomRange minimumCameraRange)
		{
			return currentCameraZoom <= minimumCameraRange;
		}
	}

	[DefOf]
	public static class Hotkeys
	{
		public static KeyBindingDef QuickShowKey;
	}
}