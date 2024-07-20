using FallHeim.States;
using HarmonyLib;
using UnityEngine;
using static FallHeim.WorldFall;

namespace FallHeim.Patches
{

	[HarmonyPatch(typeof(Heightmap), nameof(Heightmap.GetBiomeColor), typeof(float), typeof(float))]
	class Heightmap_GetBiomeColor_patch
	{
		static bool Prefix(float ix, float iy, ref Color __result, ref Heightmap __instance)
		{
			HeightmapKey hmkey = new HeightmapKey(__instance, ix, iy);

			if (WorldFall.s_biomeColors.ContainsKey(hmkey)) 
			{
				__result = s_biomeColors[hmkey];
				return false;
			}
			else return true;
		}

		static void Postfix(float ix, float iy, ref Color __result, ref Heightmap __instance)
		{
			__result = Color.Lerp(__result, new Color32(byte.MaxValue, 0, 0, byte.MaxValue), CrackState.s_ashed);
		}
	}
}
