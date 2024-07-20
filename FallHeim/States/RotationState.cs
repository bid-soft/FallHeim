
using UnityEngine;

namespace FallHeim.States
{
	public class RotationState : AbstractFallState
	{
		private float m_elapsedTime = 0f;
		public float m_fallSpeed = -9.81f; 
		public float m_tiltIntensity = 10f; 
		public float m_tiltIntensityY = 1f; 
		public float m_tiltFrequency = 0.5f;
		public float m_tiltFrequencyY = 0.1f; 
		public float m_shakeIntensity = 0.2f; 
		public float m_shakeFrequency = 1f;
		private float m_duration = 1f;
		private float m_clear = 0f;

		
		public RotationState(float duration)
		{
			this.m_duration = duration;

			m_soundIntervalMin = 0.1f;
			m_soundIntervalMax = 2f;
		}

		public override void Enter()
		{
			AddDeathSounds();
			RemoveDrops();
		}

		public override void Update()
		{
			base.Update();

			m_elapsedTime += Time.deltaTime;
			m_clear += Time.deltaTime;

			if (m_clear > 10)
			{
				m_clear = 0;
				// let fps recover a bit
				RemoveDrops();
			}


			if (m_elapsedTime > m_duration && m_nextState != null) m_stateMachine.ChangeState(m_nextState);

			Vector3 fallGravity = new Vector3(0, m_fallSpeed, 0);

			float tiltX = Mathf.Sin(m_elapsedTime * m_tiltFrequency) * m_tiltIntensity;
			float tiltZ = Mathf.Cos(m_elapsedTime * m_tiltFrequency) * m_tiltIntensity;
			float tiltY = Mathf.Sin(m_elapsedTime * m_tiltFrequencyY) * m_tiltIntensityY;

			float shakeX = (Mathf.PerlinNoise(Time.time * m_shakeFrequency, 0) * 2 - 1) * m_shakeIntensity;
			float shakeY = (Mathf.PerlinNoise(0, Time.time * m_shakeFrequency) * 2 - 1) * m_shakeIntensity;

			Vector3 newGravity = new Vector3(tiltX, fallGravity.y + tiltY + shakeY, tiltZ + shakeX);

			Physics.gravity = newGravity;

			if (Player.m_localPlayer != null)
			{
				Vector3 targetUp = -newGravity.normalized;
				WorldFall.s_rotationToTargetTop = Quaternion.Lerp(Quaternion.identity, Quaternion.FromToRotation(Player.m_localPlayer.transform.up, targetUp), 0.5f);
				Player.m_localPlayer.transform.GetChild(0).rotation = WorldFall.s_rotationToTargetTop * Player.m_localPlayer.transform.rotation;
				EnvMan.instance.m_debugTime = newGravity.normalized.x *0.3f+0.5f;
			}

		}

		
		public override void Exit()
		{
			WorldFall.s_rotationToTargetTop = Quaternion.identity;
			RemoveDrops();
		}


	
	}
		

}
