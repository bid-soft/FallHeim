
namespace FallHeim
{
	using UnityEngine;

	public class TriggerEventCaller : MonoBehaviour
	{
		public float m_triggerRadius = 75f; // Radius for the trigger sphere
		
		void Start()
		{
		
			// Add a SphereCollider as a trigger to the GameObject this script is attached to
			SphereCollider triggerCollider = gameObject.AddComponent<SphereCollider>();
			triggerCollider.isTrigger = true;
			triggerCollider.radius = m_triggerRadius;
			
		}

		

		void OnTriggerEnter(Collider other)
		{
			var top = FallHeim.GetTopmostTransform(other.transform);
			if (top.GetComponent<DungeonGenerator>() != null)
			{
				if (top.GetComponent<SmoothFallAndRotate>() == null)				
					top.gameObject.AddComponent<SmoothFallAndRotate>();					
				return;
			}

			foreach (var elem in FallHeim.GetTopmostTransform(other.transform).gameObject.GetComponentsInChildren<Rigidbody>())
				FallHeim.ActivateRigidBody(elem.gameObject);
			
		}
	}

}
