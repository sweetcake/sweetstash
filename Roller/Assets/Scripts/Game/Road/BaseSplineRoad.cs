using UnityEngine;
namespace Game
{
	[RequireComponent(typeof(Spline))]
	[RequireComponent(typeof(SplineMesh))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(MeshCollider))]
	public class BaseSplineRoad : MonoBehaviour
	{
		public Spline Spline;
		public SplineMesh Mesh;

		/// <summary>
		/// Init this instance.
		/// </summary>
//		public virtual void Init()
//		{
//			UpdateRoad();
//		}

		/// <summary>
		/// Populate this instance.
		/// </summary>
		public virtual void Populate()
		{
			UpdateRoad();
		}

		/// <summary>
		/// Populate this instance using the given spline
		/// </summary>
		/// <param name="spline">Spline.</param>
		public virtual void Populate(Spline spline)
		{
			UpdateRoad();
		}

		/// <summary>
		/// Updates the road, USE SPARINGLY, EXTREMELY EXPENSIVE
		/// </summary>
		protected virtual void UpdateRoad()
		{
			if(Spline != null)
			{
				Spline.UpdateSpline();
			}
			if(Mesh != null)
			{
				Mesh.UpdateMesh();
			}
		}

		/// <summary>
		/// Creates a new spline. Uses the starting node as the first node, if it is not null. Otherwise, it uses the starting position
		/// </summary>
		/// <param name="length">Length.</param>
		/// <param name="startingPosition">Starting position.</param>
		/// <param name="startingNode">Starting node.</param>
//		public virtual void Create(float length, Vector3 startingPosition, float targetLength)
//		{
//			Debug.Log("START: " + startingPosition);
//
//			float splineLength = spline.Length;
//			SplineNode lastNode = null;
//			while(splineLength < targetLength)
//			{
//				float time = Mathf.Clamp(splineLength / targetLength, 0.0f, 1.0f);
//				
//				// Insert node
//				lastNode = AddNode(lastNode, startingPosition, time);
//				
//				// Update spline
//				spline.UpdateSpline();
//				
//				// Recalc length
//				splineLength = spline.Length;
//			}
//
//			Debug.Log("End: " + LastNode.transform.position);
//		}

		/// <summary>
		/// Adds the node.
		/// </summary>
		/// <returns>The node.</returns>
		/// <param name="lastNode">Last node.</param>
		/// <param name="offsetTime">Offset time.</param>
//		private SplineNode AddNode(SplineNode lastNode, Vector3 position, float offsetTime)
//		{
//			// Validate offset between 0 and 1
//			offsetTime = Mathf.Clamp(offsetTime, 0.0f, 1.0f);
//			
//			// Get last node position	
//			Vector3 nodePosition;
//			if(lastNode == null)
//			{
//				nodePosition = position;
//			}
//			else
//			{
//				nodePosition = lastNode.transform.position + Vector3.forward * 25.0f;
//			}
//			
//			// Add node
//			SplineNode newNode = spline.AddSplineNode(lastNode).GetComponent<SplineNode>();
//			
//			// Horizontal
////			float min = Mathf.Lerp(StartingMinHorizontalOffset, EndingMinHorizontalOffset, offsetTime);
////			float max = Mathf.Lerp(StartingMaxHorizontalOffset, EndingMaxHorizontalOffset, offsetTime);
//			nodePosition += Vector3.right * Random.Range(-5, 5);
//			
//			// Vertical
////			min = Mathf.Lerp(StartingMinVerticalOffset, EndingMinVerticalOffset, offsetTime);
////			max = Mathf.Lerp(StartingMaxVerticalOffset, EndingMaxVerticalOffset, offsetTime);
//			nodePosition += Vector3.up * Random.Range(-1, 1);
//			
//			// Set parent
//			newNode.transform.parent = transform;
//			
//			// Set name
//			newNode.gameObject.name = "Node" + (spline.splineNodesArray.Count - 1);
//			
//			// Set position
//			newNode.transform.position = nodePosition;
//			
//			return newNode;
//		}
		
		public SplineNode FirstNode
		{
			get 
			{
				if(Spline != null && Spline.splineNodesArray.Count > 0)
				{
					return Spline.splineNodesArray[0];
				}
				return null;
			}
		}

		public SplineNode LastNode
		{
			get 
			{
				if(Spline != null && Spline.splineNodesArray.Count > 0)
				{
					return Spline.splineNodesArray[Spline.splineNodesArray.Count - 1];
				}
				return null;
			}
		}

	}
}

