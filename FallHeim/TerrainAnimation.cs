namespace FallHeim
{
	using UnityEngine;

	public class TerrainAnimation : MonoBehaviour
	{
		public float m_rotationSpeed = 10f;
		public float m_moveSpeed = 1f;

		private Vector3 randomRotationDirection;

		void Start()
		{
			randomRotationDirection = new Vector3(
				Random.Range(-1f, 1f),
				Random.Range(-1f, 1f),
				Random.Range(-1f, 1f)
			).normalized;
		}

		void Update()
		{
			float rotationAmount = m_rotationSpeed * Time.deltaTime;
			transform.Rotate(randomRotationDirection * rotationAmount, Space.World);
			float moveAmount = m_moveSpeed * Time.deltaTime;
			transform.Translate(Vector3.up * moveAmount, Space.World);
		}
	}

}
