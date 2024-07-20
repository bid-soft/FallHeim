using HarmonyLib;

namespace FallHeim.Patches
{
	[HarmonyPatch(typeof(ClutterSystem), nameof(ClutterSystem.UpdateGrass))]
	class ClutterSystem_UpdateGrass_patch
	{
		static bool Prefix()
		{
			if (WorldFall.s_noGrass) return false;
			else return true;
		}
	}
}
