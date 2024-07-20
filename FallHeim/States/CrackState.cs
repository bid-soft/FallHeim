using Jotunn.Managers;
using System.Collections.Generic;
using UnityEngine;
using static FallHeim.WorldFall;

namespace FallHeim.States
{
	public class CrackState : RotationState
	{
		float m_elapsedTime = 0f;

		public static float s_ashed = 0f;

		public CrackState(Vector3 gravity, float duration) : base(duration) 
		{
			m_soundIntervalMin = 0.1f;
			m_soundIntervalMax = 2f;
		}

		public override void Enter()
		{
			base.Enter();

			EnvMan.instance.m_debugEnv = "worldfall_storm";

			GameCamera.instance.AddShake(Player.m_localPlayer.transform.position, 100, 2, false);

			m_sounds.Clear();
			m_sounds.Add(PrefabManager.Instance.GetPrefab("sfx_eikthyr_hit"));
			m_sounds.Add(PrefabManager.Instance.GetPrefab("sfx_gdking_scream"));
			m_sounds.Add(PrefabManager.Instance.GetPrefab("sfx_Bonemass_Hit"));
			m_sounds.Add(PrefabManager.Instance.GetPrefab("sfx_dragon_scream"));
			m_sounds.Add(PrefabManager.Instance.GetPrefab("sfx_fader_fissure"));
			m_sounds.Add(PrefabManager.Instance.GetPrefab("sfx_seeker_brute_alerted"));
			m_sounds.Add(PrefabManager.Instance.GetPrefab("sfx_goblinking_taunt"));

		}

		public override void Update()
		{
			base.Update();

			if (Player.m_localPlayer == null) return;

			m_elapsedTime += Time.deltaTime;

			if (m_elapsedTime > 5f) 
			{
				m_elapsedTime = 0f;

				s_ashed += 0.15f;
				if (s_ashed > 0.9f) s_ashed = 0.9f;

				foreach (var hm in Heightmap.s_heightmaps)
				{
					var compiler = hm.GetAndCreateTerrainCompiler();
					CreateCrack(compiler.m_hmap, ref compiler.m_levelDelta, ref compiler.m_modifiedHeight, ref compiler.m_paintMask,
						ref compiler.m_modifiedPaint, 7, -2);
					compiler.Save();
					compiler.m_hmap.Poke(false);
				}
				
				ClutterSystem.instance?.ResetGrass(Player.m_localPlayer.transform.position, 1000);

				GameCamera.instance.AddShake(Player.m_localPlayer.transform.position, 100, 2, false);

			}

		}

		public static void CreateCrack(Heightmap hmap, ref float[] heights, ref bool[] modifiedHeights,
			ref Color[] paint, ref bool[] modifiedPaint,
			int crackWidth = 5, float crackDepth = -20f, float crackNoise = 0.5f)
		{
			int width = 64;

			int edge = Random.Range(0, 3);
			int startX = 0, startY = 0;

			switch (edge)
			{
				case 0: 
					startX = Random.Range(1, width - 1);
					startY = 1;
					break;
				case 1: 
					startX = Random.Range(1, width - 1);
					startY = width - 1;
					break;
				case 2: 
					startX = 1;
					startY = Random.Range(1, width - 1);
					break;
				case 3: 
					startX = width - 1;
					startY = Random.Range(1, width - 1);
					break;
			}

			float centerX = width / 2f;
			float centerY = width / 2f;
			float currentAngle = Mathf.Atan2(centerY - startY, centerX - startX);

			List<Vector2Int> crackPath = new List<Vector2Int>();
			Vector2Int currentPoint = new Vector2Int(startX, startY);
			crackPath.Add(currentPoint);

			while (currentPoint.x >= 1 && currentPoint.x <= width - 1 && currentPoint.y >= 1 && currentPoint.y <= width - 1)
			{
				if (Random.value < 0.1f) currentAngle += Random.Range(-Mathf.PI / 2, Mathf.PI / 2);

				int nextX = currentPoint.x + Mathf.RoundToInt(Mathf.Cos(currentAngle));
				int nextY = currentPoint.y + Mathf.RoundToInt(Mathf.Sin(currentAngle));

				nextX = Mathf.Clamp(nextX, 1, width - 1);
				nextY = Mathf.Clamp(nextY, 1, width - 1);

				currentPoint = new Vector2Int(nextX, nextY);
				crackPath.Add(currentPoint);

				if (nextX == 1 || nextX == width - 1 || nextY == 1 || nextY == width - 1) break;
			}

			foreach (Vector2Int point in crackPath)
			{
				for (int i = -crackWidth / 2; i <= crackWidth / 2; i++)
				{
					int x = point.x + i;
					if (x > 0 && x < width-1)
					{
						int index = point.y * (width + 1) + x;
						if (index >= 0 && index < heights.Length)
						{
							heights[index] += crackDepth + Random.Range(-crackNoise, crackNoise);
							modifiedHeights[index] = true;
							paint[index].a = 1f;
							modifiedPaint[index] = true;

							Color32 color = Heightmap.GetBiomeColor(hmap.m_cornerBiomes[0]);
							float ix = DUtils.SmoothStep(0f, 1f, (float)((double)x / (double)width));
							float iy = DUtils.SmoothStep(0f, 1f, (float)((double)point.y / (double)width));
							var hmkey = new HeightmapKey(hmap, ix, iy);
							s_biomeColors[hmkey] = Color32.Lerp(color, new Color32(byte.MaxValue, 0, 0, byte.MaxValue), 1);
							for (int iix = -1; iix <= 1; iix++)
							{
								for (int iiy = -1; iiy <= 1; iiy++)
								{
									if (iix == 0 && iiy == 0) continue;
									UpdateBiomeColorAndPaint(hmap, color, iix, iiy, width, paint);
								}
							}
						}
					}
				}
			}

		}

		public static void UpdateBiomeColorAndPaint(Heightmap hmap, Color color, int x, int y, int width, Color[] paint)
		{
			float ix = DUtils.SmoothStep(0f, 1f, (float)x / width);
			float iy = DUtils.SmoothStep(0f, 1f, (float)y / width);
			HeightmapKey hmkey = new HeightmapKey(hmap, ix, iy);

			if (!s_biomeColors.ContainsKey(hmkey))
			{
				s_biomeColors[hmkey] = Color32.Lerp(color, new Color32(byte.MaxValue, 0, 0, byte.MaxValue), 0.75f);
				int tindex = y * (width + 1) + x;
				if (tindex >= 0 && tindex < paint.Length) paint[tindex].a = 0.75f;
			}
		}

	}
		

}
