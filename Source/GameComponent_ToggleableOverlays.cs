
using Verse;
using UnityEngine;
 
namespace ToggleableOverlays
{
    public class GameComponent_ToggleableOverlays : GameComponent
	{
		public static IntVec3 mousePosition = new IntVec3(0,0,0); //Instead of every entity calling this, we centralize the call just once per frame
		public static CameraZoomRange currentCameraZoom; //Same thing here...
		public static IntVec2 sizeOne = new IntVec2(1,1); //Quick cache
		public static bool quickView; //Ctrl key is pressed?
		public static bool drawAllPawns; //Performance bool, skip method is irrelevant
		public override void GameComponentUpdate()
		{
			mousePosition = UI.MouseCell();
			currentCameraZoom = Find.CameraDriver.CurrentZoom;
			if (ModSettings_ToggleableOverlays.quickShowEnabled)
			{
				quickView = Event.current.control;
			}
		}

		public GameComponent_ToggleableOverlays(Game game)
		{}

		public static bool CheckMouseOver(Thing thing, bool flag, bool sizeOne = false)
		{
			if (!flag || quickView) return true;
			//Micro-optimization: We only compare x and z because y is always a match
            if ((sizeOne || thing.def.size.Area == 1) && thing.Position.x == mousePosition.x && thing.Position.z == mousePosition.z) return true;
            else if (thing.OccupiedRect().Contains(mousePosition)) return true;
            return false;
		}

		public static bool CheckZoomFirst(CameraZoomRange minimumCameraRange)
		{
			if (currentCameraZoom <= minimumCameraRange) return true;
			return false;
		}
	}
}