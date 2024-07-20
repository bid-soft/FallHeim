

using System.Collections.Generic;
using UnityEngine;

namespace FallHeim.States
{
	public abstract class AbstractFallState
	{
		protected FallStateMachine m_stateMachine;

		protected AbstractFallState m_nextState;

		protected List<GameObject> m_sounds = new List<GameObject>();

		private float m_nextSound = 0f;
		protected float m_soundIntervalMin = 0.4f;
		protected float m_soundIntervalMax = 5f;

		public AbstractFallState()
		{
			m_nextSound = Time.realtimeSinceStartup + Random.Range(m_soundIntervalMin, m_soundIntervalMax);			
		}

		public AbstractFallState AddNextState(AbstractFallState nextState)
		{
			m_nextState = nextState;
			return nextState;
		}

		public void SetStateMachine( FallStateMachine stateMachine )
		{
			m_stateMachine = stateMachine;
		}

		abstract public void Enter();

		virtual public void Update()
		{
			if (Player.m_localPlayer == null) return;
			if (Time.realtimeSinceStartup >= m_nextSound && m_sounds.Count>0) 
			{
				Object.Instantiate<GameObject>(m_sounds[UnityEngine.Random.Range(0, m_sounds.Count - 1)], GetRandomPointAround(Player.m_localPlayer.transform.position,7), Quaternion.identity);
				m_nextSound = Time.realtimeSinceStartup + Random.Range(m_soundIntervalMin, m_soundIntervalMax);
			}
		}

		abstract public void Exit();

		public void AddHitSounds()
		{
			foreach (var ch in Character.s_characters)
			{
				foreach (var elem in ch.m_hitEffects.m_effectPrefabs)
				{
					if (elem.m_prefab.name.ToUpper().StartsWith("SFX"))
					{
						m_sounds.Add(elem.m_prefab);
					}
				}
			}
		}

		public void AddDeathSounds()
		{
			foreach (var ch in Character.s_characters)
			{
				foreach (var elem in ch.m_deathEffects.m_effectPrefabs)
				{
					if (elem.m_prefab.name.ToUpper().StartsWith("SFX"))
					{
						m_sounds.Add(elem.m_prefab);
					}
				}
			}
		}


		private Vector3 GetRandomPointAround(Vector3 center, float radius)
		{
			float angle = Random.Range(0f, Mathf.PI * 2);
			//float distance = Random.Range(0f, radius);
			float distance = radius;
			float xOffset = Mathf.Cos(angle) * distance;
			float zOffset = Mathf.Sin(angle) * distance;
			Vector3 randomPoint = new Vector3(center.x + xOffset, center.y, center.z + zOffset);

			return randomPoint;
		}

		public void RemoveDrops()
		{
			foreach (ItemDrop itemDrop in Object.FindObjectsOfType<ItemDrop>())
			{				
				ZNetView zn = itemDrop.GetComponent<ZNetView>();
				if (zn && zn.IsValid() && zn.IsOwner()) zn.Destroy();
			}
		}

	}
		

}
