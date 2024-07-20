using Jotunn.Managers;
using UnityEngine;

namespace FallHeim.States
{
	public class EndState : FallState
	{
		float m_elapsedTime = 0f;

		float m_waterHeight = 0f;

		public EndState(): base() 
		{
			m_soundIntervalMin = 0.1f;
			m_soundIntervalMax = 2f;
		}
		

		public override void Enter()
		{
			base.Enter();

			EnvMan.instance.m_debugTimeOfDay = true;
			EnvMan.instance.m_debugTime = 0.5f;


			m_waterHeight = GetWaterHeight(Player.m_localPlayer.transform.position);
			
			WorldFall.s_noGrass = true;
			ClutterSystem.instance.ClearAll();

			m_sounds.Clear();
			m_sounds.Add(PrefabManager.Instance.GetPrefab("sfx_eikthyr_death"));
			m_sounds.Add(PrefabManager.Instance.GetPrefab("sfx_gdking_death"));
			m_sounds.Add(PrefabManager.Instance.GetPrefab("sfx_Bonemass_death"));
			m_sounds.Add(PrefabManager.Instance.GetPrefab("sfx_dragon_death"));			
			m_sounds.Add(PrefabManager.Instance.GetPrefab("sfx_fader_taunt"));
			m_sounds.Add(PrefabManager.Instance.GetPrefab("sfx_seeker_brute_taunt"));
			m_sounds.Add(PrefabManager.Instance.GetPrefab("sfx_goblinking_taunt"));

		}

		public override void Update()
		{
			m_elapsedTime += Time.deltaTime;

		
			if (Player.m_localPlayer.transform.position.y <= m_waterHeight)
			{
				WorldFall.s_isDead = true;
				Player.m_localPlayer.SetHealth(0);
			}

			if (m_elapsedTime > 3)
			{
				m_elapsedTime = 0;
				EnvMan.instance.m_debugTime -= 0.1f;
				if (EnvMan.instance.m_debugTime < 0)
				{
					EnvMan.instance.m_debugTime = 0;
					WorldFall.s_isDead = true;
					Player.m_localPlayer.SetHealth(0);
				}
				
			}

			// ....
			foreach (var hm in Heightmap.s_heightmaps)
				if (hm.gameObject.GetComponent<TerrainAnimation>() == null) hm.gameObject.AddComponent<TerrainAnimation>();

			foreach(var water in WaterVolume.Instances)
			{
				var w = water.transform.parent.Find("WaterSurface");
				if (w != null) w.gameObject.SetActive(false);
			}

			base.Update();

			
		}


		public float GetWaterHeight(Vector3 position)
		{
			int mask = LayerMask.GetMask(new string[] { "WaterVolume" });
			Collider[] array = Physics.OverlapSphere(position, 100, mask);

			float depth = 0;
			WaterVolume m_waterVolume;

			foreach (Collider c in array)
			{
				m_waterVolume = c.GetComponent<WaterVolume>();
				if (m_waterVolume != null)
				{
					depth = m_waterVolume.GetWaterSurface(position);
					break;
				}
			}

			return depth;
		}

	}
		

}
