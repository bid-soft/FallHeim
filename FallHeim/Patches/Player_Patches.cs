using HarmonyLib;
using UnityEngine;


namespace FallHeim.Patches
{
	
	[HarmonyPatch(typeof(Player), nameof(Player.Awake))]
	class Player_Awake_patch
	{
		static void Prefix(ref Player __instance)
		{
			GameObject rigid = new GameObject();
			rigid.SetActive(false);
			rigid.name = "rigid";
			rigid.transform.parent =__instance.transform;
			rigid.transform.localPosition = Vector3.zero;
			rigid.gameObject.AddComponent<TriggerEventCaller>();
			rigid.SetActive(false);

			__instance.transform.position = FallHeim.GetStartLocation();

		}
	}


	[HarmonyPatch(typeof(Player), nameof(Player.InGodMode))]
	class Player_InGodMode_patch
	{
		static bool Prefix(ref bool __result)
		{
			// don't die to early, you must endure until the end, haha
			__result = true;
			return false;

		}
	}

	[HarmonyPatch(typeof(Player), nameof(Player.TeleportTo))]
	public static class Player_TeleportTo_patch
	{
		public static void Prefix(Player __instance)
		{
			WorldFall.t_rigidActiveTeleport = __instance.transform.Find("rigid").gameObject.activeSelf;
		}
	}

	[HarmonyPatch(typeof(Player), nameof(Player.UpdateTeleport))]
	public static class Player_UpdateTeleport_patch
	{
		private static void Postfix(ref Player __instance)
		{
			if (!__instance.IsTeleporting()) return;
			if ((__instance.m_teleportTimer > 8f || !__instance.m_distantTeleport) && ZNetScene.instance.IsAreaReady(__instance.m_teleportTargetPos))
			{
				__instance.transform.Find("rigid").gameObject.SetActive(WorldFall.t_rigidActiveTeleport);
			}
		}
	}


}
