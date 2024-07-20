using HarmonyLib;

namespace FallHeim.Patches
{
	[HarmonyPatch(typeof(DungeonDB), nameof(DungeonDB.Start))]
	class DungeonDB_Start_patch
	{
		// not the best idea to do everything here, but who cares for the worst mod ;)
		static void Postfix()
		{
			FallHeim.Init();
		}
	}
}
