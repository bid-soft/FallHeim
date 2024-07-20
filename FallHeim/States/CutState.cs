using UnityEngine;

namespace FallHeim.States
{
	public class CutState : FallState
	{
		
		public CutState(Vector3 gravity, float duration) : base(gravity, duration) 
		{
			m_soundIntervalMin = 0.1f;
			m_soundIntervalMax = 0.1f;
		}


		public override void Enter()
		{
			base.Enter();

			EnvMan.instance.m_debugEnv = "worldfall_cut";
			Vector3 pos = Player.m_localPlayer.transform.position;
			pos.y += 5;
			FallHeim.Instantiate(FallHeim.s_sfxTreeHit, pos, Quaternion.identity);
			pos.x += 30;
			FallHeim.Instantiate(FallHeim.s_vfxTreeHit, pos, Quaternion.identity);
			pos.x -= 60;
			FallHeim.Instantiate(FallHeim.s_vfxTreeHit, pos, Quaternion.identity);
			pos.z += 30;
			FallHeim.Instantiate(FallHeim.s_vfxTreeHit, pos, Quaternion.identity);
			pos.z -= 60;
			FallHeim.Instantiate(FallHeim.s_vfxTreeHit, pos, Quaternion.identity);

			EnvMan.instance.m_currentEnv.m_ambientLoop = FallHeim.s_ambientFallCalm;
			AudioMan.instance.QueueAmbientLoop(FallHeim.s_ambientFallCalm, 1f);

			AddHitSounds();
			
		}

		public override void Update()
		{
			base.Update();

			
		}

	}
		

}
