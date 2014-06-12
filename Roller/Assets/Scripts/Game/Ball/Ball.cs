using UnityEngine;
using Core;
namespace Game
{
	public class Ball : MonoBehaviour
	{
		public float BumperForceMagnitude = 10.0f;

		public delegate void DeathDelegate();
		public event DeathDelegate DeathCallback;

		public float AngularAcceleration = 10.0f;
		public float OffroadTimeTillDeath = 0.5f;
		public bool UseSwipeInput = false;
		private bool wasUsingSwipeInput = false;

		public float HoldToSlowDownDrag = 0.1f;

		private float currentForwardAngularAcceleration = 0.0f;
		private float currentRightAngularAcceleration = 0.0f;
		public float SwipeTorqueScalarVertical = 1.0f;
		public float SwipeTorqueScalarHorizontal = 1.0f;

		public float TorqueDragVertical = 1.0f;
		public float TorqueDragHorizontal = 1.0f;

		private Rigidbody myRigidBody;
		private Transform myTransform;
		private BallSteeringTransform ballSteeringTransform; 

		private TouchTracker touchTracker;

		private bool activated = false;
		private bool alive = true;

		void Awake()
		{
			//Debug.Log("AWAKE ball");
			myRigidBody = rigidbody;
			myTransform = transform;
			touchTracker = GetComponent<TouchTracker>();
			ballSteeringTransform = transform.FindChild("BallSteeringTransform").GetComponent<BallSteeringTransform>();
			Deactivate();

			// Set up camera follow type
			wasUsingSwipeInput = !UseSwipeInput;
			UpdateCameraType();
		}

		private void UpdateCameraType()
		{
			SmoothFollow smoothFollow = Camera.main.GetComponent<SmoothFollow>();
			SmoothZFollow smoothAxisFollow = Camera.main.GetComponent<SmoothZFollow>();


			if(UseSwipeInput && !wasUsingSwipeInput)
			{
				smoothFollow.enabled = false;
				smoothAxisFollow.enabled = true;
				wasUsingSwipeInput = true;
			}
			else if(!UseSwipeInput && wasUsingSwipeInput)
			{
				smoothFollow.enabled = true;
				smoothAxisFollow.enabled = false;
				wasUsingSwipeInput = false;
			}
		}

		public void InitTransform(Transform start)
		{
			myTransform.position = start.position;
			myTransform.forward = start.forward;
		}

		public void Activate()
		{
			onRoad = true;
			activated = true;
			alive = true;
			ballSteeringTransform.gameObject.SetActive(true);
			ballSteeringTransform.Init(myTransform.forward);
			myRigidBody.useGravity = true;
			myRigidBody.isKinematic = false;
			myRigidBody.velocity = Vector3.zero;
			myRigidBody.angularVelocity = Vector3.zero; 
			currentForwardAngularAcceleration = 0.0f;
			currentRightAngularAcceleration = 0.0f;
		}

		public void Deactivate()
		{
			onRoad = true;
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
		private bool onRoad = true;
		void OnCollisionExit(Collision collision)
		{
			if(collision.gameObject.tag == "Road")
			{
				onRoad = false;
				leftRoadTime = Time.time;
			}
		}

		void OnCollisionEnter(Collision collision)
		{
			if(collision.gameObject.tag == "Road")
			{
				onRoad = true;
			}
			else if(collision.gameObject.tag == "Bumper")
			{
				// Add a reflective force
				Vector3 forceDirection = (myTransform.position - collision.gameObject.transform.position).normalized;			
				forceDirection.z = 0.0f;
				myRigidBody.AddForce(forceDirection * BumperForceMagnitude);
			}
		}

		void OnCollisionStay(Collision collision)
		{
			if(!onRoad && collision.gameObject.tag == "Road")
			{
				onRoad = true;
			}
		}

		void OnTriggerEnter(Collider other) 
		{
			if(other.gameObject.tag == "Collectable")
			{
				other.gameObject.SetActive(false);
			}
		}

		void Update()
		{
			if(!activated)
			{
				return;
			}

			UpdateCameraType();

			if(alive && !onRoad && (Time.time - leftRoadTime) > OffroadTimeTillDeath)
			{
				onRoad = true;
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

			if(UseSwipeInput)
			{
				// Don't add torque if not on the road
				if(onRoad)
				{
					Vector3 torque;
					if(touchTracker.HasSwiped)
					{
						// Get the current swipe
						Vector2 frameSwipe = touchTracker.TouchDeltaThisFrame;
						if(Mathf.Abs(frameSwipe.x) > Mathf.Abs(frameSwipe.y))
						{
							currentRightAngularAcceleration += SwipeTorqueScalarHorizontal * frameSwipe.x * -1;
						}
						else
						{
							currentForwardAngularAcceleration += SwipeTorqueScalarVertical * frameSwipe.y;
						}
					}
					else if(touchTracker.IsStationary)
					{
						currentForwardAngularAcceleration *= HoldToSlowDownDrag;
						currentRightAngularAcceleration *= HoldToSlowDownDrag;
						rigidbody.angularVelocity *= HoldToSlowDownDrag;
					}

					torque = new Vector3(currentForwardAngularAcceleration * Time.deltaTime, 0.0f, currentRightAngularAcceleration * Time.deltaTime);
					rigidbody.AddTorque(torque);

					// Add drag
					currentForwardAngularAcceleration *= TorqueDragVertical;
					currentRightAngularAcceleration *= TorqueDragHorizontal;
				}
			}
			else
			{
				Quaternion q = myTransform.rotation * myRigidBody.inertiaTensorRotation;
				Vector3 torque = q * Vector3.Scale(myRigidBody.inertiaTensor, (Quaternion.Inverse(q) * ballSteeringTransform.right)) * AngularAcceleration * Time.deltaTime;
				rigidbody.AddTorque(torque, ForceMode.Impulse);
			}
		}

		public bool Alive 
		{
			get { return alive; }
		}
	}
}

