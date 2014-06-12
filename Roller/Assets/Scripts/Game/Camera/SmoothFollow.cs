using UnityEngine;
namespace Game
{
	public class SmoothFollow : MonoBehaviour
	{
		public Transform Target;				
		public float Distance = 10.0f;
		public float Height = 5.0f;
		public float Damping = 3.0f;
		public float LookOffsetForward = 0.0f;
		public float LookOffsetRight = 0.0f;
		public float LookOffsetUp = 0.0f;
		public bool ShowDebugTarget = false;
		public Transform DebugTarget;

		public float LookAheadHorizontal = 0.0f;

		private Transform myTransform;

		void Awake()
		{
			myTransform = transform;
			if(ShowDebugTarget)
			{
				showTarget(true);
			}
		}
		private Vector3 lastLookDirection = Vector3.forward;
		void LateUpdate()
		{
			Vector3 offsetDirection;
			offsetDirection = Target.forward * -1.0f;
			offsetDirection *= Distance;
			offsetDirection.y = Height;
			
			Vector3 targetPosition = Target.position + offsetDirection;
			Vector3 velocity = new Vector3();
			myTransform.position = Vector3.SmoothDamp(myTransform.position, targetPosition, ref velocity, Damping * Time.deltaTime);

			//Look at and dampen the rotation
			Vector3 lookPosition = Target.position;
			lookPosition += Target.forward * LookOffsetForward;
			lookPosition += Target.up * LookOffsetUp;
			lookPosition += Target.right * LookOffsetRight;

			Vector3 currentLookDirection = Target.position - myTransform.position;
			currentLookDirection.y = 0;
			currentLookDirection.Normalize();

			float deltaAngle = Mathf.Acos(Vector3.Dot(lastLookDirection, currentLookDirection));
			if(!float.IsNaN(deltaAngle))
			{
				float angleDirection = AngleDir(lastLookDirection, currentLookDirection, myTransform.up);
				lookPosition += Target.right * angleDirection * LookAheadHorizontal * deltaAngle;
			}

			Quaternion rotation = Quaternion.LookRotation(lookPosition - myTransform.position);
			myTransform.rotation = Quaternion.Slerp(myTransform.rotation, rotation, Time.deltaTime * Damping);

			updateDebugTarget(lookPosition);

			lastLookDirection = currentLookDirection;
		}

		private float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) 
		{
			Vector3 perp = Vector3.Cross(fwd, targetDir);
			float dir = Vector3.Dot(perp, up);
			
			if (dir > 0f) 
			{
				return 1f;
			} 
			else if (dir < 0f) 
			{
				return -1f;
			} 
			else 
			{
				return 0f;
			}
		}

		private void updateDebugTarget(Vector3 position)
		{
			if(DebugTarget == null)
			{
				return;
			}

			DebugTarget.position = position;
		}

		private void showTarget(bool show)
		{
			if(DebugTarget == null)
			{
				return;
			}

			DebugTarget.gameObject.SetActive(show);
		}

		public void SetTarget(Transform target)
		{
			Target = target;
		}
	}
}

