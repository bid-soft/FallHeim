

namespace FallHeim.States
{
	public class FallStateMachine
	{
		private AbstractFallState m_currentState;

		public void ChangeState(AbstractFallState newState)
		{
			if (m_currentState != null)
			{
				m_currentState.Exit();
			}

			m_currentState = newState;

			if (m_currentState != null)
			{
				m_currentState.SetStateMachine(this);
				m_currentState.Enter();
			}
		}

		public void Update()
		{
			if (m_currentState != null)
			{
				m_currentState.Update();
			}
		}
	}
}
