using HarmonyLib;

namespace FallHeim.Patches
{
	
	[HarmonyPatch(typeof(GameCamera), nameof(GameCamera.UpdateCamera))]
	class GameCamera_UpdateCamera_patch
	{
		static void Postfix(ref GameCamera __instance)
		{
			__instance.transform.rotation = WorldFall.s_rotationToTargetTop * __instance.transform.rotation;
		}
	}
	
}
