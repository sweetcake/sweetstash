using UnityEngine;
using Core;
namespace Game
{
	public class BallSteeringTransform : MonoBehaviour
	{
		public float Damping = 3.0f;

		private bool touching = false;
		float initialTouchX;
		private float currentTouchXDelta = 0.0f;

		private Transform myTransform;

		void Awake()
		{
			myTransform = transform;
			myTransform.parent = GameObject.Find("Game").transform;
		}

		/// <summary>
		/// Init the specified ballTransform and ballBody.
		/// </summary>
		/// <param name="ballTransform">Ball transform.</param>
		/// <param name="ballBody">Ball body.</param>
		public void Init(Vector3 initialForward)
		{
			if(myTransform == null)
			{
				Debug.Log("My tranform is null");
			}
			//Debug.Log(myTransform.forward);
			//Debug.Log(initialForward);
			myTransform.forward = initialForward;
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update()
		{
			updateTransformFromInput();
		}

		/// <summary>
		/// Takes input, and recalculates transform
		/// </summary>
		private void updateTransformFromInput()
		{			
			Vector2 touchLocation;
			
			// Begin touch
			if(HasTouchPhaseOccurred(TouchPhase.Began, out touchLocation) && !TouchHitUI(touchLocation))
			{
				//Debug.Log("TOUCH LOCATION: " + touchLocation.x);
				initialTouchX = touchLocation.x;
				currentTouchXDelta = 0.0f;
				touching = true;
			}
			
			// End touch
			else if(HasTouchPhaseOccurred(TouchPhase.Ended, out touchLocation))
			{
				currentTouchXDelta = 0.0f;
				initialTouchX = 0.0f;
				touching = false;
			}
			
			// Either touching, or not
			else if(touching)
			{
				float touchDelta = GetTouchXDelta();
				//currentTouchXDelta += touchDelta;
				//Debug.Log("PRECLAMPED TOUCH DELTA: " + currentTouchXDelta);
				currentTouchXDelta = Mathf.Clamp(touchDelta, -1, 1);
				//Debug.Log("TOUCH DELTA: " + currentTouchXDelta);

				float interpTime;
				if(currentTouchXDelta < 0)
				{
					interpTime = currentTouchXDelta / -1;
				}
				else
				{
					interpTime = currentTouchXDelta / 1;
				}
				float delta = QuadraticEaseIn(interpTime, 0, currentTouchXDelta);
				if(!MathUtils.IsEqual(delta, 0.01f))
				{
					float angle = (20.0f) * delta;
					Quaternion destinationRotation = myTransform.rotation * Quaternion.AngleAxis(angle, up);
					myTransform.rotation = Quaternion.Slerp(myTransform.rotation, destinationRotation, Time.deltaTime * Damping);
				}
			}
		}

		
		public static bool TouchHitUI(Vector3 touchPos)
		{
			// This grabs the camera attached to the NGUI UI_Root object.
			Camera nguiCam = GameObject.Find("UI Root").GetComponentInChildren<Camera>();
			if(nguiCam != null)
			{
				Ray inputRay = nguiCam.ScreenPointToRay(touchPos);
				RaycastHit hit;				
				if(Physics.Raycast(inputRay.origin, inputRay.direction, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("UI")))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Determines whether this instance has touch phase occurred the specified phase worldPosition.
		/// </summary>
		/// <returns><c>true</c> if this instance has touch phase occurred the specified phase worldPosition; otherwise, <c>false</c>.</returns>
		/// <param name="phase">Phase.</param>
		/// <param name="worldPosition">World position.</param>
		private bool HasTouchPhaseOccurred(TouchPhase phase, out Vector2 worldPosition)
		{
			if(Application.isEditor)
			{
				if(((phase == TouchPhase.Began && Input.GetMouseButtonDown(0)) || 
				    (phase == TouchPhase.Ended && Input.GetMouseButtonUp(0))))
				{
					worldPosition = Input.mousePosition;
					return true;
				}
			}
			else
			{
				foreach(Touch touch in Input.touches)
				{
					if(touch.phase == phase)
					{
						worldPosition = touch.position;
						return true;
					}
				}
			}
			worldPosition = new Vector2();
			return false;
		}
		
		/// <summary>
		/// Uses quadtratic ease in to determine the desired value
		/// </summary>
		/// <returns>The ease out.</returns>
		/// <param name="time">Time.</param>
		/// <param name="start">Start.</param>
		/// <param name="totalDelta">Total delta.</param>
		private float QuadraticEaseIn(float time, float start, float totalDelta)
		{
			time = Mathf.Clamp(time, 0, 1);
			return totalDelta * time * time + start;
		}
		
		/// <summary>
		/// Uses quadtratic ease out to determine the desired value
		/// </summary>
		/// <returns>The ease out.</returns>
		/// <param name="time">Time.</param>
		/// <param name="start">Start.</param>
		/// <param name="totalDelta">Total delta.</param>
		private float QuadraticEaseOut(float time, float start, float totalDelta)
		{
			time = Mathf.Clamp(time, 0, 1);
			return -totalDelta * time * (time - 2) + start;
		}

		/// <summary>
		/// Returns change in x of touch in world space
		/// </summary>
		/// <returns>The touch X delta.</returns>
		private float GetTouchXDelta()
		{
			if(Application.isEditor)
			{
				if(touching)
				{
					float returnVal = Input.mousePosition.x - initialTouchX;
					return returnVal / 100;
				}
				return 0.0f;
			}
			else
			{
				float largestDelta = 0.0f;
				foreach(Touch touch in Input.touches)
				{
					float delta = touch.position.x - initialTouchX;
					if(Mathf.Abs(delta) > Mathf.Abs(largestDelta))
					{
						largestDelta = delta;
					}
				}
				return largestDelta / 100;
			}
		}

		public Vector3 right
		{
			get{ return myTransform.right; }
		}

		public Vector3 up
		{
			get{ return myTransform.up; }
		}

		public Vector3 forward
		{
			get{ return myTransform.forward; }
		}

		public Vector3 position
		{
			get{ return myTransform.position; }
			set{ myTransform.position = value; }
		}
	}
}

