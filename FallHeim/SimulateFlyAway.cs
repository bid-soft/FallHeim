using UnityEngine;

namespace FallHeim
{
	public class SimulateFlyingAway : MonoBehaviour
	{
		public float m_scalingSpeed = 0.01f;
		public float m_maxScale = 1f;
		public float m_minScale = 0.01f;
		public float m_rotationSpeed = 10f;

	
		void Update()
		{
			if (Player.m_localPlayer == null) return;

			float newScaleFactor = Mathf.Lerp(transform.localScale.x, m_minScale, 1 - Mathf.Exp(-m_scalingSpeed * Time.deltaTime));
			Vector3 newScale = new Vector3(newScaleFactor, newScaleFactor, newScaleFactor);
			transform.localScale = newScale;
			transform.position = new Vector3(Player.m_localPlayer.transform.position.x, 2000, Player.m_localPlayer.transform.position.z);
			transform.Rotate(Vector3.up, m_rotationSpeed * Time.deltaTime);
		}
	}

}
