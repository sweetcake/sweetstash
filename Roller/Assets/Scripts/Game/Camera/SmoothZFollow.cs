using UnityEngine;
namespace Game
{
	public class SmoothZFollow : MonoBehaviour
	{
		public Transform Target;				
		public float Distance = 10.0f;
		public float Height = 5.0f;
		public float Damping = 3.0f;

		public float LookOffsetForward = 0.0f;
		public float LookOffsetRight = 0.0f;
		public float LookOffsetUp = 0.0f;

		private Transform myTransform;

		void Awake()
		{
			myTransform = transform;
		}

		void LateUpdate()
		{
			if(Target == null)
			{
				return;
			}

			//Translational offset from target
			Vector3 offsetDirection;
			offsetDirection = Vector3.forward * -1.0f;
			offsetDirection *= Distance;
			offsetDirection.y = Height;
			
			Vector3 targetPosition = Target.position + offsetDirection;
			Vector3 velocity = new Vector3();
			myTransform.position = Vector3.SmoothDamp(myTransform.position, targetPosition, ref velocity, Damping * Time.deltaTime);
			
			//Look at and dampen the rotation
			Vector3 lookPosition = Target.position;
			lookPosition += Vector3.forward * LookOffsetForward;
			lookPosition += Vector3.up * LookOffsetUp;
			lookPosition += Vector3.right * LookOffsetRight;

			Quaternion rotation = Quaternion.LookRotation(lookPosition - myTransform.position);
			myTransform.rotation = Quaternion.Slerp(myTransform.rotation, rotation, Time.deltaTime * Damping);
		}
	}
}

