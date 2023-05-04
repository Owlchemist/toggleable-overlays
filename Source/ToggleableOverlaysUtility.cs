
using Verse;
using RimWorld;
 
namespace ToggleableOverlays
{
    internal static class ToggleableOverlaysUtility
	{
		public static bool quickView, drawAllPawns;
		public static int mousePositionX, mousePositionZ;
		public static IntVec3 mousePosition = new IntVec3(0,0,0); //Instead of every entity calling this, we centralize the call just once per frame
		static IntVec2 gaugeArea = new IntVec2(2,2);
		public static CameraZoomRange currentCameraZoom = CameraZoomRange.Close;

		public static bool CheckMouseOver(Thing thing, bool flag, bool sizeOne = false)
		{
			if (!flag || quickView) return true;
			
			if (sizeOne || thing.def.size.x == 1 && thing.def.size.z == 1) return thing.positionInt.x == mousePositionX && thing.positionInt.z == mousePositionZ;
			
			return GenAdj.IsInside(mousePosition, thing.positionInt, thing.rotationInt, thing.def.size);
		}
		public static bool CheckMouseOver(Thing thing)
		{			
			if (thing.def.size.z == 1 && thing.def.size.x == 1) return thing.positionInt.x == mousePositionX && thing.positionInt.z == mousePositionZ;
			
			return GenAdj.IsInside(mousePosition, thing.positionInt, thing.rotationInt, thing.def.size);
		}
		public static bool CheckMouseOver(GenDraw.FillableBarRequest r, bool flag)
        {
            if (!flag || quickView) return true;
            return (GenAdj.IsInside(mousePosition, r.center.ToIntVec3(), r.rotation, gaugeArea));
        }

		public static bool CheckZoomFirst(CameraZoomRange minimumCameraRange)
		{
			return currentCameraZoom <= minimumCameraRange;
		}
	}
}