using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace FallHeim
{

	[HarmonyPatch(typeof(ZoneSystem), nameof(ZoneSystem.SpawnLocation))]
	static class ZoneSystem_SpawnLocation_Patch
	{

		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> OnCreateLocationTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			var matcher = new CodeMatcher(instructions)
				.MatchForward(
					useEnd: true,
					new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(SoftReferenceableAssets.SoftReference<GameObject>), "Load"))
					)
				.ThrowIfInvalid("Could not patch ZoneSystem.SpawnLocation: SoftReference.Load not found! ")
				.Advance(2)
				.InsertAndAdvance(
					new CodeInstruction(OpCodes.Ldarg_1),
					Transpilers.EmitDelegate<Action<ZoneSystem.ZoneLocation>>(OnInstantiateDelegate))
				.InstructionEnumeration();

			return matcher;
		}

		static void OnInstantiateDelegate(ZoneSystem.ZoneLocation location)
		{
			FallHeim.AddRigidbody2Location(location);
		}
	}

}
