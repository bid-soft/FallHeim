using HarmonyLib;

namespace FallHeim.Patches
{
	
	[HarmonyPatch(typeof(WearNTear), nameof(WearNTear.HaveSupport))]
	class WearNTear_HaveSupport_patch
	{
		public static bool Prefix(ref bool __result)
		{
			__result = true;
			return false;	
		}
	}
	
}
