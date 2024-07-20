using HarmonyLib;

namespace FallHeim.Patches
{
	[HarmonyPatch(typeof(Game), nameof(Game.RequestRespawn))]
	class Game_RequestRespawn_patch
	{
		static bool Prefix()
		{
			if (WorldFall.s_isDead) return false;
			else return true;
		}
	}
}
