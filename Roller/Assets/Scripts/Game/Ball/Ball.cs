using UnityEngine;
using Core;
namespace Game
{
	public class Ball : MonoBehaviour
	{
		public delegate void DeathDelegate();
		public event DeathDelegate DeathCallback;

		public float AngularAcceleration = 10.0f;
		public float OffroadTimeTillDeath = 0.5f;

		private Rigidbody myRigidBody;
		private Transform myTransform;
		private BallSteeringTransform ballSteeringTransform;

		private bool activated = false;
		private bool alive = true;

		void Awake()
		{
			//Debug.Log("AWAKE ball");
			myRigidBody = rigidbody;
			myTransform = transform;
			ballSteeringTransform = transform.FindChild("BallSteeringTransform").GetComponent<BallSteeringTransform>();
			Deactivate();
		}

		public void InitTransform(Transform start)
		{
			myTransform.position = start.position;
			myTransform.forward = start.forward;
		}

		public void Activate()
		{
			leftRoad = false;
			activated = true;
			alive = true;
			ballSteeringTransform.gameObject.SetActive(true);
			ballSteeringTransform.Init(myTransform.forward);
			myRigidBody.useGravity = true;
			myRigidBody.isKinematic = false;
			myRigidBody.velocity = Vector3.zero;
			myRigidBody.angularVelocity = Vector3.zero; 
		}

		public void Deactivate()
		{
			leftRoad = false;
			activated = false;
			alive = false;
			ballSteeringTransform.gameObject.SetActive(false);
			myRigidBody.useGravity = false;
			if(!myRigidBody.isKinematic)
			{
				myRigidBody.velocity = Vector3.zero;
			}
			myRigidBody.isKinematic = true;
		}


		private float leftRoadTime;
		private bool leftRoad = false;
		void OnCollisionExit(Collision collision)
		{
			if(collision.gameObject.tag == "Road")
			{
				leftRoad = true;
				leftRoadTime = Time.time;
			}
		}

		void OnCollisionEnter(Collision collision)
		{
			if(collision.gameObject.tag == "Road")
			{
				leftRoad = false;
			}
		}

		void Update()
		{
			if(!activated)
			{
				return;
			}

			if(alive && leftRoad && (Time.time - leftRoadTime) > OffroadTimeTillDeath)
			{
				leftRoad = false;
				alive = false;
				if(DeathCallback != null)
				{
					DeathCallback();
				}
			}

			ballSteeringTransform.position = myTransform.position;
		}

		void LateUpdate()
		{
			if(!activated)
			{
				return;
			}

			Quaternion q = myTransform.rotation * myRigidBody.inertiaTensorRotation;
			Vector3 torque = q * Vector3.Scale(myRigidBody.inertiaTensor, (Quaternion.Inverse(q) * ballSteeringTransform.right)) * AngularAcceleration * Time.deltaTime;
			rigidbody.AddTorque(torque, ForceMode.Impulse);
		}

		public bool Alive 
		{
			get { return alive; }
		}
	}
}

