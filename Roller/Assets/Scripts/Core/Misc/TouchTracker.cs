using UnityEngine;
namespace Core
{
	public class TouchTracker : MonoBehaviour
	{
		private bool touching = false;
		private bool touched = false;
		private bool stoppedTouching = false;

		public float SecondsToConsiderStationary = 0.2f;
		private float stationaryStartTime = 0.0f;
		private bool stationary = false;


		private Vector2 initialTouchPosition;
		private Vector2 totalTouchDelta;

		private Vector2 lastFrameTouchPosition;
		private Vector2 frameTouchDelta;

		/// <summary>
		/// Returns if the user is currently touching
		/// </summary>
		/// <value><c>true</c> if this instance is touching; otherwise, <c>false</c>.</value>
		public bool IsTouching
		{
			get { return touching; }
		}

		/// <summary>
		/// Returns if the user began a touch this frame
		/// </summary>
		/// <value><c>true</c> if this instance has touched; otherwise, <c>false</c>.</value>
		public bool HasTouched
		{
			get { return touched; }
		}

		/// <summary>
		/// Returns if the user has stopped touching
		/// </summary>
		/// <value><c>true</c> if this instance has stopped touching; otherwise, <c>false</c>.</value>
		public bool HasStoppedTouching
		{
			get { return stoppedTouching; }
		}

		/// <summary>
		/// Returns if the user has swiped this frame
		/// </summary>
		/// <value><c>true</c> if this instance has swiped; otherwise, <c>false</c>.</value>
		public bool HasSwiped
		{
			get { return touching && frameTouchDelta.sqrMagnitude > Mathf.Epsilon; }
		}

		/// <summary>
		/// Returns if the user is touching and stationary
		/// </summary>
		/// <value><c>true</c> if this instance has swiped; otherwise, <c>false</c>.</value>
		public bool IsStationary
		{
			get { return stationary && Time.fixedTime - stationaryStartTime > SecondsToConsiderStationary; }
		}

		/// <summary>
		/// Returns the total touch delta since the user began the touch
		/// </summary>
		/// <value>The touch delta.</value>
		public Vector2 TouchDelta
		{
			get { return totalTouchDelta; }
		}

		/// <summary>
		/// Returns the total touch delta this frame
		/// </summary>
		/// <value>The frame touch delta.</value>
		public Vector2 TouchDeltaThisFrame
		{
			get { return frameTouchDelta; }
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update()
		{
			Vector2 touchLocation;
			
			// Begin touch
			if(HasTouchPhaseOccurred(TouchPhase.Began, out touchLocation) && !TouchHitUI(touchLocation))
			{
				initialTouchPosition = touchLocation;
				totalTouchDelta = Vector2.zero;
				lastFrameTouchPosition = initialTouchPosition;
				frameTouchDelta = Vector2.zero;
				touching = true;
				touched = true;
				stoppedTouching = false;
				stationary = false;
			}
			
			// End touch
			else if(HasTouchPhaseOccurred(TouchPhase.Ended, out touchLocation))
			{
				totalTouchDelta = Vector2.zero;
				frameTouchDelta = Vector2.zero;
				touching = false;
				touched = false;
				stoppedTouching = true;
				stationary = false;
			}
			
			// Touching and moved
			else if(touching)
			{
				if(Application.isEditor)
				{
					touchLocation = Input.mousePosition;
				}
				else
				{
					touchLocation = Input.GetTouch(0).position;
				}

				// Update touch delta
				totalTouchDelta = touchLocation - initialTouchPosition;
				frameTouchDelta = touchLocation - lastFrameTouchPosition;
				lastFrameTouchPosition = touchLocation;
				touched = false;
				stoppedTouching = false;

				// Determine if we are stationary
				if(frameTouchDelta.sqrMagnitude <= Mathf.Epsilon)
				{
					if(!stationary)
					{
						stationary = true;
						stationaryStartTime = Time.fixedTime;
					}
				}
				else
				{
					stationary = false;
				}
			}

//			// May be touching or not, but has not started/ended a touch
//			else
//			{
//				touched = false;
//				stoppedTouching = false;
//			}
		}

		/// <summary>
		/// Determines whether this instance has touch phase occurred the specified phase worldPosition.
		/// </summary>
		/// <returns><c>true</c> if this instance has touch phase occurred the specified phase worldPosition; otherwise, <c>false</c>.</returns>
		/// <param name="phase">Phase.</param>
		/// <param name="worldPosition">World position.</param>
		private bool HasTouchPhaseOccurred(TouchPhase phase, out Vector2 screenPosition)
		{
			if(Application.isEditor)
			{
				if(((phase == TouchPhase.Began && Input.GetMouseButtonDown(0)) || 
				    (phase == TouchPhase.Ended && Input.GetMouseButtonUp(0))))
				{
					screenPosition = Input.mousePosition;
					return true;
				}
			}
			else
			{
				foreach(Touch touch in Input.touches)
				{
					if(touch.phase == phase)
					{
						screenPosition = touch.position;
						return true;
					}
				}
			}
			screenPosition = new Vector2();
			return false;
		}

		/// <summary>
		/// Returns true if a touch hit the ui
		/// </summary>
		/// <returns><c>true</c>, if hit U was touched, <c>false</c> otherwise.</returns>
		/// <param name="touchPos">Touch position.</param>
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

	}
}

