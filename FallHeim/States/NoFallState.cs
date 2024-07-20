using UnityEngine;


namespace FallHeim.States
{
	public class NoFallState : AbstractFallState
	{
		Vector3 StartLocation = Vector3.zero;
		bool Active = false;

		public override void Enter()
		{
			EnvMan.instance.m_debugEnv = "Clear";
			EnvMan.instance.m_debugTimeOfDay = true;
			EnvMan.instance.m_debugTime = 0.5f;
			StartLocation = FallHeim.GetStartLocation();
			Physics.gravity = WorldFall.s_initialGravity;
			MusicMan.instance.m_currentMusicVol = 1f;
		}

		public override void Update()
		{
			if (Player.m_localPlayer == null) return;
			if (Active && StartLocation.DistanceTo(Player.m_localPlayer.transform.position) > 12 && m_nextState != null) m_stateMachine.ChangeState(m_nextState);	
			else if (StartLocation.DistanceTo(Player.m_localPlayer.transform.position) < 11) Active=true;
		}

		
		public override void Exit()
		{
			
		}


		
	}
		

}
