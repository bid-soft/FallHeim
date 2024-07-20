using BepInEx;
using FallHeim.States;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;

namespace FallHeim
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class FallHeim : BaseUnityPlugin
    {
        public const string PluginGUID = "org.bepinex.plugins.bid.fallheim";
        public const string PluginName = "FallHeim";
        public const string PluginVersion = "0.0.1";

		private readonly Harmony harmony = new Harmony(PluginGUID);

		public static SimulateFlyingAway s_flyaway;
		public static GameObject s_psysDebris;
		public static BiomeEnvSetup s_miomeSetupMeadows = null;
		public static EnvSetup s_envClear = null;
		public static EnvSetup s_envAsh = null;
		public static EnvSetup s_envStorm = null;
		public static GameObject s_sfxTreeHit = null;
		public static GameObject s_vfxTreeHit = null;
		public static GameObject s_sfxTreeFall = null;
		public static AudioClip s_ambientFallCalm = null;
		public static AudioClip s_ambientFallIntsense = null;
		public static bool s_init=false;

		private void Awake()
        {
            Jotunn.Logger.LogInfo("FallHeim has landed");

			var harmony = new Harmony(PluginGUID);

			harmony.PatchAll();
		}

		public static void PreparePrefab(GameObject prefab)
		{
			prefab.transform.position = Vector3.zero;
			prefab.transform.rotation = Quaternion.identity;
			foreach (MeshCollider collider in prefab.GetComponentsInChildren<MeshCollider>()) collider.convex = true;

			foreach (var elem in prefab.GetComponentsInChildren<Collider>())
			{
				if (elem.gameObject.layer == 16 || elem.gameObject.layer == 12 || elem.gameObject.layer == 23 || elem.gameObject.layer == 20)
				{

					elem.gameObject.layer = 0;
				}
			}


			var ie = prefab.AddComponent<ImpactEffect>();
			ie.m_minVelocity = 1;
			ie.m_maxVelocity = 5;
			ie.m_damagePlayers = true;
			ie.m_damageToSelf = false;
			ie.m_damageFish = true;
			ie.m_damages.m_blunt = 25;
			ie.m_damages.m_chop = 8;
			ie.m_damages.m_pickaxe = 25;
			ie.m_interval = 0.25f;
			ie.m_toolTier = 4;
			ie.m_hitType = HitData.HitType.Tree;
			ie.m_triggerMask = new LayerMask();
			ie.m_triggerMask = ~0;
			
		}

		// its not a deep clone
		public static EnvSetup GetEnvSetupClone(EnvSetup envSetup, string name)
		{
			EnvSetup es = new EnvSetup();
			es.m_name = name;
			es.m_ambColorDay = envSetup.m_ambColorDay;
			es.m_ambColorNight = envSetup.m_ambColorNight;
			es.m_envObject = envSetup.m_envObject;
			es.m_fogColorDay = envSetup.m_fogColorDay;
			es.m_fogColorEvening = envSetup.m_fogColorEvening;
			es.m_fogColorMorning = envSetup.m_fogColorMorning;
			es.m_fogColorNight = envSetup.m_fogColorNight;
			es.m_fogColorSunDay = envSetup.m_fogColorSunDay;
			es.m_fogColorSunEvening = envSetup.m_fogColorSunEvening;
			es.m_fogColorSunMorning = envSetup.m_fogColorSunMorning;
			es.m_fogColorSunNight = envSetup.m_fogColorSunNight;
			es.m_fogDensityDay = envSetup.m_fogDensityDay;
			es.m_psystems = envSetup.m_psystems;

			return es;
		}
		public static void PrepareEnvironments()
		{

			s_miomeSetupMeadows = EnvMan.instance.GetBiomeEnvSetup(Heightmap.Biome.Meadows);
			foreach(var elem in s_miomeSetupMeadows.m_environments)
			{
				if (elem.m_env.m_name.Equals("Clear"))
				{
					FallHeim.s_envClear = GetEnvSetupClone(elem.m_env, "worldfall_cut");
					FallHeim.s_envClear.m_musicDay = "boss_fader";
					FallHeim.s_envClear.m_musicEvening = "boss_fader";
					FallHeim.s_envClear.m_musicMorning = "boss_fader";
					FallHeim.s_envClear.m_musicNight = "boss_fader";
					FallHeim.s_envClear.m_name = "worldfall_cut";
				}
			}
			var ashenv = EnvMan.instance.GetBiomeEnvSetup(Heightmap.Biome.AshLands);
			
			foreach (var elem in ashenv.m_environments)
			{

				if (elem.m_env.m_name.Equals("Ashlands_ashrain"))
				{
					s_ambientFallCalm = elem.m_env.m_ambientLoop;
					FallHeim.s_psysDebris = elem.m_env.m_psystems[0];
					var newPsys = new GameObject[FallHeim.s_envClear.m_psystems.Length+1];
					for (int i = 0; i < FallHeim.s_envClear.m_psystems.Length; i++) newPsys[i] = FallHeim.s_envClear.m_psystems[i];
					newPsys[FallHeim.s_envClear.m_psystems.Length] = elem.m_env.m_psystems[0];
					newPsys[FallHeim.s_envClear.m_psystems.Length].transform.localRotation = Quaternion.Euler(180,0,0);
					newPsys[FallHeim.s_envClear.m_psystems.Length].transform.localPosition = new Vector3(0,9,0);
					FallHeim.s_envClear.m_psystems = newPsys;

					FallHeim.s_envAsh = GetEnvSetupClone(elem.m_env, "worldfall_fall");
					FallHeim.s_envAsh.m_name = "worldfall_fall";
					FallHeim.s_envAsh.m_musicDay = "boss_fader";
					FallHeim.s_envAsh.m_musicEvening = "boss_fader";
					FallHeim.s_envAsh.m_musicMorning = "boss_fader";
					FallHeim.s_envAsh.m_musicNight = "boss_fader";
				}
				else if (elem.m_env.m_name.Equals("Ashlands_storm"))
				{
					s_ambientFallIntsense = elem.m_env.m_ambientLoop;
					FallHeim.s_envStorm = GetEnvSetupClone(elem.m_env, "worldfall_storm");
					FallHeim.s_envStorm.m_name = "worldfall_storm";
					FallHeim.s_envStorm.m_musicDay = "boss_fader";
					FallHeim.s_envStorm.m_musicEvening = "boss_fader";
					FallHeim.s_envStorm.m_musicMorning = "boss_fader";
					FallHeim.s_envStorm.m_musicNight = "boss_fader";
					foreach(var sys in FallHeim.s_envStorm.m_psystems)
						sys.transform.localRotation = Quaternion.Euler(180, 0, 0);
				}

			}

			EnvMan.instance.AppendEnvironment(FallHeim.s_envClear);
			EnvMan.instance.AppendEnvironment(FallHeim.s_envAsh);
			EnvMan.instance.AppendEnvironment(FallHeim.s_envStorm);
		}

		public static void Init()
		{
			if (!FallHeim.s_init) FallHeim.AddRigidbody2Prefabs();
	
			WorldFall.s_isDead = false;
			WorldFall.s_rotationToTargetTop = Quaternion.identity;
			WorldFall.s_biomeColors.Clear();
			WorldFall.s_noGrass = false;
			CrackState.s_ashed = 0;
			Physics.gravity = WorldFall.s_initialGravity;
			FallHeim.AddRigidbody2Dungeons();
			FallHeim.PrepareEnvironments();
			FallHeim.s_init = true;
		}

		public static void AddRigidbody(GameObject prefab)
		{
			if (prefab.GetComponent<Rigidbody>() != null) return;
			if (prefab.GetComponent<Location>() != null) return;
			if (prefab.GetComponent<LocationProxy>() != null) return;

			float density = 50f;
			Bounds bounds = CalculateCombinedBounds(prefab.transform);
			float volume = bounds.size.x * bounds.size.y * bounds.size.z;
			float mass = volume * density;
			if (mass >= 0) mass = 10;			

			Rigidbody rb = prefab.AddComponent<Rigidbody>();
			rb.isKinematic = true;
			
			if (prefab.GetComponent<DungeonGenerator>() != null)
			{
				mass = 100000;
				rb.drag = 25f;
			}
			rb.mass = mass;
		}

		public static void ActivateRigidBody(GameObject prefab)
		{
			foreach (var sp in prefab.GetComponentsInChildren<StaticPhysics>()) Destroy(sp);
			//foreach (var elem in prefab.GetComponentsInChildren<LODGroup>()) Destroy(elem);
			//foreach (var elem in prefab.GetComponentsInChildren<LodFadeInOut>()) Destroy(elem);
			Rigidbody rb = prefab.GetComponent<Rigidbody>();
			if (rb != null) rb.isKinematic = false;

		}

		public static void PrepareStartLocation(GameObject prefab)
		{
			if (PrefabManager.Instance.GetPrefab("startForceField") != null) return;
			var loctrader = PrefabManager.Instance.GetPrefab("Vendor_BlackForest");
			var ff = loctrader.transform.Find("ForceField");
			var ffs = PrefabManager.Instance.CreateClonedPrefab("startForceField", ff.gameObject);
			
			ffs.transform.parent = prefab.transform;
			ffs.transform.localPosition = Vector3.zero;

			// make force field also visible from inside
			MeshFilter meshFilter = ffs.GetComponent<MeshFilter>();
			if (meshFilter != null)
			{
				Mesh mesh = meshFilter.mesh;
				Vector3[] normals = mesh.normals;
				for (int i = 0; i < normals.Length; i++)
				{
					normals[i] = -normals[i];
				}
				mesh.normals = normals;

				for (int i = 0; i < mesh.subMeshCount; i++)
				{
					int[] triangles = mesh.GetTriangles(i);
					for (int j = 0; j < triangles.Length; j += 3)
					{
						int temp = triangles[j];
						triangles[j] = triangles[j + 1];
						triangles[j + 1] = temp;
					}
					mesh.SetTriangles(triangles, i);
				}
			}
			MeshRenderer meshRenderer = ffs.GetComponent<MeshRenderer>();
			if (meshRenderer != null) meshRenderer.sharedMaterial.color = new Color(1f, 0f, 1f, 1f);
			
			var vegvesir = prefab.transform.Find("Vegvisir_Eikthyr");
			if (vegvesir != null)
			{
				vegvesir.transform.parent = null;
				Destroy(vegvesir.gameObject);

				var runestone = PrefabManager.Instance.CreateClonedPrefab("runestonestart","RuneStone_Meadows");
				if (runestone != null)
				{
					runestone.transform.parent = prefab.transform;

					PreparePrefab(runestone);
					AddRigidbody(runestone);

					runestone.transform.localPosition = new Vector3(-3.52f, -0.219f, 3.11f);
					runestone.transform.localRotation = Quaternion.Euler(0, 310, 0);

					var rss = runestone.GetComponent<RuneStone>();
					rss.m_randomTexts = new System.Collections.Generic.List<RuneStone.RandomRuneText>();
					var text = new RuneStone.RandomRuneText();
					if (text != null)
					{
						// no need for translations in a terrible mod
						text.m_text = "In the Age of Twilight, when shadows grew long and the gods faced relentless foes, Odin, All-Father of Asgard, conceived a desperate plan. To rid himself of his enemies once and for all, he resolved to sever Valheim from Yggdrasil, the World Tree. By cutting the realm adrift, he sought to cast his adversaries into the void beyond the Ten Realms.\r\n\r\nYet, not all among the Aesir shared Odin's ruthless vision. Heimdall, the ever-watchful guardian of Bifrost, foresaw the calamity this act would bring. To preserve the world and its inhabitants, Heimdall created a time capsule at the very moment Odin's blade cleaved the world's tether.\r\n\r\nWithin this time capsule, reality persists as it was, a sanctuary shielded from the cataclysm. Beyond its bounds, the world teeters on the edge of oblivion, forever frozen in the instant of its severance.\r\n\r\nBrave soul, you now stand within Heimdall's last refuge, protected by his foresight. Venture beyond this force field, and you will witness the final moment of Valheim's fall. Only by understanding this fateful lore can you hope to navigate the shattered remnants of a world adrift and perhaps restore the balance Odin's wrath has sundered.\r\n\r\nMay the gods guide you. Remember the tale of the World Tree's severance and the courage of Heimdall, who defied the All-Father to give Valheim a sliver of hope.";
						text.m_topic = "Heimdall's Sanctuary: The Fall of Valheim";
						text.m_label = "Fallen World";
						rss.m_randomTexts.Add(text);
					}
				}			
			}	
		}

        public static void AddRigidbody2Prefabs()
        {
			var locstart = PrefabManager.Instance.GetPrefab("StartTemple");
			PrepareStartLocation(locstart);

			var prefabs = ZNetScene.instance.GetPrefabNames();

			foreach (var prefabName in prefabs)
			{
				if (prefabName.StartsWith("vfx") || prefabName.StartsWith("sfx")) continue;
				if (prefabName.Equals("Valkyrie")) continue;
				var prefab = PrefabManager.Instance.GetPrefab(prefabName);
				if (prefab.GetComponent<Rigidbody>() != null) continue;

				PreparePrefab(prefab);
				AddRigidbody(prefab);
			}

			
			var branch = PrefabManager.Instance.GetPrefab("YggdrasilBranch");
			PreparePrefab(branch);
			s_flyaway = branch.AddComponent<SimulateFlyingAway>();
			s_flyaway.m_minScale = 0.005f;
			s_flyaway.m_maxScale = 1.0f;
			s_flyaway.m_scalingSpeed = 0f;
			s_flyaway.m_rotationSpeed = 0f;

			s_sfxTreeHit = PrefabManager.Instance.GetPrefab("sfx_tree_hit");
			s_sfxTreeFall = PrefabManager.Instance.CreateClonedPrefab("sfx_worldfall_tree_fall","sfx_tree_fall");
			var zs = s_sfxTreeFall.GetComponent<ZSFX>();
			var clip = zs.m_audioClips[3];
			zs.m_audioClips = new AudioClip[1] { clip };
			s_vfxTreeHit = PrefabManager.Instance.GetPrefab("vfx_yggashoot_cut");

		}

		public static void AddRigidbody2Locations()
		{
			foreach (var location in ZoneSystem.instance.m_locations)
			{
				AddRigidbody2Location(location);
			}
		}

		public static void AddRigidbody2Location(ZoneSystem.ZoneLocation location)
		{
			
			if (location.m_prefab == null) return;

			if (!location.m_prefab.IsValid) return;

			location.m_prefab.Load();
			location.m_prefab.HoldReference();

			foreach (var elem in location.m_prefab.Asset.GetComponentsInChildren<MeshRenderer>())
			{
				Vector3 pos = elem.gameObject.transform.localPosition;
				PreparePrefab(elem.gameObject);
				AddRigidbody(elem.gameObject);
				elem.transform.localPosition = pos;
			}

			if (location.m_prefab.m_name == "StartTemple") PrepareStartLocation(location.m_prefab.Asset);
			
		}

		public static void AddRigidbody2Dungeons()
		{
			foreach (var room in DungeonDB.instance.m_rooms)
			{
				room.m_prefab.Load();
				room.m_prefab.HoldReference();

				foreach (var elem in room.m_prefab.Asset.GetComponentsInChildren<MeshRenderer>())
				{
					if (IsDungeonPiece(elem.transform)) continue;
					PreparePrefab(elem.gameObject);
					AddRigidbody(elem.gameObject);
				}

			}
		}

		public static bool IsDungeonPiece(Transform transform)
		{
			if (transform.name.StartsWith("props") || transform.name.StartsWith("Props") ) return false;
			else if (transform.parent != null) return IsDungeonPiece(transform.parent);
			else return true;
		}


		public static void ChangeLayerRecursively(GameObject obj, int newLayer)
		{
			obj.layer = newLayer;

			foreach (Transform child in obj.transform)
			{
				ChangeLayerRecursively(child.gameObject, newLayer);
			}
		}

		public static Bounds CalculateCombinedBounds(Transform transform)
		{
			Renderer[] renderers = transform.GetComponentsInChildren<MeshRenderer>();

			Bounds combinedBound = new Bounds(Vector3.zero, Vector3.zero);

			foreach (Renderer renderer in renderers)
			{
				if (!renderer.gameObject.activeSelf) continue;
				Bounds bounds = renderer.bounds;
				if (bounds == null) continue;
				if (combinedBound.center == Vector3.zero && combinedBound.size == Vector3.zero) combinedBound = bounds;
				else combinedBound.Encapsulate(bounds);
			}

			return combinedBound;
		}

		public static Transform GetTopmostTransform(Transform obj)
		{
			Transform topmost = obj;
			while (topmost.parent != null) topmost = topmost.parent;
			return topmost;
		}

		public static Vector3 GetStartLocation()
		{
			if (ZoneSystem.instance == null) return Vector3.zero;
			foreach (var loc in ZoneSystem.instance.GetLocationList()) if (loc.m_location.m_prefabName.StartsWith("StartTemple")) return loc.m_position;
			return Vector3.zero;
		}

		void OnDestroy()
		{
			harmony.UnpatchSelf();
		}
	}
}

