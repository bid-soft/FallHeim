
using UnityEngine;

namespace FallHeim.States
{
	public class FallState : AbstractFallState
	{
		protected float m_startTime = 0f;
		protected float m_duration = 1f;
		protected Vector3 m_gravity = new Vector3(0f, -20f, 0f);

		public FallState() : base() { }
	
		public FallState(Vector3 gravity, float duration): base()
		{
			m_duration = duration;
			m_gravity = gravity;
		}
		

		public override void Enter()
		{
			m_startTime = Time.realtimeSinceStartup;
			Physics.gravity = m_gravity;
			if (Player.m_localPlayer!=null) Player.m_localPlayer.transform.Find("rigid").gameObject.SetActive(true);

			if (FallHeim.s_flyaway != null)
			{
				FallHeim.s_flyaway.m_scalingSpeed = 0.33f;
				FallHeim.s_flyaway.m_rotationSpeed = 100f;
			}

		}

		public override void Update()
		{
			base.Update();
			if (m_nextState!=null && Time.realtimeSinceStartup - m_startTime > m_duration) m_stateMachine.ChangeState(m_nextState);
		}

		
		public override void Exit()
		{

		}


	
	}
		

}
