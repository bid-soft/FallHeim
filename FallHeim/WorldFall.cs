using FallHeim.States;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FallHeim
{
	
	public class WorldFall : MonoBehaviour
	{
		public struct HeightmapKey : IEquatable<HeightmapKey>
		{
			public Heightmap m_heightmap;
			public float m_x;
			public float m_y;

			public HeightmapKey(Heightmap heightmap, float x, float y)
			{
				m_heightmap = heightmap;
				m_x = x;
				m_y = y;
			}

			public override bool Equals(object obj)
			{
				return obj is HeightmapKey key && Equals(key);
			}

			public bool Equals(HeightmapKey other)
			{
				return EqualityComparer<Heightmap>.Default.Equals(m_heightmap, other.m_heightmap) && m_x == other.m_x && m_y == other.m_y;
			}

			public override int GetHashCode()
			{
				unchecked
				{
					int hash = 17;

					hash = hash * 31 + (m_heightmap != null ? m_heightmap.GetHashCode() : 0);
					hash = hash * 31 + m_x.GetHashCode();
					hash = hash * 31 + m_y.GetHashCode();

					return hash;
				}
			}

			public override string ToString()
			{
				return $"HeightmapKey {{ Heightmap = {m_heightmap}, X = {m_x}, Y = {m_y} }}";
			}
		}

		public static readonly Dictionary<HeightmapKey, Color> s_biomeColors = new Dictionary<HeightmapKey, Color>();

		private FallStateMachine m_fallStateMachine;

		public static Quaternion s_rotationToTargetTop = Quaternion.identity;

		// global states have always been a "good" idea
		public static bool s_isDead = false;

		public static Vector3 s_initialGravity = new Vector3(0, -20, 0);

		public static bool s_noGrass = false;

		public static bool t_rigidActiveTeleport = false;

		void Start()
		{
			m_fallStateMachine = new FallStateMachine();
			var state = new NoFallState();
			state.AddNextState(new CutState(new Vector3(0, 100, 0), 0.35f))
				.AddNextState(new InitialFallState(new Vector3(0.7f, -5f, 0f), 23))
				.AddNextState(new HitState(new Vector3(50, 50, 0), 0.5f))
				.AddNextState(new RotationState(45f))
				.AddNextState(new CrackState(new Vector3(0, -9, 0), 45))
				.AddNextState(new EndState());

			m_fallStateMachine.ChangeState(state);

		}


		public static TerrainComp[] GetCompilers(Vector3 position, float radius)
		{
			List<Heightmap> heightMaps = new List<Heightmap>();
			Heightmap.FindHeightmap(position, radius, heightMaps);
			var pos = ZNet.instance.GetReferencePosition();
			var zs = ZoneSystem.instance;
			var ns = ZNetScene.instance;
			return heightMaps.Where(hmap => ZNetScene.InActiveArea(zs.GetZone(hmap.transform.position), pos)).Select(hmap => hmap.GetAndCreateTerrainCompiler()).ToArray();
		}

		
		void Update()
		{
			m_fallStateMachine.Update();
		}
		
	}

}
