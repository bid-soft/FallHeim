using UnityEngine;

namespace FallHeim
{
	public class SmoothFallAndRotate : MonoBehaviour
	{
		public float m_fallSpeed = 1f; 
		public float m_rotationSpeed = 5f; 
		public Vector3 m_rotationAxis = Vector3.up; 
		public float m_rotationVariation = 5f; 
		public float m_fallSpeedVariation = 0.2f; 
		public float m_smoothTime = 10f; 
		public float m_startTime;
		public Vector3 m_fallDirection = Vector3.down;
		public float m_scaleTime = 0f;

		private Rigidbody m_rb;
		private float m_currentFallSpeed;
		private float m_targetFallSpeed;
		private Vector3 m_currentRotationAxis;
		private Vector3 m_targetRotationAxis;
		private float m_fallVelocity;
		private Vector3 m_rotationVelocity;
		private Vector3 m_startScale;
		private float m_timeElapsed;
		

		void Start()
		{
			m_rb = GetComponent<Rigidbody>();

			if (m_rb == null) return;

			m_rb.isKinematic = true;
			m_currentFallSpeed = m_fallSpeed;
			m_targetFallSpeed = m_fallSpeed;
			m_currentRotationAxis = m_rotationAxis;
			m_targetRotationAxis = m_rotationAxis;
			m_startScale = transform.localScale;

			m_startTime = Time.realtimeSinceStartup + 5f;
		}

		void Update()
		{
			if (m_startTime > Time.realtimeSinceStartup) return;

			m_timeElapsed += Time.deltaTime;

			m_targetFallSpeed = m_fallSpeed + UnityEngine.Random.Range(-m_fallSpeedVariation, m_fallSpeedVariation);
			m_targetRotationAxis = m_rotationAxis + new Vector3(
				UnityEngine.Random.Range(-m_rotationVariation, m_rotationVariation),
				UnityEngine.Random.Range(-m_rotationVariation, m_rotationVariation),
				UnityEngine.Random.Range(-m_rotationVariation, m_rotationVariation)
			);

			m_currentFallSpeed = Mathf.SmoothDamp(m_currentFallSpeed, m_targetFallSpeed, ref m_fallVelocity, m_smoothTime);
			m_currentRotationAxis = Vector3.SmoothDamp(m_currentRotationAxis, m_targetRotationAxis, ref m_rotationVelocity, m_smoothTime);

			transform.position += m_fallDirection * m_currentFallSpeed * Time.deltaTime;

			transform.Rotate(m_currentRotationAxis.normalized, m_rotationSpeed * Time.deltaTime);
			
			if (m_scaleTime> 0) transform.localScale = new Vector3(Mathf.Lerp(m_startScale.x, 0, m_timeElapsed / m_scaleTime),
																Mathf.Lerp(m_startScale.y, 0, m_timeElapsed / m_scaleTime), 
																Mathf.Lerp(m_startScale.z, 0, m_timeElapsed / m_scaleTime));

		}
	}
}
