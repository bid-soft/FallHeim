
using UnityEngine;

namespace FallHeim.States
{
	public class HitState : FallState
	{
		public HitState(Vector3 gravity, float duration) : base(gravity, duration) 
		{
			m_soundIntervalMin = 0.1f;
			m_soundIntervalMax = 0.2f;
		}
		

		public override void Enter()
		{
			base.Enter();

			MusicMan.instance.m_musicFadeTime = 0f;

			EnvMan.instance.m_debugEnv = "worldfall_fall";

			AddDeathSounds();

		}

		public override void Update()
		{
			base.Update();

		}

		
		public override void Exit()
		{
			base.Exit();
		}


	
	}
		

}
