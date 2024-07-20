using HarmonyLib;
using UnityEngine.SceneManagement;
using UnityEngine;


namespace FallHeim.Patches
{
	
	[HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.Start))]
	public static class FejdStartup_Awake_Patch
	{

		private static void Postfix(FejdStartup __instance)
		{
			Scene currentScene = SceneManager.GetActiveScene();
			foreach (GameObject rootGameObject in currentScene.GetRootGameObjects())
				ParseChildren(rootGameObject.transform);

			Physics.gravity = Vector3.zero;

		}

		static void ParseChildren(Transform parent)
		{

			float density = 10f;
			foreach (Transform child in parent)
			{
				ParseChildren(child);
				// some hard codings are always nice, haha
				if (child.name.StartsWith("Menu_fir") || child.name.StartsWith("Menu_Pine")
					|| child.name.StartsWith("Rock_3")
					|| child.name.StartsWith("Menu_SmallStone") || child.name.StartsWith("Pine_tree_lod0")
					|| child.name.StartsWith("Pinetree_01_lod_02") || child.name.StartsWith("Pine_tree_lod0")
					|| child.name.StartsWith("Rocks")
					|| child.name.StartsWith("stubbe") || child.name.StartsWith("bushes")
					|| child.name.StartsWith("Trees2") || child.name.StartsWith("bush")
					|| child.name.StartsWith("Oak") || child.name.StartsWith("Beech")
					|| child.name.StartsWith("menu_bush")
					|| child.name.StartsWith("runestone") || child.name.StartsWith("Rocks2"))
				{
					var mcol = child.gameObject.AddComponent<MeshCollider>();
					mcol.convex = true;
					var mren = child.gameObject.GetComponentInChildren<MeshFilter>();
					if (mren != null) mcol.sharedMesh = mren.sharedMesh;					

					Bounds bounds = FallHeim.CalculateCombinedBounds(child);
					float volume = bounds.size.x * bounds.size.y * bounds.size.z;
					float mass = volume * density;

					Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
					rb.mass = mass;
					rb.isKinematic = false;
				}

			}
		}
	}
	
}
