using UnityEngine;
namespace Game
{
	public class RotatingCylinder : MonoBehaviour
	{
		public float RotationSpeed = 1.0f;
		public bool ChooseRandomRotationDirection = false;

		void Awake()
		{
			if(ChooseRandomRotationDirection)
			{
				RotationSpeed *= (Random.value < 0.5f ? -1.0f : 1.0f);
			}
		}

		void Update()
		{
			rigidbody.angularVelocity = new Vector3(0, 0, RotationSpeed);
		}
	}
}

