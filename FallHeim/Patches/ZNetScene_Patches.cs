using HarmonyLib;

namespace FallHeim.Patches
{
	[HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
	public static class ZNetScene_Awake_AddWearNTearFireController
	{
		private static void Postfix(ZNetScene __instance)
		{
			__instance.transform.parent.gameObject.AddComponent<WorldFall>();
		}
	}
}
