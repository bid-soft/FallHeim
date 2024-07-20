
using UnityEngine;

namespace FallHeim.States
{
	public class InitialFallState : FallState
	{
	
		public InitialFallState(Vector3 gravity, float duration) : base(gravity, duration) 
		{
			m_soundIntervalMin = 0.1f;
			m_soundIntervalMax = 2f;
		}

		public override void Enter()
		{
			base.Enter();
			Vector3 pos = Player.m_localPlayer.transform.position;
			pos.y += 5;
			FallHeim.Instantiate(FallHeim.s_sfxTreeFall, pos, Quaternion.identity);

			AddHitSounds();
		}


		public override void Update()
		{
			base.Update();
			EnvMan.instance.m_currentEnv.m_lightIntensityDay -= 0.001f;
		}


	}
		

}
